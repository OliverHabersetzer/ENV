using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENV.Many
{
    public static class Repetition
    {
        public static IEnumerable<T> Repeat<T>(this T o, int count = 2)
        {
            T[] result = new T[count];
            while (--count >= 0)
                result[count] = o;
            return result;
        }

        public static string Repeat(this char o, int count = 2)
        {
            return new string(o, count);
        }

        public static string Repeat(this string o, int count = 2)
        {
            StringBuilder builder = new StringBuilder(count);
            while (--count >= 0)
                builder.Append(o);
            return builder.ToString();
        }
    }
}