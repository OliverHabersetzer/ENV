using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ENV.Debug
{
    [TestFixture]
    public class SerialisationTest
    {
        [Test]
        public void Primitives()
        {
            new object[2, 2, 2].Print("3D Array");
            new[,] {{"hallo"}, {"welt"}}.Print("new C()");
            var a = new int[4] {1, 3, 3, 7};
            var b = new object[4] {a, null, "42", null};
            var c = new Array[2] {a, b};
            c.Print("Complex array");

            "example string".Print("String test");
            new[] {"", null}.Print("Null test");
            '%'.Print("Char test");
            new G<int, bool, decimal>().Print("new G<T1, T2, T3>()");
            Directory.GetDirectories(@"C:\").ToList().Print(@"C:\ contents");
            0.Print("0");
            0.0.Print("0.0");
            new C().Print("new C()");
            new[] {"hallo", "welt"}.Print("new C()");
        }

        public class C
        {
        }

        public class G<T1, T2, T3>
        {
        }
    }
}