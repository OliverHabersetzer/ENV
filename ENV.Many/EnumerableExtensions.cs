using System;
using System.Collections;
using System.Collections.Generic;

namespace ENV.Many
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
                action(element);
        }

        public static IEnumerable<R> Select<T, R>(IEnumerable<T> enumerable, Func<T, R> func)
        {
            List<R> list = new List<R>();
            foreach (var element in enumerable)
                list.Add(func(element));
            return list;
        }

        /// <summary>
        /// Loops over all elements in the enumeration, executing specific actions based on a boolean expression
        /// </summary>
        /// <param name="enumerable">Source for elements</param>
        /// <param name="predicate">Decider which action should be executed for each element</param>
        /// <param name="trueAction">Action to be executed on each element where <code>predicate(element) == true</code>.</param>
        /// <param name="falseAction">Action to be executed on each element where <code>predicate(element) == false</code>. Can be null when no falseAction is needed.</param>
        /// <typeparam name="T">Enumeration element type</typeparam>
        /// <exception cref="ArgumentException">When the true action is null, no action can be executed</exception>
        public static void ConditionalForEach<T>(
            IEnumerable<T> enumerable,
            Predicate<T> predicate,
            Action<T> trueAction,
            Action<T> falseAction = null)
        {
            if (trueAction == null)
                throw new ArgumentException("trueAction cannot be null");

            var isFalseActionNotNull = falseAction != null;

            foreach (var element in enumerable)
            {
                if (predicate(element))
                    trueAction(element);
                else if (isFalseActionNotNull)
                    falseAction(element);
            }
        }

        /// <summary>
        /// <para>Loops over all elements in the enumeration, executing specific selector/converter method based on a boolean expression</para>
        /// <para>Can be configured to not add elements to the result when the <code>predicate(element) == false</code> reducing the size of the enumeration</para>
        /// </summary>
        /// <param name="enumerable">Source for elements</param>
        /// <param name="predicate">Decider which selector/converter should be executed for each element</param>
        /// <param name="trueFunc">Selector/converter to be executed on each element where <code>predicate(element) == true</code>.</param>
        /// <param name="falseFunc">Selector/converter to be executed on each element where <code>predicate(element) == false</code>. Can be null when no falseFunc is needed or if no element should be added when <code>predicate(element) == false</code>.</param>
        /// <param name="addDefaultOnNull">When set true, <code>default(TOut)</code> will be added to the result when <code>predicate(element) == false</code>.</param>
        /// <typeparam name="TIn">Type of the input enumeration</typeparam>
        /// <typeparam name="TOut">Type of the output enumeration</typeparam>
        /// <returns>The conditionally converted enumeration</returns>
        /// <exception cref="ArgumentException">When the trueFunc is null, no selector/converter can be executed</exception>
        public static IEnumerable<TOut> ConditionalSelect<TIn, TOut>(
            IEnumerable<TIn> enumerable,
            Predicate<TIn> predicate,
            Func<TIn, TOut> trueFunc,
            Func<TIn, TOut> falseFunc = null,
            bool addDefaultOnNull = false)
        {
            var result = new List<TOut>();

            if (trueFunc == null)
                throw new ArgumentException("trueFunc cannot be null");

            var isFalseActionNotNull = falseFunc != null;

            foreach (var element in enumerable)
            {
                if (predicate(element))
                    result.Add(trueFunc(element));
                else if (isFalseActionNotNull)
                    result.Add(falseFunc(element));
                else if (addDefaultOnNull)
                    result.Add(default(TOut));
            }

            return result;
        }
    }
}