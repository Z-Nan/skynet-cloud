﻿// ======================================================================
// 
//           filename : TypeExtensions.cs
//           description :
// 
//           created by magic.s.g.xie at  2019-09-11 16:53
// 
// ======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UWay.Skynet.Cloud.IE.Core.Extension
{
    /// <summary>
    ///     Defines the <see cref="TypeExtensions" />
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     获取显示名
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string GetDisplayName(this ICustomAttributeProvider customAttributeProvider, bool inherit = false)
        {
            string displayName = null;
            var displayAttribute = customAttributeProvider.GetAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                displayName = displayAttribute.Name;
            }
            else
            {
                var displayNameAttribute = customAttributeProvider.GetAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null)
                    displayName = displayNameAttribute.DisplayName;
            }

            return displayName;
        }

        /// <summary>
        ///     获取类型描述
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string GetDescription(this ICustomAttributeProvider customAttributeProvider, bool inherit = false)
        {
            var des = string.Empty;
            var desAttribute = customAttributeProvider.GetAttribute<DescriptionAttribute>();
            if (desAttribute != null) des = desAttribute.Description;
            return des;
        }

        /// <summary>
        ///     获取类型描述或显示名
        /// </summary>
        /// <param name="customAttributeProvider"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static string GetTypeDisplayOrDescription(this ICustomAttributeProvider customAttributeProvider,
            bool inherit = false)
        {
            var dispaly = customAttributeProvider.GetDescription(inherit);
            if (dispaly.IsNullOrWhiteSpace()) dispaly = customAttributeProvider.GetDisplayName(inherit);
            return dispaly ?? string.Empty;
        }


        /// <summary>
        ///     获取程序集属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider assembly, bool inherit = false)
            where T : Attribute
        {
            return assembly
                .GetCustomAttributes(typeof(T), inherit)
                .OfType<T>()
                .FirstOrDefault();
        }

        /// <summary>
        ///     获取程序集属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider assembly, bool inherit = false)
            where T : Attribute
        {
            return assembly
                .GetCustomAttributes(typeof(T), inherit)
                .OfType<T>();
        }

        /// <summary>
        ///     检查指定指定类型成员中是否存在指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">要检查的Attribute特性类型</typeparam>
        /// <param name="assembly">The assembly<see cref="ICustomAttributeProvider" /></param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>是否存在</returns>
        public static bool AttributeExists<T>(this ICustomAttributeProvider assembly, bool inherit = false)
            where T : Attribute
        {
            return assembly.GetCustomAttributes(typeof(T), inherit).Any(m => m as T != null);
        }

        /// <summary>
        ///     是否必填
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static bool IsRequired(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetAttribute<RequiredAttribute>(true) != null) return true;
            //Boolean、Byte、SByte、Int16、UInt16、Int32、UInt32、Int64、UInt64、Char、Double、Single
            if (propertyInfo.PropertyType.IsPrimitive) return true;
            switch (propertyInfo.PropertyType.Name)
            {
                case "DateTime":
                case "Decimal":
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     获取当前程序集中应用此特性的类
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="inherit">The inherit<see cref="bool" /></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypesWith<TAttribute>(this Assembly assembly, bool inherit)
            where TAttribute : Attribute
        {
            var attrType = typeof(TAttribute);
            foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttributes(attrType, true).Length > 0))
                yield return type;
        }

        /// <summary>
        ///     获取枚举定义列表
        /// </summary>
        /// <returns>返回枚举列表元组（名称、值、显示名、描述）</returns>
        public static List<Tuple<string, int, string, string>> GetEnumDefinitionList(this Type type)
        {
            var list = new List<Tuple<string, int, string, string>>();
            var attrType = type;
            if (!attrType.IsEnum) return null;
            var names = Enum.GetNames(attrType);
            var values = Enum.GetValues(attrType);
            var index = 0;
            foreach (var value in values)
            {
                var name = names[index];
                var field = value.GetType().GetField(value.ToString());
                var displayName = field.GetDisplayName();
                var des = field.GetDescription();
                var item = new Tuple<string, int, string, string>(
                    name,
                    Convert.ToInt32(value),
                    displayName.IsNullOrWhiteSpace() ? null : displayName,
                    des.IsNullOrWhiteSpace() ? null : des
                );
                list.Add(item);
                index++;
            }

            return list;
        }

        /// <summary>
        ///     是否为可为空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        ///     获取可为空类型的底层类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type);
        }


        /// <summary>
        ///     获取枚举列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        ///     key :返回显示名称或者描述
        ///     value：值
        /// </returns>
        public static IDictionary<string, int> GetEnumTextAndValues(this Type type)
        {
            if (!type.IsEnum) throw new InvalidOperationException();
            var items = type.GetEnumDefinitionList();
            var dic = new Dictionary<string, int>();
            //枚举名 值 显示名称 描述
            foreach (var tuple in items)
            {
                //如果描述、显示名不存在，则返回枚举名称
                dic.Add(tuple.Item4 ?? tuple.Item3 ?? tuple.Item1, tuple.Item2);
            }
            return dic;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCSharpTypeName(this Type type)
        {
            var sb = new StringBuilder();
            var name = type.Name;
            if (!type.IsGenericType) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                .Select(t => t.GetCSharpTypeName())));
            sb.Append(">");
            return sb.ToString();
        }
    }
}