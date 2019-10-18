﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWay.Skynet.Cloud.IE.Core.Models;
using UWay.Skynet.Cloud.IE.Core;
using UWay.Skynet.Cloud;
using System.Linq.Dynamic.Core;
using System.IO;
using Aspose.Cells;
using System.Reflection;
using System.Drawing;
using System.Data;

namespace UWay.Skynet.Cloud.IE.Excel.Utility
{
    public class ImportDataTableHelper:IDisposable
    {
        /// <summary>
        ///     导入文件路径
        /// </summary>
        protected string FilePath { get; set; }


        /// <summary>
        ///     导入结果
        /// </summary>
        protected DataTable ImportResult { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="filePath"></param>
        public ImportDataTableHelper(string filePath = null) => FilePath = filePath;

        public void Dispose()
        {
            //ExcelImporterAttribute = null;
            FilePath = null;
            //ImporterHeaderInfos = null;
            ImportResult = null;
            GC.Collect();
        }

        public Task<DataTable> Import(string filePath,  Action<DataTable> addHeaderAction)
        {
            
            CheckImportFile(FilePath);
            ImportResult = new DataTable();
            addHeaderAction(ImportResult);

            using (Stream stream = new FileStream(FilePath, FileMode.Open))
            {
                using (var excelPackage = new Workbook(stream))
                {
                    //Pars
                }
            }
            return Task.FromResult(ImportResult);
        }


        /// <summary>
        ///     检查导入文件路劲
        /// </summary>
        /// <exception cref="ArgumentException">文件路径不能为空! - filePath</exception>
        private static void CheckImportFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("文件路径不能为空!", nameof(filePath));
            }

