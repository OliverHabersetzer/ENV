using System;
using System.Collections.Generic;

namespace ENV.Many
{
    public static class EnumerableExtensions
    {
        public static void ConditionalForEach<T>(IEnumerable<T> enumerable, Predicate<T> predicate,
            Action<T> trueAction,
            Action<T> falseAction = null)
        {
            if (trueAction == null)
                throw new ArgumentException("True action cannot be null");

            var isFalseActionNotNull = falseAction != null;

            foreach (var element in enumerable)
            {
                if (predicate(element))
                    trueAction(element);
                else if (isFalseActionNotNull)
                    falseAction(element);
            }
        }
    }
}