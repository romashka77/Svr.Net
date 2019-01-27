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
            try
            {
                return Int64.Parse(str);
            }
            catch (Exception)
            {

                return null;
            }
        }
        public static string ErrorFind(this string id)
        {
            return $"Ошибка: Не удалось найти ID = {id}.";
        }
    }
}
