﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lucenemvc.Common
{
    /// <summary>
    /// 类型操作辅助类
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// 根据类型描述通过反射技术获取指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeDescription"></param>
        /// <returns></returns>
        public static T Activate<T>(string typeDescription)
        {
            Type targetClassType = Type.GetType(typeDescription);
            return Activate<T>(targetClassType);
        }

        /// <summary>
        /// 根据类型描述通过反射技术获取指定类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Activate<T>(Type type)
        {
            T target = default(T);

            try
            {
                target = (T)Activator.CreateInstance(type);
            }
            catch
            {
                //do nothing;
            }

            return target;
        }

       

        /// <summary>
        /// 判断给定的类型是否为数字类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ConfirmIsNumberType<T>()
        {
            return ConfirmIsNumberType(typeof(T));
        }

        /// <summary>
        /// 判断给定的类型是否为数字类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ConfirmIsNumberType(Type type)
        {
            if (type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(Double)
                || type == typeof(Single)
                || type == typeof(Decimal)
                || type.IsEnum)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断给定的类型是否为字符串类型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ConfirmIsStringType(Type type)
        {
            if (type == typeof(String)
                || type == typeof(Char)
                || type == typeof(Guid)
                || type == typeof(DateTime))
            {
                return true;
            }
            return false;
        }
    }
}