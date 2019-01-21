using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ENV.Many
{
    public class Range : IEnumerable<long>
    {
        private readonly long[] _rangeArray;

        public static Range From(long toExclusive) => new Range(toExclusive);
        public static Range From(long fromInclusive, long toExclusive) => new Range(fromInclusive, toExclusive);

        public Range(long toExclusive) : this(0, toExclusive)
        {
        }

        public Range(long fromInclusive, long toExclusive)
        {
            _rangeArray = new long[toExclusive - fromInclusive];
            for (long i = fromInclusive; i < toExclusive; i++)
            {
                _rangeArray[i - fromInclusive] = i;
            }
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