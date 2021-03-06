﻿namespace UWay.Skynet.Cloud.Nacos
{
    using System;
    using System.Collections.Generic;

    public class ServerAddressManager 
    {
        private string _server;

        private readonly List<string> _servers = new List<string>();

        private int _offset = 0;

        public ServerAddressManager(NacosClientConfiguration options)
        {
            var serverAddresses = options.ServerAddresses;
            Init(serverAddresses);


        }

        public ServerAddressManager(NacosDiscoveryOptions options)
        {
            var serverAddresses = options.Host;
            Init(serverAddresses);
        }

        private void Init(string serverAddresses)
        {
            if (string.IsNullOrWhiteSpace(serverAddresses)) throw new ArgumentNullException();

            var hostAndPorts = serverAddresses.Split(',');

            foreach (var item in hostAndPorts)
            {
                var hostAndPort = string.Empty;

                var tmp = item.Split(':');

                if (tmp.Length == 2)
                {
                    hostAndPort = item;
                }
                else if (tmp.Length == 1)
                {
                    hostAndPort = $"{tmp[0]}:8848";
                }
                else
                {
                    throw new ArgumentException();
                }

                _servers.Add(hostAndPort);
            }

            if (_servers.Count <= 0) throw new Exceptions.NacosException("can not find out UWay.Skynet.Cloud.Nacos server");

            _server = _servers[0];
        }

        public void ChangeServer()
        {
            _offset = (_offset + 1) % _servers.Count;
            _server = _servers[_offset];
        }

        public string GetCurrentServer()
        {
            return _server;                 
        }


        public List<string> GetServers()
        {
            return _servers;
        }

    }
}
