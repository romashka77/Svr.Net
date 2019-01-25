using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Extensions
{
    public static class StringExtension
    {
        public static long? ToLong(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return null;
            }
            return Int64.Parse(str);
        }

    }
}
