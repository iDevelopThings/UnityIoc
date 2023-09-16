using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityIoc.Runtime.Attributes;
using Object = UnityEngine.Object;

namespace UnityIoc.Runtime
{

    public delegate void OnResolvingDelegate(object               instance);
    public delegate void OnResolvedDelegate(object                instance);
    public delegate void OnInjectingPropertyDelegate(PropertyInfo property, object instance);
    public delegate void OnInjectedPropertyDelegate(PropertyInfo  property, object instance);

    public class ContainerManager
    {
        // Interface type -> concrete type
        private Dictionary<Type, Type> _mappings = new();

        private Dictionary<Type, TypeInformation> _typeCache = new();

        private Dictionary<Type, object> _singletons = new();

        private Dictionary<Type, Func<object>> _singletonFactories = new();
        private Dictionary<Type, Func<object>> _transientFactories = new();
        private Dictionary<Type, PrefabInfo>   _prefabMappings     = new();

        public event OnResolvingDelegate         OnResolving;
        public event OnResolvedDelegate          OnResolved;
        public event OnInjectingPropertyDelegate OnInjectingProperty;
        public event OnInjectedPropertyDelegate  OnInjectedProperty;

        private TypeInformation EnsureTypeCached(Type type) {
            if (_typeCache.TryGetValue(type, out var typeInfo)) {
                return typeInfo;
            }

            typeInfo         = new TypeInformation(type);
            _typeCache[type] = typeInfo;

            return typeInfo;
        }

        public void RegisterSingleton(Type concreteType, Type interfaceType = null, Func<object> factory = null) {
            var concreteInfo = EnsureTypeCached(concreteType);

            factory ??= () =>
            {
                if (concreteInfo.IsMonoSingleton) {
                    return concreteInfo.SingletonGetter(null);
                }

                return Activator.CreateInstance(concreteType);
            };

            _singletonFactories.TryAdd(concreteType, factory);

            if (interfaceType == null) return;

            EnsureTypeCached(interfaceType);
            _mappings[interfaceType] = concreteType;
        }

        public void RegisterSingleton<TInterface, TConcrete>(Func<object> factory = null) where TConcrete : class, TInterface, new() {
            RegisterSingleton(typeof(TConcrete), typeof(TInterface), factory);
        }

        public void RegisterSingleton<TConcrete>(Func<object> factory = null) where TConcrete : class {
            RegisterSingleton(typeof(TConcrete), null, factory);
        }

        public void RegisterTransient(Type concreteType, Type interfaceType = null) {
            EnsureTypeCached(concreteType);
            _transientFactories.TryAdd(concreteType, () => Activator.CreateInstance(concreteType));

            if (interfaceType == null) return;

            EnsureTypeCached(interfaceType);
            _mappings[interfaceType] = concreteType;
        }

        public void RegisterTransient<TInterface, TConcrete>() where TConcrete : class, TInterface, new() {
            RegisterTransient(typeof(TConcrete), typeof(TInterface));
        }

        public void RegisterPrefab<T>(string path, Func<T> factory = null) where T : MonoBehaviour {
            var type = typeof(T);
            EnsureTypeCached(type);
            var prefabInfo = new PrefabInfo(type, path, factory);
            _prefabMappings[type] = prefabInfo;
        }

        private object FindInstance(Type type, out bool calledFactory) {
            calledFactory = false;

            // If we don't have an instance in _singletons, check if we have a _singletonFactories
            // If we have a _singletonFactories, create an instance and store it in _singletons

            if (_singletons.TryGetValue(type, out var instance)) {
                return instance;
            }

            if (_transientFactories.TryGetValue(type, out var factory)) {
                calledFactory = true;
                return factory();
            }

            if (!_singletons.TryGetValue(type, out instance) && _singletonFactories.TryGetValue(type, out var singletonFactory)) {
                instance          = singletonFactory();
                calledFactory     = true;
                _singletons[type] = instance;
                return instance;
            }

            if (_prefabMappings.TryGetValue(type, out var prefabInfo)) {
                var go = prefabInfo.Factory() as GameObject;
                instance      = go.GetComponent(type);
                calledFactory = true;
                return instance;
            }

            return null;
        }

        public object ResolveUntyped(Type type) {
            object concreteInstance = null;
            var    didCallFactory   = false;

            OnResolving?.Invoke(type);

            if (_mappings.TryGetValue(type, out var concreteType)) {
                concreteInstance = FindInstance(concreteType, out didCallFactory);
                if (concreteInstance != null) {
                    return FinalizeResolve(concreteType, concreteInstance, didCallFactory);
                }
            }

            concreteInstance = FindInstance(type, out didCallFactory);
            if (concreteInstance != null) {
                return FinalizeResolve(type, concreteInstance, didCallFactory);
            }

            throw new Exception($"Service of type {type} not registered.");
        }

        public T Resolve<T>() {
            return (T) ResolveUntyped(typeof(T));
        }

        public T InstantiatePrefab<T>() where T : MonoBehaviour {
            return (T) ResolveUntyped(typeof(T));
        }

        private object FinalizeResolve(Type type, object instance, bool didCallFactory) {
            var typeInfo = _typeCache[type];

            OnResolved?.Invoke(instance);

            return InjectProperties(instance, typeInfo, didCallFactory);
        }

        private object InjectProperties(object instance, TypeInformation typeInfo, bool didCallFactory) {
            if (!didCallFactory)
                return instance;

            foreach (var prop in typeInfo.Properties) {
                if (prop.GetAttribute<InjectAttribute>() == null)
                    continue;

                OnInjectingProperty?.Invoke(prop.Property, instance);

                var value = ResolveUntyped(prop.Type);
                prop.Set(instance, value);

                OnInjectedProperty?.Invoke(prop.Property, instance);
            }

            return instance;
        }

    }


}