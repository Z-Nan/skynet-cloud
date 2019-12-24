// ======================================================================
// 
//           Copyright (C) 2019-2030 �����������Ƽ����޹�˾
//           All rights reserved
// 
//           filename : ExcelExporter_Tests.cs
//           description :
// 
//           created by magic.s.g.xie at  2019-09-11 13:51
//           
//           
//           
//           Blog��http://www.cnblogs.com/codelove/
// 
// ======================================================================

using UWay.Skynet.Cloud.IE.Core;
using UWay.Skynet.Cloud.IE.Excel;
using UWay.Skynet.Cloud.IE.Excel.Builder;
using UWay.Skynet.Cloud.IE.Tests.Models;
using Shouldly;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UWay.Skynet.Cloud.IE.Tests
{
    public class ExcelExporter_Tests : TestBase
    {
        [Fact(DisplayName = "���Ե���")]
        public async Task AttrsExport_Test()
        {
            IExporter exporter = new ExcelExporter();

            var filePath = GetTestFilePath($"{nameof(AttrsExport_Test)}.xlsx");

            DeleteFile(filePath);

            var result = await exporter.Export(filePath,
                GenFu.GenFu.ListOf<ExportTestDataWithAttrs>());
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "ExportAsByteArray_Test")]
        public async Task ExportAsByteArray_Test()
        {
            IExporter exporter = new ExcelExporter();

            var filePath = GetTestFilePath($"{nameof(ExportAsByteArray_Test)}.xlsx");

            DeleteFile(filePath);

            var result = await exporter.ExportAsByteArray(GenFu.GenFu.ListOf<ExportTestDataWithAttrs>());
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            await File.WriteAllBytesAsync(filePath, result);
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "ExportHeaderAsByteArrayWithItems_Test")]
        public async Task ExportHeaderAsByteArrayWithItems_Test()
        {
            IExporter exporter = new ExcelExporter();

            var filePath = GetTestFilePath($"{nameof(ExportHeaderAsByteArrayWithItems_Test)}.xlsx");

            DeleteFile(filePath);

            var result = await exporter.ExportHeaderAsByteArray(new string[] { "Name1", "Name2", "Name3", "Name4", "Name5", "Name6", }, "Test");
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            await File.WriteAllBytesAsync(filePath, result);
            File.Exists(filePath).ShouldBeTrue();
            //TODO:Excel��ȡ����֤
        }

        [Fact(DisplayName = "ExportHeaderAsByteArray_Test")]
        public async Task ExportHeaderAsByteArray_Test()
        {
            IExporter exporter = new ExcelExporter();

            var filePath = GetTestFilePath($"{nameof(ExportHeaderAsByteArray_Test)}.xlsx");

            DeleteFile(filePath);

            var result = await exporter.ExportHeaderAsByteArray<ExportTestDataWithAttrs>(GenFu.GenFu.New<ExportTestDataWithAttrs>());
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            await File.WriteAllBytesAsync(filePath, result);
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "���������Ե���")]
        public async Task AttrsLocalizationExport_Test()
        {
            IExporter exporter = new ExcelExporter();
            ExcelBuilder.Create().WithColumnHeaderStringFunc(key =>
            {
                if (key.Contains("�ı�"))
                {
                    return "Text";
                }

                return "δ֪����";
            }).Build();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "testAttrsLocalization.xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var data = GenFu.GenFu.ListOf<AttrsLocalizationTestData>();
            var result = await exporter.Export(filePath, data);
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "�������ݵ���Excel")]
        public async Task Export_Test()
        {
            IExporter exporter = new ExcelExporter();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), nameof(Export_Test) + ".xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var result = await exporter.Export(filePath, GenFu.GenFu.ListOf<ExportTestData>(100000));
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "��̬�е���Excel")]
        public async Task DynamicExport_Test()
        {
            IExporter exporter = new ExcelExporter();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), nameof(DynamicExport_Test) + ".xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            List<ExportTestDataWithAttrs> exportDatas = GenFu.GenFu.ListOf<ExportTestDataWithAttrs>(1000);

            DataTable dt = new DataTable();
            //2.����������������������(���ַ�ʽ��ѡ��һ)
            dt.Columns.Add("Text", System.Type.GetType("System.String"));
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            dt.Columns.Add("Number", System.Type.GetType("System.Decimal"));
            dt = EntityToDataTable<ExportTestDataWithAttrs>(dt, exportDatas);

            var result = await exporter.Export<ExportTestDataWithAttrs>(filePath,dt);
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();
        }   

        /// <summary>
        /// ��entitiesֱ��ת��DataTable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entities">entity����</param>
        /// <returns>��Entity��ֵתΪDataTable</returns>
        private static DataTable EntityToDataTable<T>(DataTable dt, IEnumerable<T> entities)
        {
            if (entities.Count() == 0)
            {
                return dt;
            }

            var properties = typeof(T).GetProperties();

            foreach (var entity in entities)
            {
                var dr = dt.NewRow();

                foreach (var property in properties)
                {
                    if (dt.Columns.Contains(property.Name))
                    {
                        dr[property.Name] = property.GetValue(entity, null);
                    }
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}