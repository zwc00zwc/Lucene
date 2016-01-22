using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lucenemvc.Common
{
    /// <summary>
    /// 日期操作辅助类
    /// </summary>
    public static class DateTimeHelper
    {
        public static DateTime Min
        {
            get
            {
                return new DateTime(1753, 1, 1);
            }
        }
    }
}