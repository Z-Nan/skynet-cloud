﻿using Microsoft.Extensions.Configuration;
using Steeltoe.Discovery.Nacos.Discovery;
using System;
using System.Collections.Generic;
using System.Text;
using UWay.Skynet.Cloud.Nacos;

namespace Steeltoe.Discovery.Nacos.Registry
{
    public class NacosRegistration : INacosRegistration
    {
        private NacosRegistration(string serviceId
            , string host
            , string cluster
            , bool enabled
            , bool ephemeral
            , string groupName
            , bool healthy
            , bool isSecure
            , string namespaceInfo
            , int port )
        {

            this.ServiceId = serviceId;
            this.ClusterName = cluster;
            this.Enable = enabled;
            this.Host = host;
            this.Ephemeral = ephemeral;
            this.GroupName = groupName;
            this.Healthy = healthy;
            this.Port = port;
            this.IsSecure = isSecure;
            this.Namespace = namespaceInfo;
        }

        public string ClusterName { get; protected set; }

        public bool Ephemeral { get; protected set; }

        public bool Enable { get; protected set; }

        public bool Healthy { get; protected set; }

        public string GroupName { get; protected set; }

        public string NamespaceId { get; protected set; }

        public string ServiceId { get; protected set; }

        public string Host { get; protected set; }

        public int Port { get; protected set; }

        public bool IsSecure { get; protected set; }

        public Uri Uri {
            get
            {
                if(IsSecure)
                    return new Uri($"https://{Host}:{Port}");
                else
                    return new Uri($"http://{Host}:{Port}");
            }
        }

        //public IDictionary<string, string> Metadata => throw new NotImplementedException();

        

        public string Namespace { set; get; }

        public IDictionary<string, string> Metadata { set; get; }

        public static NacosRegistration CreateRegistration(IConfiguration config, NacosDiscoveryOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return new NacosRegistration(GetAppName(options, config), options.IpAddress, options.Cluster, options.Enabled, GetEphemeral(options, config), options.GroupName, GetHealthy(options, config),  options.IsSecure, options.Namespace, options.Port);

        }

        internal static bool  GetEphemeral(NacosDiscoveryOptions options, IConfiguration config)
        {
            bool? ephemeral = options.Ephemeral;
            if (!ephemeral.HasValue)
            {
                return true;
            }


            return ephemeral.Value;

        }

        internal static bool GetHealthy(NacosDiscoveryOptions options, IConfiguration config)
        {
            bool? ephemeral = options.Healthy;
            if (!ephemeral.HasValue)
            {
                return true;
            }


            return ephemeral.Value;

        }

        internal static string GetAppName(NacosDiscoveryOptions options, IConfiguration config)
        {
            string appName = options.ServiceName;
            if (!string.IsNullOrEmpty(appName))
            {
                return appName;
            }

            return config.GetValue("spring:application:name", "application");
        }

    }

  
}
