using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityIoc.Runtime.Attributes;
using UnityIoc.Runtime.Types;

namespace UnityIoc.Runtime
{
    public class IocConfig : ScriptableObject
    {

        #region Singleton

        public static string DefaultPath = "Assets/Resources/IocConfig.asset";
        public        string Path        = DefaultPath;

        private static IocConfig _instance;

        public static IocConfig Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var instances = Resources.FindObjectsOfTypeAll<IocConfig>();

#if UNITY_EDITOR
                if (instances.Length == 0) {
                    var so = CreateInstance<IocConfig>();
                    if (!AssetDatabase.IsValidFolder("Assets/Resources")) {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    AssetDatabase.CreateAsset(so, DefaultPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    instances = Resources.FindObjectsOfTypeAll<IocConfig>();
                }
#endif

                switch (instances.Length) {
                    case 0:
                        Debug.LogError("No instance of IocConfig found!");
                        break;
                    case > 1:
                        Debug.LogError("Multiple instances of IocConfig found!");
                        break;
                    default:
                        _instance = instances[0];
#if UNITY_EDITOR
                        _instance.Path = AssetDatabase.GetAssetPath(_instance);
#endif
                        break;
                }

                return _instance;
            }
        }

        protected void OnEnable() {
            if (_instance == null) {
                _instance = this;
            } else if (_instance != this) {
                Debug.LogError("Multiple instances of IocConfig found!");
            }
        }

        protected void OnDisable() {
            if (_instance == this) {
                _instance = null;
            }
        }

        #endregion

        public List<AssetReference> assetReferences = new();

        public TypeReference IocBootstrapperType;

#if UNITY_EDITOR
        [MenuItem("IOC/Add References")]
        public static void AddReferences() {
            Instance.AddPrefabs();
        }

        private void AddPrefabs() {
            var prefabTypes = TypeCache.GetTypesWithAttribute<PrefabAttribute>();

            foreach (var t in prefabTypes) {
                var attr = t.GetCustomAttribute<PrefabAttribute>();
                if (attr == null) continue;

                var path = attr.Path;
                if (string.IsNullOrEmpty(path)) {
                    Debug.LogError("Prefab path is null or empty for " + t.Name);
                    continue;
                }

                var assetRef = new AssetReference();
                assetRef.SetAssetAtPath(path, t);
                var asset = AssetDatabase.LoadAssetAtPath(assetRef.AssetPath, t);
                if (asset == null) {
                    Debug.LogError("Could not load asset at path " + assetRef.AssetPath);
                    continue;
                }

                assetRef.Asset = asset;

                assetReferences.Add(assetRef);

                EditorUtility.SetDirty(this);
            }
        }

#endif

        // public class AssetReferences : SerializableDictionary<TypeReference, > {}
    }
}