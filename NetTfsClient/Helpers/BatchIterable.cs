using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Helpers
{
    /// <summary>
    /// "batch" function that would take as input an iterable and return an iterable of iterables
    /// https://stackoverflow.com/questions/13731796/create-batches-in-linq/13731854
    /// </summary>
    internal static class BatchIterable
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source,
            int batchSize)
        {
            if (batchSize >= source.Count())
            {
                yield return source;
            }
            else
            {
                TSource[]? bucket = null;
                int count = 0;

                foreach (var item in source)
                {
                    if (bucket == null)
                    {
                        bucket = new TSource[batchSize];
                    }

                    bucket[count++] = item;

                    if (count != batchSize) continue;
                    yield return bucket;

                    bucket = null;
                    count = 0;
                }

                if ((bucket != null) && (count > 0))
                {
                    yield return bucket
                        .Take(count)
                        .ToArray();
                }
            }
        }
    }
}
