using System;

namespace ENV.Runtime
{
    internal class SpecialCharSerialisation : ISpecialSerialisation
    {
        public Type[] CompatibleTypes => new Type[]
        {
            typeof(char),
            typeof(Char),
        };

        public string Serialize(object o, bool minimizeSerialisation)
        {
            return $"'{o}'";
        }
    }
}