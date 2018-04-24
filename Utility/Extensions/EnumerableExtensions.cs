using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Extensions
{
    public static class EnumerableExtensions
    {
        public static TItem WithMaximal<TItem,TMetric>(this IEnumerable<TItem> source, Func<TItem, TMetric> predicate) where TMetric : IComparable
        {
            var result = default(TItem);
            var resultMetric = default(TMetric);
            foreach (var item in source)
            {
                var metric = predicate(item);
                if (metric.CompareTo(resultMetric) > 0)
                {
                    resultMetric = metric;
                    result = item;
                }
            }
            return result;
        }
    }
}
