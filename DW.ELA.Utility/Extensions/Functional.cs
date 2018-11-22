namespace DW.ELA.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Functional
    {
        public static Exception ExecuteAndCatch(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Exception ExecuteAndCatch<T>(Action<T> action, T argument)
        {
            try
            {
                action(argument);
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static void ExecuteManyWithAggregateException<T>(this IEnumerable<T> items, Action<T> function)
        {
            var exceptions = items
                .Select(i => ExecuteAndCatch(function,i))
                .Where(e => e != null)
                .ToList();
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }

        public static void ExecuteManyWithAggregateException<T>(this ParallelQuery<T> items, Action<T> function)
        {
            var exceptions = items
                .Select(i => ExecuteAndCatch(function, i))
                .Where(e => e != null)
                .ToList();
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}
