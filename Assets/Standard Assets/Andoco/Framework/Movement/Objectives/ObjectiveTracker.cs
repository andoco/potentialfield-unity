namespace Andoco.Unity.Framework.Movement.Objectives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Andoco.Unity.Framework.Core;
    using Andoco.Core.Data;

    public class ObjectiveTracker : MonoBehaviour
    {
    
        private readonly SimpleStore<Transform, IObjective> objectiveStore = new SimpleStore<Transform, IObjective>();
    
        private readonly ListStore<string, IObjective> categoryStore = new ListStore<string, IObjective>();
    
        private readonly Dictionary<string, List<Objective>> categorisedObjectives = new Dictionary<string, List<Objective>>();
    
        private static Dictionary<GameObject, Objective> objectives = new Dictionary<GameObject, Objective>();
    
        private static SingletonGameObjectComponent<ObjectiveTracker> instance = new SingletonGameObjectComponent<ObjectiveTracker>();

        public static ObjectiveTracker Instance
        {
            get
            {
                return instance.Instance;
            }
        }

        #region Public methods

        public void AddCategory(IObjective objective, string category)
        {
            this.categoryStore.Add(category, objective);
        }

        public void RemoveCategory(IObjective objective, string category)
        {
            this.categoryStore.Remove(category, objective);
        }

        public void VisitCategory(string category, Action<string, IObjective> visitor)
        {
            this.categoryStore.Visit(category, visitor);
        }

        public void AddObjective(Objective objective, params string[] categories)
        {
            foreach (var category in categories)
            {
                List<Objective> list;
                if (!this.categorisedObjectives.TryGetValue(category, out list))
                {
                    list = new List<Objective>();
                    this.categorisedObjectives.Add(category, list);
                }
    
                list.Add(objective);
            }
        }

        public IObjective GetOrCreateObjective(Transform tx)
        {
            IObjective objective;
            if (!this.objectiveStore.TryGet(tx, out objective))
            {
                objective = new Objective(tx);
                this.objectiveStore.Add(tx, objective);
            }
    
            return objective;
        }

        public IEnumerable<Objective> GetObjectives(string category)
        {
            List<Objective> list;
            if (this.categorisedObjectives.TryGetValue(category, out list))
            {
                return list;
            }
    
            return Enumerable.Empty<Objective>();
        }

        public void AddObjective(Transform objectiveTx, params string[] categories)
        {
            var objective = new Objective(objectiveTx);
            this.AddObjective(objective);
        }

        public void TrackObjective(GameObject go, Objective objective)
        {
            objectives[go] = objective;
        }

        public Objective TrackObjective(GameObject go, Vector3 target)
        {
            var objective = new Objective(target);
    
            this.TrackObjective(go, objective);
    
            return objective;
        }

        public Objective TrackObjective(GameObject go, Transform target)
        {
            var objective = new Objective(target);
            
            this.TrackObjective(go, objective);
            
            return objective;
        }

        public void ClearObjective(GameObject go)
        {
            objectives.Remove(go);
        }

        public IEnumerable<GameObject> GetWithObjective(Transform target)
        {
            return objectives.Where(item => item.Value.TargetTransform == target).Select(item => item.Key);
        }
    
        /// <summary>
        /// Calculates and assigns a weight to each objective in the specified category.
        /// </summary>
        /// <param name="category">Category to calculate weights for.</param>
        /// <param name="calculate">Function to calculate the weight.</param>
        //    public void WeightObjectives(string category, Func<Objective, float> calculate)
    //    {
    //
    //    }
    
        #endregion
    }
}
