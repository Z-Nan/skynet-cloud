// ======================================================================
// 
//           Copyright (C) 2019-2030 �����������Ƽ����޹�˾
//           All rights reserved
// 
//           filename : ExcelImporter_Tests.cs
//           description :
// 
//           created by magic.s.g.xie at  2019-09-11 13:51
//           
//           
//           
//          
// 
// ======================================================================

using UWay.Skynet.Cloud.IE.Core;
using UWay.Skynet.Cloud.IE.Core.Extension;
using UWay.Skynet.Cloud.IE.Core.Models;
using UWay.Skynet.Cloud.IE.Excel;
using UWay.Skynet.Cloud.IE.Tests.Models;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UWay.Skynet.Cloud.IE.Tests
{
    public class ExcelImporter_Tests
    {
        public IImporter Importer = new ExcelImporter();

        [Fact(DisplayName = "����ģ��")]
        public async Task GenerateTemplate_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), nameof(GenerateTemplate_Test) + ".xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var result = await Importer.GenerateTemplate<ImportProductDto>(filePath);
            result.ShouldNotBeNull();
            File.Exists(filePath).ShouldBeTrue();

            //TODO:��ͷ���ز���
        }

        [Fact(DisplayName = "����ģ���ֽ�")]
        public async Task GenerateTemplateBytes_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), nameof(GenerateTemplateBytes_Test) +".xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var result = await Importer.GenerateTemplateBytes<ImportProductDto>();
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            File.WriteAllBytes(filePath, result);
            File.Exists(filePath).ShouldBeTrue();
        }

        [Fact(DisplayName = "����")]
        public async Task Importer_Test()
        {
            //��һ������

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Import", "��Ʒ����ģ��.xlsx");
            var import = await Importer.Import<ImportProductDto>(filePath);
            import.ShouldNotBeNull();

            import.HasError.ShouldBeFalse();
            import.Data.ShouldNotBeNull();
            import.Data.Count.ShouldBeGreaterThanOrEqualTo(2);
            foreach (var item in import.Data)
            {
                if (item.Name.Contains("�ո����"))
                {
                    item.Name.ShouldBe(item.Name.Trim());
                }

                if (item.Code.Contains("��ȥ���ո����"))
                {
                    item.Code.ShouldContain(" ");
                }
                //ȥ���м�ո����
                item.BarCode.ShouldBe("123123");
            }

            //��Ϊ�����Ͳ���
            import.Data.ElementAt(4).Weight.HasValue.ShouldBe(true);
            import.Data.ElementAt(5).Weight.HasValue.ShouldBe(false);
            //��ȡ�Ա�ʽ����
            import.Data.ElementAt(0).Sex.ShouldBe("Ů");
            //��ȡ��ǰ�����Լ��������Ͳ���  ���ʱ�䲻�ԣ���򿪶�Ӧ��Excel���ɸ���Ϊ��ǰʱ�䣬Ȼ�������д˵�Ԫ����
            //import.Data[0].FormulaTest.Date.ShouldBe(DateTime.Now.Date);
            //��ֵ����
            import.Data.ElementAt(0).DeclareValue.ShouldBe(123123);
            import.Data.ElementAt(0).Name.ShouldBe("1212");
            import.Data.ElementAt(0).BarCode.ShouldBe("123123");
            import.Data.ElementAt(1).Name.ShouldBe("12312312");
            import.Data.ElementAt(2).Name.ShouldBe("���ո����");
        }

        [Fact(DisplayName = "��������")]
        public async Task IsRequired_Test()
        {
            var pros = typeof(ImportProductDto).GetProperties();
            foreach (var item in pros)
            {
                switch (item.Name)
                {
                    //DateTime
                    case "FormulaTest":
                    //int
                    case "DeclareValue":
                    //Required
                    case "Name":
                        item.IsRequired().ShouldBe(true);
                        break;
                    //��Ϊ������
                    case "Weight":
                    //string
                    case "IdNo":
                        item.IsRequired().ShouldBe(false);
                        break;
                }
            }
        }

        [Fact(DisplayName = "��⵼�����")]
        public async Task QuestionBankImporter_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Import", "��⵼��ģ��.xlsx");
            var import = await Importer.Import<ImportQuestionBankDto>(filePath);
            import.ShouldNotBeNull();

            import.HasError.ShouldBeFalse();
            import.Data.ShouldNotBeNull();
            import.Data.Count.ShouldBe(404);

            import.RowErrors.Count.ShouldBe(0);
            import.TemplateErrors.Count.ShouldBe(0);
        }

        [Fact(DisplayName = "�ɷ���ˮ�������")]
        public async Task ImportPaymentLogs_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Import", "�ɷ���ˮ����ģ��.xlsx");
            var import = await Importer.Import<ImportPaymentLogDto>(filePath);
            import.ShouldNotBeNull();
            import.HasError.ShouldBeTrue();
            import.Exception.ShouldBeNull();
            import.Data.Count.ShouldBe(20);
        }

        [Fact(DisplayName = "���ݴ�����")]
        public async Task RowDataError_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Errors", "���ݴ���.xlsx");
            var result = await Importer.Import<ImportRowDataErrorDto>(filePath);
            result.ShouldNotBeNull();
            result.HasError.ShouldBeTrue();

            result.TemplateErrors.Count.ShouldBe(0);

            result.RowErrors.ShouldContain(p => p.RowIndex == 2 && p.FieldErrors.ContainsKey("��Ʒ����"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 3 && p.FieldErrors.ContainsKey("��Ʒ����"));

            result.RowErrors.ShouldContain(p => p.RowIndex == 7 && p.FieldErrors.ContainsKey("��Ʒ����"));

            result.RowErrors.ShouldContain(p => p.RowIndex == 3 && p.FieldErrors.ContainsKey("����(KG)"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 4 && p.FieldErrors.ContainsKey("��ʽ����"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 5 && p.FieldErrors.ContainsKey("��ʽ����"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 6 && p.FieldErrors.ContainsKey("��ʽ����"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 7 && p.FieldErrors.ContainsKey("��ʽ����"));

            result.RowErrors.ShouldContain(p => p.RowIndex == 3 && p.FieldErrors.ContainsKey("���֤"));
            result.RowErrors.First(p => p.RowIndex == 3 && p.FieldErrors.ContainsKey("���֤")).FieldErrors.Count
                .ShouldBe(3);

            result.RowErrors.ShouldContain(p => p.RowIndex == 4 && p.FieldErrors.ContainsKey("���֤"));
            result.RowErrors.ShouldContain(p => p.RowIndex == 5 && p.FieldErrors.ContainsKey("���֤"));

            #region �ظ�����

            var errorRows = "5,6".Split(',').ToList();
            result.RowErrors.ShouldContain(p =>
                errorRows.Contains(p.RowIndex.ToString()) && p.FieldErrors.ContainsKey("��Ʒ����") &&
                p.FieldErrors.Values.Contains("���������ظ������飡�����У�5��6��"));

            errorRows = "8,9,11,13".Split(',').ToList();
            result.RowErrors.ShouldContain(p =>
                errorRows.Contains(p.RowIndex.ToString()) && p.FieldErrors.ContainsKey("��Ʒ����") &&
                p.FieldErrors.Values.Contains("���������ظ������飡�����У�8��9��11��13��"));

            errorRows = "4��6��8��10��11��13".Split('��').ToList();
            result.RowErrors.ShouldContain(p =>
                errorRows.Contains(p.RowIndex.ToString()) && p.FieldErrors.ContainsKey("��Ʒ�ͺ�") &&
                p.FieldErrors.Values.Contains("���������ظ������飡�����У�4��6��8��10��11��13��"));
            #endregion

            result.RowErrors.Count.ShouldBeGreaterThan(0);

            //һ�н��������һ������
            foreach (var item in result.RowErrors.GroupBy(p => p.RowIndex).Select(p => new { p.Key, Count = p.Count() }))
            {
                item.Count.ShouldBe(1);
            }
        }

        [Fact(DisplayName = "ģ�������")]
        public async Task TplError_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Errors", "ģ���ֶδ���.xlsx");
            var result = await Importer.Import<ImportProductDto>(filePath);
            result.ShouldNotBeNull();
            result.HasError.ShouldBeTrue();
            result.TemplateErrors.Count.ShouldBeGreaterThan(0);
            result.TemplateErrors.Count(p => p.ErrorLevel == ErrorLevels.Error).ShouldBe(1);
            result.TemplateErrors.Count(p => p.ErrorLevel == ErrorLevels.Warning).ShouldBe(1);
        }

        [Fact(DisplayName = "�ض����ݲ���")]
        public async Task ImporterDataEnd_Test()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "Import", "�ض����ݲ���.xlsx");
            var import = await Importer.Import<ImportProductDto>(filePath);
            import.ShouldNotBeNull();
            import.Data.ShouldNotBeNull();
            import.Data.Count.ShouldBe(6);
        }
    }
}