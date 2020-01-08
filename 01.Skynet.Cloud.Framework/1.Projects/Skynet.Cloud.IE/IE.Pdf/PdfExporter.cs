﻿// ======================================================================
// 
//           filename : PdfExporter.cs
//           description :
// 
//           created by magic.s.g.xie at  2019-09-26 14:59
//           
//          
//           
//           
// 
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DinkToPdf;
using UWay.Skynet.Cloud.IE.Core;
using UWay.Skynet.Cloud.IE.Core.Models;
using UWay.Skynet.Cloud.IE.Html;

namespace UWay.Skynet.Cloud.IE.Pdf
{
    /// <summary>
    ///     Pdf导出逻辑
    /// </summary>
    public class PdfExporter : IExportListFileByTemplate, IExportFileByTemplate
    {
        private static readonly SynchronizedConverter PdfConverter = new SynchronizedConverter(new PdfTools());

        /// <summary>
        ///     根据模板导出列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="dataItems"></param>
        /// <param name="htmlTemplate"></param>
        /// <returns></returns>
        public async Task<ExportFileInfo> ExportListByTemplate<T>(string fileName, ICollection<T> dataItems,
            string htmlTemplate = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("文件名必须填写!", nameof(fileName));

            var exporterAttribute = GetExporterAttribute<T>();
            var exporter = new HtmlExporter();
            var htmlString = await exporter.ExportListByTemplate(dataItems, htmlTemplate);

            if (exporterAttribute.IsWriteHtml)
                File.WriteAllText(fileName + ".html", htmlString);

            var doc = GetHtmlToPdfDocumentByExporterAttribute(fileName, exporterAttribute, htmlString);

            PdfConverter.Convert(doc);
            var fileInfo = new ExportFileInfo(fileName, "application/pdf");
            return fileInfo;
        }

        /// <summary>
        ///     根据模板导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="htmlTemplate"></param>
        /// <returns></returns>
        public async Task<ExportFileInfo> ExportByTemplate<T>(string fileName, T data, string htmlTemplate)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("文件名必须填写!", nameof(fileName));

            var exporterAttribute = GetExporterAttribute<T>();
            var exporter = new HtmlExporter();
            var htmlString = await exporter.ExportByTemplate(data, htmlTemplate);

            if (exporterAttribute.IsWriteHtml)
                File.WriteAllText(fileName + ".html", htmlString);

            var doc = GetHtmlToPdfDocumentByExporterAttribute(fileName, exporterAttribute, htmlString);
            PdfConverter.Convert(doc);
            var fileInfo = new ExportFileInfo(fileName, "application/pdf");
            return fileInfo;
        }

        /// <summary>
        ///     获取文档转换配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pdfExporterAttribute"></param>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        private HtmlToPdfDocument GetHtmlToPdfDocumentByExporterAttribute(string fileName,
            PdfExporterAttribute pdfExporterAttribute,
            string htmlString)
        {
            var htmlToPdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = pdfExporterAttribute.Orientation,
                    PaperSize = pdfExporterAttribute.PaperKind,
                    Out = fileName,
                    DocumentTitle = pdfExporterAttribute.Name
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        PagesCount = pdfExporterAttribute.IsEnablePagesCount,
                        HtmlContent = htmlString,
                        WebSettings = {DefaultEncoding = pdfExporterAttribute.Encoding.BodyName},
                        Encoding = pdfExporterAttribute.Encoding,
                        HeaderSettings = pdfExporterAttribute.HeaderSettings,
                        FooterSettings = pdfExporterAttribute.FooterSettings
                    }
                }
            };
            return htmlToPdfDocument;
        }


        /// <summary>
        ///     获取全局导出定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static PdfExporterAttribute GetExporterAttribute<T>() where T : class
        {
            var exporterTableAttributes =
                typeof(T).GetCustomAttributes(typeof(PdfExporterAttribute), true) as PdfExporterAttribute[];
            if (exporterTableAttributes != null && exporterTableAttributes.Length > 0)
                return exporterTableAttributes[0];

            var exporterAttributes =
                typeof(T).GetCustomAttributes(typeof(ExporterAttribute), true) as ExporterAttribute[];

            if (exporterAttributes == null || exporterAttributes.Length <= 0) return null;

            var export = exporterAttributes[0];
            return new PdfExporterAttribute
            {
                FontSize = export.FontSize,
                HeaderFontSize = export.HeaderFontSize
            };
        }
    }
}