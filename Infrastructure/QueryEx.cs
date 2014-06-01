using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPatterns.ObjectRelational;

namespace TestLab.Infrastructure
{
    public static class QueryEx
    {
        public static IEnumerable<T> Actives<T>(this IEnumerable<T> query) where T : IArchivable
        {
            return query.Where(z => z.Deleted == null);
        }

        public static IEnumerable<T> Archives<T>(this IEnumerable<T> query) where T : IArchivable
        {
            return query.Where(z => z.Deleted != null);
        }
    }
}
