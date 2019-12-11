﻿

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UWay.Skynet.Cloud.Storage.Core
{
    /// <summary>
    /// 程序提供程序
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 提供程序名称
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName">目录名称</param>
        /// <param name="blobName">文件对象名称</param>
        /// <param name="source">流</param>
        Task SaveBlobStream(string containerName, string blobName, Stream source);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="containerName">目录</param>
        /// <param name="blobName">文件对象名称</param>
        /// <returns></returns>
        Task<Stream> GetBlobStream(string containerName, string blobName);

        /// <summary>
        /// 获取文件链接
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<string> GetBlobUrl(string containerName, string blobName);

        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName);

        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<IList<BlobFileInfo>> ListBlobs(string containerName);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        Task DeleteBlob(string containerName, string blobName);

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        Task DeleteContainer(string containerName);

        /// <summary>
        /// 获取授权访问链接
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName">文件名称</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isDownload">是否允许下载</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="access">访问限制</param>
        /// <returns></returns>
        Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read);
    }
}