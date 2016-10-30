namespace Andoco.Unity.Framework.Core
{
    using System;
    using Zenject;

    public class ZenjectTypeFactory : ITypeFactory
    {
        private readonly DiContainer container;

        public ZenjectTypeFactory(DiContainer container)
        {
            this.container = container;
        }

        public object CreateInstance(Type type, params object[] args)
        {
            return this.container.Instantiate(type, args);
        }

        public object GetInstance(Type type, string identifier)
        {
            return this.container.Resolve(type, identifier);
        }
    }
}
