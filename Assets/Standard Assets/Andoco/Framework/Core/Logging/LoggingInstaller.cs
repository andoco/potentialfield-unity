namespace Andoco.Unity.Framework.Core.Logging
{
    using System.Linq;
    using Andoco.Core.Diagnostics.Logging;
    using UnityEngine;
    using Zenject;

    public class LoggingInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IStructuredLogEventWriter>().To<UnityStructuredLogEventWriter>().AsSingle();

            Container
                .Bind<ILoggerFactory>()
                .FromMethod(ctx =>
                {
                    var subcontainer = ctx.Container.CreateSubContainer();
                    BindLoggerFactory(subcontainer, ctx);

                    return subcontainer.Resolve<ILoggerFactory>();
                });

            Container
                .Bind<IStructuredLog>()
                .FromMethod(ctx =>
                {
                    var subcontainer = ctx.Container.CreateSubContainer();
                    BindLoggerFactory(subcontainer, ctx);

                    return subcontainer
                        .Resolve<ILoggerFactory>()
                        .CreateStructuredLog(new LogSource(null, ctx.ObjectType));
                });
        }

        private static void BindLoggerFactory(DiContainer container, InjectContext ctx)
        {
            var containingGo = GetContainingGameObject(ctx);

            if (containingGo != null)
            {
                container.Bind<GameObject>().FromInstance(containingGo);
            }

            container.Bind<ILoggerFactory>().To<UnityLoggerFactoryV2>().AsSingle();
        }

        private static GameObject GetContainingGameObject(InjectContext ctx)
        {
            var firstMonoBehaviourCtx = ctx.ParentContextsAndSelf.FirstOrDefault(x => x.ObjectInstance is MonoBehaviour);

            if (firstMonoBehaviourCtx == null)
                return null;
            
            var topComponent = firstMonoBehaviourCtx.ObjectInstance as MonoBehaviour;

            if (topComponent == null)
                return null;

            return topComponent.gameObject;
        }
    }
}
