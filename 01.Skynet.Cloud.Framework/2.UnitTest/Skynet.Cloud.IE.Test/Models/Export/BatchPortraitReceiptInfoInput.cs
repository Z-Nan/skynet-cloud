﻿// ======================================================================
// 
//           filename : BatchPortraitReceiptInfoInput.cs
//           description :
// 
//           created by magic.s.g.xie at  2019-11-25 17:09
//           
//           
//           
//           
// 
// ======================================================================

using System.Collections.Generic;
using DinkToPdf;
using UWay.Skynet.Cloud.IE.Pdf;

namespace UWay.Skynet.Cloud.IE.Tests.Models.Export
{
    /// <summary>
    ///     批量
    /// </summary>
    [PdfExporter(Orientation = Orientation.Portrait, PaperKind = PaperKind.A4)]
    public class BatchPortraitReceiptInfoInput
    {
        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Logo地址
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        ///     印章地址
        /// </summary>
        public string SealUrl { get; set; }

        /// <summary>
        ///     收款人
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        ///     电子收据输入参数
        /// </summary>
        public List<BatchPortraitReceiptInfoDto> ReceiptInfoInputs { get; set; }
    }
}