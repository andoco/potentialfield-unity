namespace Andoco.Unity.Framework.Core
{
    using System;

    public interface ITypeFactory
    {
        object CreateInstance(Type type, params object[] args);

        object GetInstance(Type type, string identifier);
    }
}
