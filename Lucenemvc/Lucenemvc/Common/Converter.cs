using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Lucenemvc.Common
{
    /// <summary>
    /// 数据类型转换器
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 对系统Convert.ChangeType方法进行扩展（其可以对可空值类型进行转换）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        /// <remarks>当使用系统Convert.ChangeType方法进行可空值类型进行转换的时候，会抛出异常。本扩展解决了这个问题。</remarks>
        public static object ChangeType(object data, Type conversionType)
        {
            return ChangeType(data, conversionType, null);
        }

        /// <summary>
        /// 对系统Convert.ChangeType方法进行扩展（其可以对可空值类型进行转换）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="conversionType"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <remarks>当使用系统Convert.ChangeType方法进行可空值类型进行转换的时候，会抛出异常。本扩展解决了这个问题。</remarks>
        public static object ChangeType(object data, Type conversionType, object defaultValue)
        {
            object result = defaultValue;

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (data == null)
                {
                    return null;
                }

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            try
            {
                if (defaultValue == null && (data == null || data.ToString().Trim() == string.Empty))
                {
                    if (TypeHelper.ConfirmIsNumberType(conversionType))
                    {
                        data = 0;
                    }

                    if (conversionType == typeof(DateTime))
                    {
                        data = DateTimeHelper.Min;
                    }
                }

                result = Convert.ChangeType(data, conversionType);
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// 对系统Convert.ChangeType方法进行扩展（其可以对可空值类型进行转换,并且使用泛型的方式调用更方便）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T ChangeType<T>(object data)
        {
            return ChangeType<T>(data, default(T));
        }

        /// <summary>
        /// 对系统Convert.ChangeType方法进行扩展（其可以对可空值类型进行转换,并且使用泛型的方式调用更方便）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static T ChangeType<T>(object data, T defaultValue)
        {
            T result;
            try
            {
                //Guid是一个非常特殊的类型，需要单独转换
                if (typeof(T) == typeof(Guid))
                {
                    result = (T)(object)GuidHelper.TryConvert(data.ToString(), (Guid)(object)defaultValue);
                }
                else
                {
                    result = (T)ChangeType(data, typeof(T), defaultValue);
                }
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

       

        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="data">要转换的值,即原值</param>
        /// <param name="fromBase">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="toBase">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(string data, int fromBase, int toBase)
        {
            try
            {
                int intValue = Convert.ToInt32(data, fromBase);  //先转成10进制
                string result = Convert.ToString(intValue, toBase);  //再转成目标进制
                return result;
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ToInt32(bool data)
        {
            return data ? 1 : 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ToBoolean(int data)
        {
            return data > 0;
        }

        

        /// <summary>
        /// 将布尔值转换为中文显示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToChineseString(bool data)
        {
            return ToChineseString(data, string.Empty);
        }

        /// <summary>
        /// 将布尔值转换为中文显示
        /// </summary>
        /// <param name="data"></param>
        /// <param name="displaySerialName"></param>
        /// <returns></returns>
        public static string ToChineseString(bool data, string displaySerialName)
        {
            string result;
            if (displaySerialName == null)
            {
                displaySerialName = string.Empty;
            }
            displaySerialName = displaySerialName.ToLower();

            switch (displaySerialName)
            {
                case "effect":
                    result = data ? "有效" : "无效";
                    break;
                default:
                    result = data ? "是" : "否";
                    break;
            }

            return result;
        }

        /// <summary>
        /// 将某个枚举项的字符串值转化成其对应的枚举类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumItemValue"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(string enumItemValue) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), enumItemValue, true);
        }

        /// <summary>
        /// 将某个枚举项的字符串值转化成其对应的枚举类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumItemValue"></param>
        /// <returns></returns>
        public static TEnum TryToEnum<TEnum>(string enumItemValue) where TEnum : struct
        {
            TEnum defaultValue = default(TEnum);
            return TryToEnum<TEnum>(enumItemValue, defaultValue);
        }

        /// <summary>
        /// 将某个枚举项的字符串值转化成其对应的枚举类型
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumItemValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TEnum TryToEnum<TEnum>(string enumItemValue, TEnum defaultValue) where TEnum : struct
        {
            TEnum result = defaultValue;
            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), enumItemValue, true);
            }
            catch
            {
                //Do Nothing;
            }

            return result;
        }

        /// <summary>
        /// 将某个枚举项的字符串值转化成其对应的枚举类型
        /// </summary>
        /// <param name="enumType"> </param>
        /// <param name="enumItemValue"></param>
        /// <returns></returns>
        public static object TryToEnum(Type enumType, string enumItemValue)
        {
            object result = null;
            try
            {
                result = Enum.Parse(enumType, enumItemValue, true);
            }
            catch
            {
                //Do Nothing;
            }

            return result;
        }

        /// <summary>
        /// 将字符串类型的bool值转换成bool类型
        /// </summary>
        /// <param name="data">字符串类型的bool值</param>
        /// <returns></returns>
        public static bool TryToBoolean(string data)
        {
            return TryToBoolean(data, false);
        }

        /// <summary>
        /// 将字符串类型的bool值转换成bool类型
        /// </summary>
        /// <param name="data">字符串类型的bool值</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static bool TryToBoolean(string data, bool defaultValue)
        {
            bool result;
            return bool.TryParse(data, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串类型的数字转换成数字类型
        /// </summary>
        /// <param name="data">字符串类型的数字</param>
        /// <returns></returns>
        public static int TryToInt32(string data)
        {
            return TryToInt32(data, 0);
        }

        /// <summary>
        /// 将字符串类型的数字转换成数字类型
        /// </summary>
        /// <param name="data">字符串类型的数字</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static int TryToInt32(string data, int defaultValue)
        {
            int result;
            return int.TryParse(data, out result) ? result : defaultValue;
        }


        /// <summary>
        /// 将字符串类型的Decimal转换成Decimal类型
        /// </summary>
        /// <param name="data">字符串类型的Decimal</param>
        /// <returns></returns>
        public static decimal TryToDecimal(string data)
        {
            return TryToDecimal(data, 0);
        }

        /// <summary>
        /// 将字符串类型的Decimal转换成Decimal类型
        /// </summary>
        /// <param name="data">字符串类型的Decimal</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static decimal TryToDecimal(string data, decimal defaultValue)
        {
            decimal result;
            return decimal.TryParse(data, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串类型的Double转换成Double类型
        /// </summary>
        /// <param name="data">字符串类型的Double</param>
        /// <returns></returns>
        public static double TryToDouble(string data)
        {
            return TryToDouble(data, 0);
        }

        /// <summary>
        /// 将字符串类型的Double转换成Double类型
        /// </summary>
        /// <param name="data">字符串类型的Double</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static double TryToDouble(string data, double defaultValue)
        {
            double result;
            return double.TryParse(data, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串类型的Single转换成Single类型
        /// </summary>
        /// <param name="data">字符串类型的Single</param>
        /// <returns></returns>
        public static float TryToSingle(string data)
        {
            return TryToSingle(data, 0);
        }

        /// <summary>
        /// 将字符串类型的Single转换成Single类型
        /// </summary>
        /// <param name="data">字符串类型的Single</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static float TryToSingle(string data, float defaultValue)
        {
            float result;
            return float.TryParse(data, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串类型的Long转换成Long类型
        /// </summary>
        /// <param name="data">字符串类型的Long</param>
        /// <returns></returns>
        public static long TryToLong(string data)
        {
            return TryToLong(data, 0);
        }

        /// <summary>
        /// 将字符串类型的Long转换成Long类型
        /// </summary>
        /// <param name="data">字符串类型的Long</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns></returns>
        public static long TryToLong(string data, long defaultValue)
        {
            long result;
            return long.TryParse(data, out result) ? result : defaultValue;
        }


        /// <summary>
        /// 将字符串转换成Guid
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Guid TryToGuid(string data)
        {
            return GuidHelper.TryConvert(data);
        }
    }
}