namespace Andoco.Unity.Framework.Installers
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Reader;
    using Andoco.BehaviorTree.Reader.Source;
    using Andoco.BehaviorTree.Scheduler;
    using Andoco.BehaviorTree.Scheduler.Monitoring;
    using Andoco.Core;
    using Andoco.Core.Reflection;
    using Andoco.Unity.Framework.BehaviorTree;
    using Zenject;

    public class BehaviorInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ITaskScheduler>().To<TaskScheduler>().AsSingle();
            Container.Bind<ITicked>().To<TaskScheduler>().AsSingle();
            Container.Bind<IBehaviorReader>().To<CustomBehaviorReader>().AsSingle();
            Container.Bind<ICustomBehaviourReaderParser>().To<RegexCustomBehaviourReaderParser>().AsSingle();
            Container.Bind<IBehaviorReaderSelector>().To<UnityBehaviorReaderSelector>().AsSingle();
            Container.Bind<ITreePerformanceMonitor>().To<NullTreePerformanceMonitor>().AsSingle();
            Container.Bind<ITaskIdBuilder>().To<TaskIdBuilder>().AsSingle();
            Container.Bind<ITaskTypeFinder>().To<TypeIndexTaskTypeFinder>().AsSingle();
            Container.Bind<ITaskFactory>().To<DefaultTaskFactory>().AsSingle();

            // Task types.
            Container.Bind(x => x.AllTypes().DerivingFrom<ITask>()).ToSelf().AsTransient();

            Container.Bind<IBehaviorSourceResolver>().To<UnityBehaviorSourceResolver>().AsSingle();
            Container.Bind<IBehaviorTreeFactory>().To<CachingBehaviorTreeFactory>().AsSingle();

            Container.Bind<IInitializable>().To<BehaviorStartup>().AsSingle();
        }
    }

    public class BehaviorStartup : IInitializable
    {
        DiContainer container;

        ITypeIndex typeIndex;

        public BehaviorStartup(DiContainer container, ITypeIndex typeIndex)
        {
            this.container = container;
            this.typeIndex = typeIndex;
        }

        public void Initialize()
        {
            var taskFactory = (DefaultTaskFactory)container.Resolve<ITaskFactory>();
            taskFactory.Resolver = t => container.Resolve(t);

            var taskTypeAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in taskTypeAssemblies)
            {
                foreach (var t in assembly.FindAssignableTypes(typeof(ITask)))
                {
                    this.typeIndex.IndexByAllAssignableToTypes(t);
                }
            }
        }
    }
}
