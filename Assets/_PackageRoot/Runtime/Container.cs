using System;
using UnityEngine;

namespace UnityIoc.Runtime
{
    public class Container : MultiSceneSingleton<Container>
    {
        private ContainerManager _containerInstance;

        protected override void Awake() {
            base.Awake();

            _containerInstance ??= new ContainerManager();
        }

        protected override void OnSingletonReset() {
            base.OnSingletonReset();
            _containerInstance = null;
        }

        public static void RegisterSingleton<TInterface, TConcrete>(Func<object> factory = null) where TConcrete : class, TInterface, new()
            => Instance._containerInstance.RegisterSingleton<TInterface, TConcrete>(factory);

        public static void RegisterSingleton<TConcrete>(Func<object> factory = null) where TConcrete : class
            => Instance._containerInstance.RegisterSingleton<TConcrete>(factory);

        public static void RegisterTransient<TInterface, TConcrete>() where TConcrete : class, TInterface, new()
            => Instance._containerInstance.RegisterTransient<TInterface, TConcrete>();

        public static void RegisterPrefab<T>(string path, Func<T> factory = null) where T : MonoBehaviour
            => Instance._containerInstance.RegisterPrefab<T>(path, factory);

        public static T Resolve<T>()
            => Instance._containerInstance.Resolve<T>();

        public T InstantiatePrefab<T>() where T : MonoBehaviour
            => Instance._containerInstance.InstantiatePrefab<T>();
    }
}