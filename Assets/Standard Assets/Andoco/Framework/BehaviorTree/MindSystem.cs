namespace Andoco.Unity.Framework.BehaviorTree
{
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Scheduler;
    using Andoco.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class MindSystem : MonoBehaviour, IMindSystem
    {
        private bool initialised = false;
        readonly List<MindState> minds = new List<MindState>();

        [Inject]
        private ITaskScheduler scheduler;

        [Inject]
        private IBehaviorTreeFactory behaviorTreeFactory;

        public float iterationTimeLimit;
        public bool profileTaskExecution = false;

        #region Lifecycle

        [Inject]
        void OnPostInject()
        {
            EnsureInitialised();
        }

        void Update()
        {
            for (int i = 0; i < minds.Count; i++)
            {
                if (minds[i].Valid && minds[i].UpdateDue)
                {
                    scheduler.Schedule(minds[i].Context);
                    minds[i].LastRun = Time.time;
                }
            }
        }

        #endregion

        public void Add(Mind mind)
        {
            Assert.IsFalse(minds.Any(x => x.Component == mind));

            var state = new MindState(mind);

            state.Tree = this.behaviorTreeFactory.Create(mind.BehaviorFile);

            state.Context = new BehaviorContext(
                string.Format("{0}-{1}",
                mind.gameObject.name,
                mind.gameObject.GetInstanceID()),
                state.Tree.Root);

            minds.Add(state);

            StartMind(state);
        }

        public void StartMind(Mind mind)
        {
            var state = minds.Single(x => x.Component == mind);
            StartMind(state);
        }

        public void StopMind(Mind mind)
        {
            var state = minds.Single(x => x.Component == mind);
            StopMind(state, true);
        }

        public void Remove(Mind mind)
        {
            var index = minds.FindIndex(x => x.Component == mind);
            StopMind(minds[index], false);
            minds.RemoveAt(index);
        }

        #region Private methods

        private void EnsureInitialised()
        {
            if (!initialised)
            {
                this.scheduler.IterationTimeLimit = this.iterationTimeLimit;

                if (this.profileTaskExecution)
                {
                    this.SetupProfiling();
                }

                initialised = true;
            }
        }

        private void SetupProfiling()
        {
            this.scheduler.OnTaskExecuteBegin += (task, ctx) =>
            {
                Profiler.BeginSample("{0}-{1}".FormatWith(task.GetType().Name, task.Id));
            };

            this.scheduler.OnTaskExecuteEnd += (task, ctx) =>
            {
                Profiler.EndSample();
            };
        }

        private void StartMind(MindState state)
        {
            var ctx = state.Context;
            var go = state.Component.gameObject;

            ctx.Status = BehaviorContextStatus.None;

            ctx.SetItem("BehaviorFile", state.Component.BehaviorFile);
            ctx.SetItem("GameObjectName", go.name);
            ctx.SetItem("GameObjectId", go.GetInstanceID());
            ctx.SetItem("gameObject", go);
            ctx.SetItem("signalContext", go);
        }

        private void StopMind(MindState state, bool recycle)
        {
            // Mind could already have been stopped if MindCoordinator was destroyed first
            if (state.IsMindStarted)
            {
                state.Tree.Root.Stop(state.Tree.Root.GetNode(state.Context));

                // remove any schedule that might have been added in this game loop
                this.scheduler.Unschedule(state.Context);

                if (recycle)
                    state.Context.Reset();
                else
                    state.Context.Dispose();
            }

            state.IsMindStarted = false;
        }

        #endregion

        private class MindState
        {
            public MindState(Mind component)
            {
                this.Component = component;
            }

            public Mind Component { get; private set; }

            public bool IsMindStarted { get; set; }

            public float LastRun { get; set; }

            public IBehaviorTree Tree { get; set; }

            public IBehaviorContext Context { get; set; }

            public bool Valid { get { return Component.gameObject.activeInHierarchy; } }

            public bool UpdateDue { get { return Time.time - LastRun > (1f / Component.IterationRate); } } 
        }
    }
}