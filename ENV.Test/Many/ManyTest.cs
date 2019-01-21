using System;
using System.Linq;
using ENV.Debug;
using NUnit.Framework;

namespace ENV.Many
{
    [TestFixture]
    public class ManyTest
    {
        [Test]
        public static void ForwardsRangeTEst()
        {
            Range.Inclusive(5, 42).ToList().ForEach(i => i.Print());
        }

        [Test]
        public static void BackwardsRangeTEst()
        {
            Range.Inclusive(42, 5).ToList().ForEach(i => i.Print());
        }
    }
}