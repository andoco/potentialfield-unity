namespace Andoco.Unity.Framework.Level
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Andoco.Core;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Data;
    using Andoco.Unity.Framework.Level.Monitors;
    using Andoco.Unity.Framework.Level.Objectives;
    using UnityEngine;
    using Zenject;

    [System.Serializable]
    public class LevelState
    {
        public int levelNumber;
    }

    public class LevelSystem : MonoBehaviour, ILevelSystem
    {
        private readonly List<ILevelObjective> objectives = new List<ILevelObjective>();
        private readonly List<ILevelMonitor> levelEvents = new List<ILevelMonitor>();
        private readonly Dictionary<Type, ILevelConfig> configs = new Dictionary<Type, ILevelConfig>();

        private SimpleFsm<States, Events> fsm;

        private bool pausedFlag;
        private bool stopFlag;

        [Inject]
        private IStructuredLog log;

        [Inject]
        private IGameData gameData;

        [InjectOptional]
        private List<ILevelObjective> initialObjectives;

        [InjectOptional]
        private List<ILevelMonitor> initialMonitors;

        private LevelState state;

        public string stateDataKey = "LevelState";

        [Inject]
        void OnPostInject()
        {
            this.state = gameData.GetOrAdd<LevelState>(this.stateDataKey);
            this.SetupFsm();
            this.Objectives = this.objectives.AsReadOnly();

            if (initialObjectives != null)
            {
                for (var i = 0; i < initialObjectives.Count; i++)
                {
                    this.AddObjective(initialObjectives[i]);
                }
            }

            if (initialMonitors != null)
            {
                for (var i = 0; i < initialMonitors.Count; i++)
                {
                    this.AddEvent(initialMonitors[i]);
                }
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.fsm.CurrentState == States.Playing ||
                    this.fsm.CurrentState == States.Paused;
            }
        }

        public bool IsPaused { get { return this.fsm.CurrentState == States.Paused; } }

        public float LevelTime { get; private set; }

        public int LevelNumber
        {
            get
            {
                return this.state.levelNumber;
            }
        }

        public ReadOnlyCollection<ILevelObjective> Objectives { get; private set; }

        public void AddObjective(ILevelObjective objective)
        {
            this.objectives.Add(objective);
        }

        public void AddEvent(ILevelMonitor levelEvent)
        {
            this.levelEvents.Add(levelEvent);
        }

        public LevelObjectiveResult CheckRequiredObjectives(string category = null)
        {
            var pending = false;

            for (var i = 0; i < this.objectives.Count; i++)
            {
                var objective = this.objectives[i];

                var categoryMatch = category == null || string.Equals(objective.Category, category, StringComparison.OrdinalIgnoreCase);

                if (categoryMatch && objective.IsRequired)
                {
                    var status = objective.CheckObjectiveStatus();

                    log.Trace("Checked level objective").Field("name", objective.Name).Field("status", status).Write();

                    switch (status)
                    {
                        case LevelObjectiveResult.Failed:
                            return LevelObjectiveResult.Failed;
                        case LevelObjectiveResult.Pending:
                            // If any required objective is pending, we don't need to check the rest.
                            pending = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            return pending ? LevelObjectiveResult.Pending : LevelObjectiveResult.Completed;
        }

        public void AddConfig(ILevelConfig config)
        {
            this.configs.Add(config.GetType(), config);
        }

        public T GetConfigOrDefault<T>() where T : ILevelConfig
        {
            ILevelConfig value;
            if (this.configs.TryGetValue(typeof(T), out value))
            {
                return (T)value;
            }

            return default(T);
        }

        public void Configure(int levelNumber)
        {
            this.state.levelNumber = levelNumber;
        }

        public void StartLevel()
        {
            this.stopFlag = false;
            this.pausedFlag = false;
            this.LevelTime = 0f;

            this.fsm.Start(States.Playing);
        }

        public void PauseLevel()
        {
            this.pausedFlag = true;
        }

        public void ResumeLevel()
        {
            this.pausedFlag = false;
        }

        public void StopLevel()
        {
            this.stopFlag = true;

            // Execute immediately so that it moves to the Stopped state without delay.
            this.fsm.Execute();

            this.fsm.Stop();

            for (var i = 0; i < this.objectives.Count; i++)
            {
                this.objectives[i].Reset();
            }

            for (var i = 0; i < this.levelEvents.Count; i++)
            {
                this.levelEvents[i].ResetMonitor();
            }
        }

        public void ResetState()
        {
            this.state.levelNumber = 0;
        }

        #region ITickable implementation

        public void Tick()
        {
            if (this.fsm.IsStarted)
            {
                this.fsm.Execute();
            }
        }

        #endregion

        #region Private methods

        private void SetupFsm()
        {
            this.fsm = new SimpleFsm<States, Events>("Level");

            this.fsm.AddTransition(States.None, States.Playing, Events.Start);
            this.fsm.AddTransition(States.Playing, States.Paused, Events.Pause);
            this.fsm.AddTransition(States.Paused, States.Playing, Events.Resume);
            this.fsm.AddTransition(States.Playing, States.Stopped, Events.Stop);
            this.fsm.AddTransition(States.Paused, States.Stopped, Events.Stop);
            this.fsm.AddTransition(States.Stopped, States.Playing, Events.Start);

            this.fsm.OnEnter(States.Playing, OnPlayingEnter);
            this.fsm.OnExecute(States.Playing, OnPlayingExecute);

            this.fsm.OnExecute(States.Paused, OnPausedExecute);
        }

        private void OnPlayingEnter()
        {
            for (var i = 0; i < this.levelEvents.Count; i++)
            {
                this.levelEvents[i].StartMonitor();
            }
        }

        private Events OnPlayingExecute()
        {
            if (this.stopFlag)
            {
                return Events.Stop;
            }

            if (this.pausedFlag)
            {
                return Events.Pause;
            }

            this.LevelTime += Time.deltaTime;

            return Events.None;
        }

        private Events OnPausedExecute()
        {
            return this.pausedFlag
                ? Events.None
                    : Events.Resume;
        }

        #endregion

        #region Types

        private enum States
        {
            None,
            Paused,
            Playing,
            Stopped
        }

        private enum Events
        {
            None,
            Pause,
            Resume,
            Start,
            Stop
        }

        #endregion
    }
}
