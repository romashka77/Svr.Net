using System;

namespace Svr.Web.Extensions
{
    public static class StringExtension
    {
        public static long? ToLong(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            try
            {
                return long.Parse(str);
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
