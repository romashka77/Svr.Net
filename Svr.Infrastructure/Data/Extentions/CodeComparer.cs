using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                    {
                        return 1;
                    }
                    else if (c < d) return -1;
                }
                else
                {
                    break;
                }
            }
            return 0;
        }
    }
}
