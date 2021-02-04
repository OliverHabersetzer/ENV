using System;

namespace ENV.Runtime
{
    public interface ISpecialSerialisation
    {
        Type[] CompatibleTypes { get; }
        string Serialize(object o, bool minimizeSerialisation);
    }
}