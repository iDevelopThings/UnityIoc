using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityIoc.Runtime
{

    public interface ISingleton
    {
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class Singleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviour
    {
        protected static T _sInstance;

        // ReSharper disable once StaticMemberInGenericType
        private static object _lock = new();

#if UNITY_EDITOR
        static Singleton() {
            EditorApplication.playModeStateChanged -= ModeStateChanged;
            EditorApplication.playModeStateChanged += ModeStateChanged;
        }

        private static void ModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingPlayMode) {
                if (_sInstance != null && _sInstance is Singleton<T> s) {
                    s.OnSingletonReset();
                }

                _sInstance = null;
            }
        }
#endif

        public static T Instance
        {
            get
            {
                lock (_lock) {
                    if (_sInstance == null) {
                        _sInstance = FindFirstObjectByType<T>();
                        if (_sInstance == null) {
                            var singletonObj = new GameObject();
                            _sInstance        = singletonObj.AddComponent<T>();
                            singletonObj.name = typeof(T) + " (Singleton)";
                            DontDestroyOnLoad(singletonObj);
                        }
                    }

                    return _sInstance;
                }
            }
        }

        public static T EnsureLoaded() => Instance;

        protected virtual void Awake() {
            if (_sInstance == null) {
                _sInstance = this as T;
            } else if (_sInstance != this) {
                Destroy(gameObject);
            }
        }

        private void OnEnable() {
            SceneManager.sceneUnloaded      += OnSceneUnloaded;
            SceneManager.sceneLoaded        += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable() {
            SceneManager.sceneUnloaded      -= OnSceneUnloaded;
            SceneManager.sceneLoaded        -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        protected virtual void OnSceneUnloaded(Scene scene) {
            _sInstance = null;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

        protected virtual void OnActiveSceneChanged(Scene current, Scene next) { }

        protected virtual void OnSingletonReset() { }
    }

    public abstract class MultiSceneSingleton<T> : Singleton<T>, ISingleton where T : MonoBehaviour
    {
        protected override void OnSceneUnloaded(Scene scene) {
            // We'll leave this empty since we don't want to reset 
            // Essentially keeping the handling of `DontDestroyOnLoad` in the base class
        }
    }
}