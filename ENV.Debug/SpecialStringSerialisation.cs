using System;

namespace ENV.Debug
{
    internal class SpecialStringSerialisation : ISpecialSerialisation
    {
        public Type[] CompatibleTypes => new Type[]
        {
            typeof(string),
            typeof(String),
        };

        public string Serialize(object o, bool minimizeSerialisation)
        {
            return "\"" + o + "\"";
        }
    }
}