using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ENV.Many
{
    public class Range : IEnumerable<long>
    {
        private readonly long[] _rangeArray;

        public static Range Inclusive(long to) => new Range(to);
        public static Range Inclusive(long from, long to) => new Range(from, to);
        public static Range Exclusive(long to) => new Range(0, to - 1);
        public static Range Exclusive(long from, long to) => new Range(from, to - 1);


        public Range(long to) : this(0, to)
        {
        }

        public Range(long from, long to)
        {
            long interval = Math.Abs(to - from);
            _rangeArray = new long[interval + 1];

            long index = 0;
            long curr = from;
            long step = (to - from) / interval;

            do
            {
                _rangeArray[index] = curr;
                curr += step;
                index++;
            } while (index <= interval);
        }

        public IEnumerator<long> GetEnumerator()
        {
            return this._rangeArray.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}