            //TODO:在Docker容器中存在文件路径找不到问题，暂时先注释掉
            //if (!File.Exists(filePath))
            //{
            //    throw new ImportException("导入文件不存在!");
            //}
        }
    }


    public class ImportHelper<T> : IDisposable where T : class, new()
    {
        /// <summary>
        /// </summary>
        /// <param name="filePath"></param>
        public ImportHelper(string filePath = null) => FilePath = filePath;

        /// <summary>
        ///     导入全局设置
        /// </summary>
        protected ExcelImporterAttribute ExcelImporterAttribute { get; set; }

        /// <summary>
        ///     导入文件路径
        /// </summary>
        protected string FilePath { get; set; }

        /// <summary>
        ///     导入结果
        /// </summary>
        protected ImportResult<T> ImportResult { get; set; }

        /// <summary>
        ///     列头定义
        /// </summary>
        protected List<ImporterHeaderInfo> ImporterHeaderInfos { get; set; }

        public void Dispose()
        {
            ExcelImporterAttribute = null;
            FilePath = null;
            ImporterHeaderInfos = null;
            ImportResult = null;
            GC.Collect();
        }




        /// <summary>
        ///     导入模型验证数据
        /// </summary>
        /// <returns></returns>
        public Task<ImportResult<T>> Import(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                FilePath = filePath;
            }

            ImportResult = new ImportResult<T>
            {
                RowErrors = new List<DataRowErrorInfo>()
            };
            ExcelImporterAttribute =
                typeof(T).GetAttribute<ExcelImporterAttribute>(true) ?? new ExcelImporterAttribute();

            try
            {
                CheckImportFile(FilePath);

                using (Stream stream = new FileStream(FilePath, FileMode.Open))
                {
                    using (var excelPackage = new Workbook(stream))
                    {
                        #region 检查模板

                        ParseTemplate(excelPackage);
                        if (ImportResult.HasError)
                        {
                            return Task.FromResult(ImportResult);
                        }

                        #endregion

                        ParseData(excelPackage);
                        #region 数据验证

                        for (var i = 0; i < ImportResult.Data.Count; i++)
                        {
                            
                            var isValid = ValidatorHelper.TryValidate(ImportResult.Data.ElementAt(i), out var validationResults);
                            if (!isValid)
                            {
                                var dataRowError = new DataRowErrorInfo
                                {
                                    RowIndex = ExcelImporterAttribute.HeaderRowIndex + i + 1
                                };
                                foreach (var validationResult in validationResults)
                                {
                                    var key = validationResult.MemberNames.First();
                                    var column = ImporterHeaderInfos.FirstOrDefault(a => a.PropertyName == key);
                                    if (column != null)
                                    {
                                        key = column.ExporterHeader.Name;
                                    }

                                    var value = validationResult.ErrorMessage;
                                    if (dataRowError.FieldErrors.ContainsKey(key))
                                    {
                                        dataRowError.FieldErrors[key] += Environment.NewLine + value;
                                    }
                                    else
                                    {
                                        dataRowError.FieldErrors.Add(key, value);
                                    }
                                }

                                ImportResult.RowErrors.Add(dataRowError);
                            }
                        }

                        RepeatDataCheck();

                        #endregion

                        LabelingError(excelPackage);
                    }
                }
            }
            catch (Exception ex)
            {
                ImportResult.Exception = ex;
            }

            return Task.FromResult(ImportResult);
        }

        /// <summary>
        /// 检查重复数据
        /// </summary>
        private void RepeatDataCheck()
        {
            //获取需要检查重复数据的列
            var notAllowRepeatCols = ImporterHeaderInfos.Where(p => p.ExporterHeader.IsAllowRepeat == false).ToList();
            if (notAllowRepeatCols.Count == 0)
            {
                return;
            }

            var rowIndex = ExcelImporterAttribute.HeaderRowIndex;
            var qDataList = ImportResult.Data.Select(p =>
            {
                rowIndex++;
                return new { RowIndex = rowIndex, RowData = p };
            }).ToList().AsQueryable();

            foreach (var notAllowRepeatCol in notAllowRepeatCols)
            {
                //查询指定列
                var qDataByProp = qDataList
                    .Select($"new(RowData.{notAllowRepeatCol.PropertyName} as Value, RowIndex)")
                    .OrderBy("Value").ToDynamicList();

                //重复行的行号
                var listRepeatRows = new List<int>();
                for (var i = 0; i < qDataByProp.Count; i++)
                {
                    //当前行值
                    var currentValue = qDataByProp[i].Value;
                    if (i == 0 || string.IsNullOrEmpty(currentValue?.ToString()))
                    {
                        continue;
                    }

                    //上一行的值
                    var preValue = qDataByProp[i - 1].Value;
                    if (currentValue == preValue)
                    {
                        listRepeatRows.Add(qDataByProp[i - 1].RowIndex);
                        listRepeatRows.Add(qDataByProp[i].RowIndex);
                        //如果不是最后一行，则继续检测
                        if (i != qDataByProp.Count - 1)
                        {
                            continue;
                        }
                    }

                    if (listRepeatRows.Count == 0)
                    {
                        continue;
                    }

                    var errorIndexsStr = string.Join("，", listRepeatRows.Distinct());
                    foreach (var repeatRow in listRepeatRows.Distinct())
                    {
                        var dataRowError = ImportResult.RowErrors.FirstOrDefault(p => p.RowIndex == repeatRow);
                        if (dataRowError == null)
                        {
                            dataRowError = new DataRowErrorInfo
                            {
                                RowIndex = repeatRow
                            };
                            ImportResult.RowErrors.Add(dataRowError);
                        }

                        var key = notAllowRepeatCol.ExporterHeader?.Name ??
                                  notAllowRepeatCol.PropertyName;
                        var error = $"存在数据重复，请检查！所在行：{errorIndexsStr}。";
                        if (dataRowError.FieldErrors.ContainsKey(key))
                        {
                            dataRowError.FieldErrors[key] += Environment.NewLine + error;
                        }
                        else
                        {
                            dataRowError.FieldErrors.Add(key, error);
                        }
                    }

                    listRepeatRows.Clear();
                }
            }
        }

        /// <summary>
        ///     标注错误
        /// </summary>
        /// <param name="excelPackage"></param>
        protected virtual void LabelingError(Workbook excelPackage)
        {
            //是否标注错误
            if (ExcelImporterAttribute.IsLabelingError && ImportResult.HasError)
            {
                var worksheet = GetImportSheet(excelPackage);

                //TODO:标注模板错误
                //标注数据错误
                foreach (var item in ImportResult.RowErrors)
                {
                    foreach (var field in item.FieldErrors)
                    {
                        var col = ImporterHeaderInfos.First(p => p.ExporterHeader.Name == field.Key);
                        var cell = worksheet.Cells[item.RowIndex, col.ExporterHeader.ColumnIndex];
                        var style = excelPackage.CreateStyle();
                        style.Font.Color = (Color.Red);
                        style.Font.IsBold = true;
                        cell.SetStyle(style);
                        worksheet.Comments[item.RowIndex, col.ExporterHeader.ColumnIndex].Note = string.Join(",", field.Value);
                        worksheet.Comments[item.RowIndex, col.ExporterHeader.ColumnIndex].Author = col.ExporterHeader.Author;
                    }
                }

                var ext = Path.GetExtension(FilePath);
                excelPackage.Save(FilePath.Replace(ext, "_" + ext));
            }
        }

        /// <summary>
        ///     检查导入文件路劲
        /// </summary>
        /// <exception cref="ArgumentException">文件路径不能为空! - filePath</exception>
        private static void CheckImportFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("文件路径不能为空!", nameof(filePath));
            }

            //TODO:在Docker容器中存在文件路径找不到问题，暂时先注释掉
            //if (!File.Exists(filePath))
            //{
            //    throw new ImportException("导入文件不存在!");
            //}
        }

        /// <summary>
        /// 解析模板
        /// </summary>
        /// <returns></returns>
        protected virtual void ParseTemplate(Workbook excelPackage)
        {
            ImportResult.TemplateErrors = new List<TemplateErrorInfo>();
            //获取导入实体列定义
            ParseImporterHeader(out var enumColumns, out var boolColumns);
            try
            {
                //根据名称获取Sheet，如果不存在则取第一个
                var worksheet = GetImportSheet(excelPackage);
                var excelHeaders = new Dictionary<string, int>();
                var endColumnCount = ExcelImporterAttribute.EndColumnCount ?? worksheet.Cells.MaxColumn;
                for (var columnIndex = 1; columnIndex <= endColumnCount; columnIndex++)
                {
                    var header = worksheet.Cells[ExcelImporterAttribute.HeaderRowIndex, columnIndex].Value;

                    //如果未设置读取的截止列，则默认指定为出现空格，则读取截止
                    if (ExcelImporterAttribute.EndColumnCount.HasValue &&
                        columnIndex > ExcelImporterAttribute.EndColumnCount.Value ||
                        header== null)
                    {
                        break;
                    }

                    //不处理空表头
                    if (header == null)
                    {
                        continue;
                    }

                    if (excelHeaders.ContainsKey(header.ToString()))
                    {
                        ImportResult.TemplateErrors.Add(new TemplateErrorInfo
                        {
                            ErrorLevel = ErrorLevels.Error,
                            ColumnName = header.ToString(),
                            RequireColumnName = null,
                            Message = "列头重复！"
                        });
                    }

                    excelHeaders.Add(header.ToString(), columnIndex);
                }

                foreach (var item in ImporterHeaderInfos)
                {
                    if (!excelHeaders.ContainsKey(item.ExporterHeader.Name))
                    {
                        //仅验证必填字段
                        if (item.IsRequired)
                        {
                            ImportResult.TemplateErrors.Add(new TemplateErrorInfo
                            {
                                ErrorLevel = ErrorLevels.Error,
                                ColumnName = null,
                                RequireColumnName = item.ExporterHeader.Name,
                                Message = "当前导入模板中未找到此字段！"
                            });
                            continue;
                        }

                        ImportResult.TemplateErrors.Add(new TemplateErrorInfo
                        {
                            ErrorLevel = ErrorLevels.Warning,
                            ColumnName = null,
                            RequireColumnName = item.ExporterHeader.Name,
                            Message = "当前导入模板中未找到此字段！"
                        });
                    }
                    else
                    {
                        item.IsExist = true;
                        //设置列索引
                        if (item.ExporterHeader.ColumnIndex == 0)
                        {
                            item.ExporterHeader.ColumnIndex = excelHeaders[item.ExporterHeader.Name];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ImportResult.TemplateErrors.Add(new TemplateErrorInfo
                {
                    ErrorLevel = ErrorLevels.Error,
                    ColumnName = null,
                    RequireColumnName = null,
                    Message = $"模板出现未知错误：{ex}"
                });
                throw new Exception($"模板出现未知错误：{ex.Message}", ex);
            }
        }

        /// <summary>
        ///     解析头部
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">导入实体没有定义ImporterHeader属性</exception>
        protected virtual bool ParseImporterHeader(out Dictionary<int, IDictionary<string, int>> enumColumns,
            out List<int> boolColumns)
        {
            ImporterHeaderInfos = new List<ImporterHeaderInfo>();
            enumColumns = new Dictionary<int, IDictionary<string, int>>();
            boolColumns = new List<int>();
            var objProperties = typeof(T).GetProperties();
            if (objProperties.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < objProperties.Length; i++)
            {
                //TODO:简化并重构
                //如果不设置，则自动使用默认定义
                var importerHeaderAttribute =
                    (objProperties[i].GetCustomAttributes(typeof(ImporterHeaderAttribute), true) as
                        ImporterHeaderAttribute[])?.FirstOrDefault() ?? new ImporterHeaderAttribute
                        {
                            Name = objProperties[i].GetDisplayName() ?? objProperties[i].Name
                        };

                if (string.IsNullOrWhiteSpace(importerHeaderAttribute.Name))
                {
                    importerHeaderAttribute.Name = objProperties[i].GetDisplayName() ?? objProperties[i].Name;
                    importerHeaderAttribute.DictioaryTypeId = objProperties[i].GetGroupName();
                    //if ()
                    //{

                    //}
                }

                ImporterHeaderInfos.Add(new ImporterHeaderInfo
                {
                    IsRequired = objProperties[i].IsRequired(),
                    PropertyName = objProperties[i].Name,
                    ExporterHeader = importerHeaderAttribute,
                });

                
                if (objProperties[i].PropertyType.BaseType?.Name.ToLower() == "enum")
                {
                    enumColumns.Add(i + 1, objProperties[i].PropertyType.GetEnumDisplayNames());
                }


                if (objProperties[i].PropertyType == typeof(bool))
                {
                    boolColumns.Add(i + 1);
                }
            }

            return true;
        }

        /// <summary>
        ///     构建Excel模板
        /// </summary>
        protected virtual void StructureExcel(Workbook excelPackage)
        {
            var worksheet =
                excelPackage.Worksheets.Add(typeof(T).GetDisplayName() ??
                                                     ExcelImporterAttribute.SheetName ?? "导入数据");
            if (!ParseImporterHeader(out var enumColumns, out var boolColumns))
            {
                return;
            }

            //设置列头
            for (var i = 0; i < ImporterHeaderInfos.Count; i++)
            {
                worksheet.Cells[1, i + 1].PutValue(ImporterHeaderInfos[i].ExporterHeader.Name);
                if (!string.IsNullOrWhiteSpace(ImporterHeaderInfos[i].ExporterHeader.Description))
                {
                    worksheet.Comments[1, i + 1].Note = ImporterHeaderInfos[i].ExporterHeader.Description;
                    worksheet.Comments[1, i + 1].Author = ImporterHeaderInfos[i].ExporterHeader.Author;
                }
                //如果必填，则列头标红
                if (ImporterHeaderInfos[i].IsRequired)
                {
                    var style = excelPackage.CreateStyle();
                    style.Font.Color = Color.Red;
                    
                    worksheet.Cells[1, i + 1].SetStyle(style);
                    //worksheet.Cells[1, i + 1]..Font.Color.SetColor(Color.Red);
                } 
            }

            worksheet.AutoFitColumns();
            var cellStyle = excelPackage.CreateStyle();
            cellStyle.IsTextWrapped = true;
            //worksheet.Cells.Style.WrapText = true;
            cellStyle.HorizontalAlignment = TextAlignmentType.Center;
            cellStyle.VerticalAlignment = TextAlignmentType.Center;
            cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.DarkGray);
            cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.DarkGray);
            cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.DarkGray);
            cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.DarkGray);
            cellStyle.ShrinkToFit = true;
            cellStyle.BackgroundColor = Color.DarkSeaGreen;
            StyleFlag styleFlag = new StyleFlag();
            
            worksheet.Cells.ApplyStyle(cellStyle, styleFlag);

            //CellArea
            ValidationCollection vcollections =  worksheet.Validations;
            //excelPackage.ValidateFormula()
               
            //枚举处理
            foreach (var enumColumn in enumColumns)
            {
                CellArea area = new CellArea
                {
                    StartRow = 1,
                    EndRow = Int32.MaxValue,
                    StartColumn = enumColumn.Key,
                    EndColumn = enumColumn.Key
                };
                Validation validation = vcollections[vcollections.Add(area)];
                validation.Type = ValidationType.List;
                validation.Operator = OperatorType.None;
                validation.InCellDropDown = true;
                validation.Formula1 = enumColumn.Value.Values.JoinString(",");
                validation.ShowError = true;
                //Set the alert type severity level.
                validation.AlertStyle = ValidationAlertType.Stop;
                // Set the error title.
                validation.ErrorTitle = "Error";
                // Set the error message.
                validation.ErrorMessage = "Please select a color from the list";
            }

            //Bool类型处理
            foreach (var boolColumn in boolColumns)
            {
                CellArea area = new CellArea
                {
                    StartRow = 1,
                    EndRow = Int32.MaxValue,

                    StartColumn = boolColumn,
                    EndColumn = boolColumn
                };
                Validation validation = vcollections[vcollections.Add(area)];
                validation.Type = ValidationType.List;
                validation.Operator = OperatorType.None;
                validation.InCellDropDown = true;
                validation.Formula1 = "是,否";
                validation.ShowError = true;
                //Set the alert type severity level.
                validation.AlertStyle = ValidationAlertType.Stop;
                // Set the error title.
                validation.ErrorTitle = "Error";
                // Set the error message.
                validation.ErrorMessage = "Please select a color from the list";

            }
        }

        /// <summary>
        ///     解析数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">最大允许导入条数不能超过5000条</exception>
        protected virtual void ParseData(Workbook excelPackage)
        {
            var worksheet = GetImportSheet(excelPackage);
            if (worksheet.Cells.MaxDataRow > 5000)
            {
                throw new ArgumentException("最大允许导入条数不能超过5000条");
            }

            ImportResult.Data = new List<T>();
            var propertyInfos = new List<PropertyInfo>(typeof(T).GetProperties());

            for (var rowIndex = ExcelImporterAttribute.HeaderRowIndex + 1;
                rowIndex <= worksheet.Cells.MaxDataRow;
                rowIndex++)
            {
                var isNullNumber = 1;
                for (var column = 1; column < worksheet.Cells.MaxColumn; column++)
                {
                    if (worksheet.Cells[rowIndex, column].Value == null)
                    {
                        isNullNumber++;
                    }
                }

                if (isNullNumber < worksheet.Cells.MaxDataColumn)
                {
                    var dataItem = new T();
                    foreach (var propertyInfo in propertyInfos)
                    {
                        var col = ImporterHeaderInfos.Find(a => a.PropertyName == propertyInfo.Name);
                        //检查Excel中是否存在
                        if (!col.IsExist)
                        {
                            continue;
                        }

                        var cell = worksheet.Cells[rowIndex, col.ExporterHeader.ColumnIndex];
                        try
                        {
                            switch (propertyInfo.PropertyType.BaseType?.Name)
                            {
                                case "Enum":
                                    var enumDisplayNames = propertyInfo.PropertyType.GetEnumDisplayNames();
                                    if (enumDisplayNames.ContainsKey(
                                        cell.Value?.ToString() ?? throw new ArgumentException()))
                                    {
                                        propertyInfo.SetValue(dataItem,
                                            enumDisplayNames[cell.Value?.ToString()]);
                                    }
                                    else
                                    {
                                        AddRowDataError(rowIndex, col, $"值 {cell.Value} 不存在模板下拉选项中");
                                    }

                                    continue;
                            }

                            var cellValue = cell.Value?.ToString();
                            switch (propertyInfo.PropertyType.GetCSharpTypeName())
                            {
                                case "Boolean":
                                    propertyInfo.SetValue(dataItem, GetBooleanValue(cellValue));
                                    break;
                                case "Nullable<Boolean>":
                                    propertyInfo.SetValue(dataItem,
                                        string.IsNullOrWhiteSpace(cellValue)
                                            ? (bool?)null
                                            : GetBooleanValue(cellValue));
                                    break;
                                case "String":
                                    //TODO:进一步优化
                                    //移除所有的空格，包括中间的空格
                                    if (col.ExporterHeader.FixAllSpace)
                                    {
                                        propertyInfo.SetValue(dataItem, cellValue?.Replace(" ", string.Empty));
                                    }
                                    else if (col.ExporterHeader.AutoTrim)
                                    {
                                        propertyInfo.SetValue(dataItem, cellValue?.Trim());
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(dataItem, cellValue);
                                    }

                                    break;
                                //long
                                case "Int64":
                                    {
                                        if (!long.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Int64>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!long.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Int32":
                                    {
                                        if (!int.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Int32>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!int.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Int16":
                                    {
                                        if (!short.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Int16>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!short.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的整数数值！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Decimal":
                                    {
                                        if (!decimal.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Decimal>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!decimal.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Double":
                                    {
                                        if (!double.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Double>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!double.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                //case "float":
                                case "Single":
                                    {
                                        if (!float.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "Nullable<Single>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!float.TryParse(cellValue, out var number))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的小数！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, number);
                                    }
                                    break;
                                case "DateTime":
                                    {
                                        if (!DateTime.TryParse(cellValue, out var date))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的日期时间格式！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, date);
                                    }
                                    break;
                                case "Nullable<DateTime>":
                                    {
                                        if (string.IsNullOrWhiteSpace(cellValue))
                                        {
                                            propertyInfo.SetValue(dataItem, null);
                                            break;
                                        }

                                        if (!DateTime.TryParse(cellValue, out var date))
                                        {
                                            AddRowDataError(rowIndex, col, $"值 {cellValue} 无效，请填写正确的日期时间格式！");
                                            break;
                                        }

                                        propertyInfo.SetValue(dataItem, date);
                                    }
                                    break;
                                default:
                                    propertyInfo.SetValue(dataItem, cell.Value);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            AddRowDataError(rowIndex, col, ex.Message);
                        }
                    }

                    ImportResult.Data.Add(dataItem);
                }
            }
        }

        /// <summary>
        ///     获取导入的Sheet
        /// </summary>
        /// <param name="excelPackage"></param>
        /// <returns></returns>
        protected virtual Worksheet GetImportSheet(Workbook excelPackage) => excelPackage.Worksheets[typeof(T).GetDisplayName()] ??
                   excelPackage.Worksheets[ExcelImporterAttribute.SheetName] ??
                   excelPackage.Worksheets[0];

        /// <summary>
        ///     添加数据行错误
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="importerHeaderInfo"></param>
        /// <param name="errorMessage"></param>
        protected virtual void AddRowDataError(int rowIndex, ImporterHeaderInfo importerHeaderInfo,
            string errorMessage = "数据格式无效！")
        {
            var rowError = ImportResult.RowErrors.FirstOrDefault(p => p.RowIndex == rowIndex);
            if (rowError == null)
            {
                rowError = new DataRowErrorInfo
                {
                    RowIndex = rowIndex
                };
                ImportResult.RowErrors.Add(rowError);
            }

            rowError.FieldErrors.Add(importerHeaderInfo.ExporterHeader.Name, errorMessage);
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool GetBooleanValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            switch (value.ToLower())
            {
                case "1":
                case "是":
                case "yes":
                case "true":
                    return true;
                case "0":
                case "否":
                case "no":
                case "false":
                default:
                    return false;
            }
        }

        /// <summary>
        ///     生成Excel导入模板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>二进制字节</returns>
        public Task<byte[]> GenerateTemplateByte()
        {
            ExcelImporterAttribute =
                typeof(T).GetAttribute<ExcelImporterAttribute>(true) ?? new ExcelImporterAttribute();
            using (var excelPackage = new Workbook())
            {
                StructureExcel(excelPackage);
                using (var stream = excelPackage.SaveToStream())
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    return Task.FromResult(buffer);
                }

            }
        }

        /// <summary>
        ///     生成Excel导入模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">文件名必须填写! - fileName</exception>
        public Task<TemplateFileInfo> GenerateTemplate(string fileName = null)
        {
            ExcelImporterAttribute =
                typeof(T).GetAttribute<ExcelImporterAttribute>(true) ?? new ExcelImporterAttribute();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("文件名必须填写!", fileName);
            }

            var fileInfo =
                ExcelHelper.CreateExcelPackage(fileName, excelPackage => { StructureExcel(excelPackage); });
            return Task.FromResult(fileInfo);
        }
    }
}
