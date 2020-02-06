using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleGoogleWebServices.ConsoleApp
{
    internal static class BatchHelpers
    {
        internal static async Task<IEnumerable<UResult>> BatchInvokeAsync<TQuery, UResult>(IEnumerable<TQuery> queries,
                                                                                           Func<TQuery, CancellationToken, Task<UResult>> func,
                                                                                           CancellationToken cancellationToken)
        {
            if (queries is null)
            {
                throw new ArgumentNullException(nameof(queries));
            }

            // Batch this up.
            var batches = queries.Batch(20).ToList();

            var results = new List<UResult>();

            foreach (var batch in batches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var batchedList = batch.ToList();

                var tasks = new List<Task<UResult>>();

                foreach (var query in batchedList)
                {
                    var task = func.Invoke(query, cancellationToken);

                    tasks.Add(task);
                }

                var autocompleteResults = await Task.WhenAll(tasks);

                results.AddRange(autocompleteResults);
            }

            return results;
        }
    }
}
