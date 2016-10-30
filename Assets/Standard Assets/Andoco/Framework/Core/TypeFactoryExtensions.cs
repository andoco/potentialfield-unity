namespace Andoco.Unity.Framework.Core
{
    using System;

    public static class TypeFactoryExtensions
    {
        public static T CreateInstance<T>(this ITypeFactory factory, params object[] args)
        {
            return (T)factory.CreateInstance(typeof(T), args);
        }

        public static object CreateInstance(this ITypeFactory factory, string typeName, params object[] args)
        {
            return factory.CreateInstance(Type.GetType(typeName, true, false), args);
        }

        public static T CreateInstance<T>(this ITypeFactory factory, string typeName, params object[] args)
        {
            return (T)factory.CreateInstance(Type.GetType(typeName, true, false), args);
        }

        public static T GetInstance<T>(this ITypeFactory factory, string identifier = null)
        {
            return (T)factory.GetInstance(typeof(T), identifier);
        }
    }
}
