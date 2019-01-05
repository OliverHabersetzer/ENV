using System;

namespace ENV.Debug
{
    public interface ISpecialSerialisation
    {
        Type[] CompatibleTypes { get; }
        string Serialize(object o, bool minimizeSerialisation);
    }
}