using System;

namespace ENV.Runtime
{
    internal class SpecialNumberSerialisation : ISpecialSerialisation
    {
        public Type[] CompatibleTypes => new Type[]
        {
            // 1B
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(Boolean),
            typeof(Byte),
            typeof(SByte),
            // 2B
            typeof(short),
            typeof(ushort),
            typeof(Int16),
            typeof(UInt16),
            // 4B
            typeof(int),
            typeof(uint),
            typeof(float),
            typeof(Int32),
            typeof(UInt32),
            typeof(Single),
            // 8B
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(Int64),
            typeof(UInt64),
            typeof(Double),
            // 16B
            typeof(decimal),
            typeof(Decimal),
        };

        public string Serialize(object o, bool minimizeSerialisation)
        {
            return o.ToString();
        }
    }
}