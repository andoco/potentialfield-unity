namespace Andoco.Unity.Framework.Installers
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Andoco.Core;
    using Andoco.Core.Reflection;
    using Andoco.Core.Signals;
    using Andoco.Unity.Framework.Core;

    using UnityEngine;

    using Zenject;

    public static class DiContainerExtensions
    {
        /// <summary>
        /// Binds the supplied assemblies to the <see cref="Assembly"/> service type, allowing a list of them to be injected into dependencies.
        /// </summary>
        /// <param name="container">The container to bind in.</param>
        /// <param name="assemblies">The assemblies to bind in the container.</param>
        public static void BindAssemblies(this DiContainer container, params Assembly[] assemblies)
        {
            assemblies.ForEach(assembly => container.Bind<Assembly>().FromInstance(assembly).AsSingle());
        }

        /// <summary>
        /// Discovers and binds all concrete <see cref="ISignal{}"/> types in the assembly.
        /// </summary>
        /// <remarks>
        /// If the signal inherits from <see cref="SimpleSignal"/> it will also be bound against that type.
        /// </remarks>
        /// <param name="container">Container.</param>
        /// <param name="assembly">Assembly.</param>
        [System.Obsolete("This is now handled by CoreInstaller using Zenject's convention based binding")]
        public static void AutoBindSignals(this DiContainer container, Assembly assembly)
        {
            var signalTypes = assembly
                .FindTypesImplementingOpenGenericType(typeof(ISignal<>))
                .Where(t => t.IsConcrete());
            
            var simpleSignalType = typeof(SimpleSignal);

            foreach (var t in signalTypes)
            {
                if (!container.HasBinding(new InjectContext(container, t, null)))
                {
                    container.Bind(t).AsSingle();
                }

                if (simpleSignalType.IsAssignableFrom(t))
                {
                    container.Bind(simpleSignalType).To(t).AsSingle();
                }
            }
        }

        public static void AutoBindToSceneMonoBehaviour<TContract>(this DiContainer container)
        {
            container.AutoBindToSceneMonoBehaviour(typeof(TContract));
        }

        public static void AutoBindToSceneMonoBehaviour(this DiContainer container, Type contractType)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(contractType))
            {
                var instance = GameObject.FindObjectOfType(contractType);

                if (instance != null)
                {
                    container.Bind(contractType).FromInstance(instance);
                    return;
                }
            }
            else if (contractType.IsInterface)
            {
                var behaviours = FindAllBehavioursInScene();
                for (int i=0; i < behaviours.Length; i++)
                {
                    var behaviour = behaviours[i];
                    if (behaviour.gameObject.activeInHierarchy && contractType.IsAssignableFrom(behaviour.GetType()))
                    {
                        container.Bind(contractType).FromInstance(behaviour);
                        return;
                    }
                }
            }

            throw new InvalidOperationException(string.Format("Cannot discover object in scene for the contract type {0}", contractType));
        }

        /// <summary>
        /// Finds all <see cref="MonoBehaviour"/> instances in the scene that are assignable to <typeparamref name="TContract"/> and registers them with the container.
        /// </summary>
        /// <param name="container">The container to register with.</param>
        /// <typeparam name="TContract">The type of the contract to discover on behaviours in the scene.</typeparam>
        public static void AutoBindToAllInScene<TContract>(this DiContainer container)
        {
            container.AutoBindToAllInScene(typeof(TContract));
        }

        /// <summary>
        /// Finds all <see cref="MonoBehaviour"/> instances in the scene that are assignable to <paramref name="contractType"/> and registers them with the container.
        /// </summary>
        /// <param name="container">The container to register with.</param>
        /// <param name="contractType">The type of the contract to discover on behaviours in the scene.</param>
        public static void AutoBindToAllInScene(this DiContainer container, Type contractType)
        {
            var behaviours = FindAllBehavioursInScene();
            for (int i=0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                var behaviourType = behaviour.GetType();

                if (contractType.IsAssignableFrom(behaviourType))
                {
                    container.Bind(behaviourType).FromInstance(behaviour);

                    // TODO: Also bind to implemented interfaces?
                }
                else
                {
                    var fields = behaviourType.GetFields();
                    for (int j=0; j < fields.Length; j++)
                    {
                        var field = fields[j];

                        if (contractType.IsAssignableFrom(field.FieldType))
                        {
//                            Debug.LogFormat("Found bindable field {0} for contract type {1}", field, contractType);
                            var instance = field.GetValue(behaviour);
                            container.Bind(field.FieldType).FromInstance(instance);

                            var interfaces = field.FieldType.GetInterfaces();
                            for (var k = 0; k < interfaces.Length; k++)
                            {
                                container.Bind(interfaces[k]).FromInstance(instance);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Binds all interface types implemented by <paramref name="instance"/> to <paramref name="instance"/>.
        /// </summary>
        /// <param name="container">The container to register with.</param>
        /// <param name="instance">The instance to bind all interfaces for.</param>
        /// <param name="identifier">The identifier to bind the instance with.</param>
        public static void BindAllInterfacesToInstance(this DiContainer container, object instance, string identifier = null)
        {
            var interfaces = instance.GetType().GetInterfaces();

            for (var i = 0; i < interfaces.Length; i++)
            {
                if (identifier == null)
                {
                    container.Bind(interfaces[i]).FromInstance(instance);
                }
                else
                {
                    container.Bind(interfaces[i]).WithId(identifier).FromInstance(instance);
                }
            }

        }

        #region Private methods

        private static MonoBehaviour[] FindAllBehavioursInScene()
        {
            return GameObject.FindObjectsOfType<MonoBehaviour>();
        }

        #endregion
    }
}
