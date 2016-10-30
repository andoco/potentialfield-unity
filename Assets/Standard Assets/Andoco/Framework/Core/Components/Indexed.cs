using System;
using System.Collections.Generic;
using Andoco.Core.Collections;
using Andoco.Core.Pooling;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Core
{
    public interface IComponentIndex
    {
        void Add(Type type, object instance);

        void Remove(Type type, object instance);

        List<T> GetAll<T>() where T : class;

        T GetSingle<T>() where T : class;

        object GetSingle(Type type);

        T GetSingleOrDefault<T>() where T : class;
    }

    public class DefaultComponentIndex : IComponentIndex
    {
        private readonly MultiMap<Type, object> index = new MultiMap<Type, object>();

        public void Add(Type type, object instance)
        {
            this.index.Add(type, instance);
        }

        public void Remove(Type type, object instance)
        {
            this.index.Remove(type, instance);
        }

        /// <summary>
        /// Gets the behaviours that are indexed by the specified type.
        /// </summary>
        /// <returns>The behaviours indexed by the requested type.</returns>
        /// <typeparam name="T">The indexed type to lookup.</typeparam>
        public List<T> GetAll<T>() where T : class
        {
            var list = ListPool<T>.Take();
            IList<object> indexedBehaviours;

            if (index.TryGet(typeof(T), out indexedBehaviours))
            {
                for (int i = 0; i < indexedBehaviours.Count; i++)
                {
                    list.Add((T)indexedBehaviours[i]);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public T GetSingle<T>() where T : class
        {
            return (T)this.GetSingle(typeof(T));
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public object GetSingle(Type t)
        {
            object val = null;
            IList<object> indexedBehaviours;

            if (index.TryGet(t, out indexedBehaviours))
            {
                if (indexedBehaviours.Count > 1)
                {
                    throw new InvalidOperationException(string.Format("Could not get a single instance that is indexed by the type {0} because multiple instances are indexed", t));
                }

                if (indexedBehaviours.Count > 0)
                {
                    val = indexedBehaviours[0];
                }
            }

            if (val == null)
                throw new InvalidOperationException(string.Format("Could not find an instance that is indexed by the type {0}", t));

            return val;
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public T GetSingleOrDefault<T>() where T : class
        {
            var t = typeof(T);
            T val = null;
            IList<object> indexedBehaviours;

            if (index.TryGet(t, out indexedBehaviours))
            {
                if (indexedBehaviours.Count > 1)
                {
                    throw new InvalidOperationException(string.Format("Could not get a single instance that is indexed by the type {0} because multiple instances are indexed", t));
                }

                if (indexedBehaviours.Count > 0)
                {
                    val = (T)indexedBehaviours[0];
                }
            }

            return val;
        }
    }

    public class Indexed : MonoBehaviour
    {
        private static IComponentIndex index;

        [Inject]
        private IComponentIndex _index;

        [Inject]
        private IBinder binder;

        [Tooltip("The behaviours that will be automatically indexed")]
        public MonoBehaviour[] behavioursToIndex;

        public bool indexByType;
        public bool indexByInterfaces;
        public bool bindZenject;

//        public static void SetIndex(IComponentIndex val)
//        {
//            index = val;
//        }

        /// <summary>
        /// Gets the behaviours that are indexed by the specified type.
        /// </summary>
        /// <returns>The behaviours indexed by the requested type.</returns>
        /// <typeparam name="T">The indexed type to lookup.</typeparam>
        public static List<T> GetAll<T>() where T : class
        {
            return index.GetAll<T>();
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public static T GetSingle<T>() where T : class
        {
            return index.GetSingle<T>();
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public static object GetSingle(Type t)
        {
            return index.GetSingle(t);
        }

        /// <summary>
        /// Gets the behaviour indexed by the specified type.
        /// </summary>
        public static T GetSingleOrDefault<T>() where T : class
        {
            return index.GetSingleOrDefault<T>();
        }

        #region Lifecycle

        [Inject]
        void OnPostInject()
        {
            index = _index;
            this.AddToIndex();
        }

        void Spawned()
        {
            this.AddToIndex();
        }

        void Recycled()
        {
            this.RemoveFromIndex();
        }

        void OnDestroy()
        {
            this.RemoveFromIndex();
        }

        #endregion

        private void AddToIndex()
        {
            for (int i = 0; i < this.behavioursToIndex.Length; i++)
            {
                var b = this.behavioursToIndex[i];
                var t = b.GetType();

                if (this.indexByType)
                {
                    index.Add(t, b);

                    if (this.bindZenject)
                    {
                        this.binder.Bind(t).FromInstance(b);
                    }
                }

                if (this.indexByInterfaces)
                {
                    var interfaces = t.GetInterfaces();

                    for (int j = 0; j < interfaces.Length; j++)
                    {
                        index.Add(interfaces[j], b);

                        if (this.bindZenject)
                        {
                            this.binder.Bind(interfaces[j]).FromInstance(b);
                        }
                    }
                }
            }
        }

        private void RemoveFromIndex()
        {
            for (int i = 0; i < this.behavioursToIndex.Length; i++)
            {
                var b = this.behavioursToIndex[i];
                var t = b.GetType();

                index.Remove(t, b);

                var interfaces = t.GetInterfaces();

                for (int j = 0; j < interfaces.Length; j++)
                {
                    index.Remove(interfaces[j], b);
                }

                // TODO: Unbind from Zenject?
            }
        }
    }
}