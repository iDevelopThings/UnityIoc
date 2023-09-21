using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityIoc.Runtime.Types
{
    [System.Serializable]
    public class AssetReference // : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _assetGuid;

        [SerializeField]
        private string _assetPath;

        [SerializeField]
        private TypeReference _assetType;

        [SerializeField]
        private Object _asset;

        public string AssetGuid => _assetGuid;
        public string AssetPath => _assetPath;

        public Object Asset
        {
            get => _asset;
            set => _asset = value;
        }

        public TypeReference AssetType
        {
            get => _assetType;
            set => _assetType = value;
        }

        public T LoadAsset<T>() where T : Object {
            return Resources.Load<T>(_assetPath);
        }

        public Object LoadAsset() {
            return Resources.Load(_assetPath, AssetType.Type) as Object;
        }

#if UNITY_EDITOR
        public void SetAsset<T>(T asset) where T : Object {
            _assetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
            _assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(_assetPath);
            _assetType = new TypeReference {Type = typeof(T)};
        }

        public void SetAssetAtPath(string path, Type type) {
            _assetPath = path;
            _assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(_assetPath);
            _assetType = new TypeReference {Type = type};
        }
#endif

        // public void OnBeforeSerialize() { }
        // public void OnAfterDeserialize() { }
    }
}