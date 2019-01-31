using System;
using System.Collections.Generic;

namespace Svr.Infrastructure.Extensions
{
    public class CodeComparer : IComparer<Object>
    {
        public int Compare(object x, object y)
        {
            var a = x.ToString().Split('.');
            var b = y.ToString().Split('.');
            for (int i = 0; (i < a.Length) && (i < b.Length); i++)
            {
                if (long.TryParse(a[i], out long c) && long.TryParse(b[i], out long d))
                {
                    if (c > d)
                        return 1;
                    else if (c < d)
                        return -1;
                }
                else
                {
                    if (long.TryParse(a[i], out c))
                        return 1;
                    else
                        return -1;
                }
            }
            if (a.Length>b.Length)
            {
                return 1;
            }
            if (a.Length < b.Length)
            {
                return -1;
            }
            return 0;
        }
    }
}
