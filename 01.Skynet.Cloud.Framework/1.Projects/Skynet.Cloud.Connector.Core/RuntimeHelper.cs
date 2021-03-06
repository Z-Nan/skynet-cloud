﻿namespace UWay.Skynet.Cloud.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.Extensions.DependencyModel;

    /// <summary>
    /// 运行时帮助类.
    /// </summary>
    public class RuntimeHelper
    {
        /// <summary>
        /// 获取项目程序集，排除所有的系统程序集(Microsoft.***、System.***等)、Nuget下载包.
        /// </summary>
        /// <returns>IList.</returns>
        public static IList<Assembly> GetAllAssemblies()
        {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;

            // 排除所有的系统程序集、Nuget下载包.
            foreach (var lib in deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package"))
            {
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    list.Add(assembly);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return list;
        }

        /// <summary>
        /// 获取Assmbly
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <returns>Assembly</returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            return GetAllAssemblies().FirstOrDefault(assembly => assembly.FullName.Contains(assemblyName));
        }

        /// <summary>
        /// 获取Service.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public static IEnumerable<Assembly> GetServicesAssembly()
        {
            return GetAllAssemblies().Where(assembly => assembly.FullName.Match("((?:[a-z][a-z0-9_]*))(\\.)(Service)(,)") || assembly.FullName.Match("((?:[a-z][a-z0-9_]*))(\\.)(Services)(,)"));
        }

        /// <summary>
        /// 获取所有类型.
        /// </summary>
        /// <returns>IList.</returns>
        public static IList<Type> GetAllTypes()
        {
            var list = new List<Type>();
            foreach (var assembly in GetAllAssemblies())
            {
                var typeInfos = assembly.DefinedTypes;
                foreach (var typeInfo in typeInfos)
                {
                    list.Add(typeInfo.AsType());
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有类型
        /// </summary>
        /// <param name="assemblyName">Assembly.</param>
        /// <returns>IList.</returns>
        public static IList<Type> GetTypesByAssembly(string assemblyName)
        {
            var list = new List<Type>();
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            var typeInfos = assembly.DefinedTypes;
            foreach (var typeInfo in typeInfos)
            {
                list.Add(typeInfo.AsType());
            }
            return list;
        }

        /// <summary>
        /// 获取实现类.
        /// </summary>
        /// <param name="typeName">类型名称.</param>
        /// <param name="baseInterfaceType">基础类型.</param>
        public static Type GetImplementType(string typeName, Type baseInterfaceType)
        {
            return GetAllTypes().FirstOrDefault(t =>
            {
                if (t.Name == typeName &&
                    t.GetTypeInfo().GetInterfaces().Any(b => b.Name == baseInterfaceType.Name))
                {
                    var typeInfo = t.GetTypeInfo();
                    return typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType;
                }

                return false;
            });
        }
    }
}
