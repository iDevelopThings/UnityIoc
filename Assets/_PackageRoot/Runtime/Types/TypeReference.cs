using UnityEngine;

namespace UnityIoc.Runtime.Types
{

    [System.Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _typeAssemblyQualifiedName;

        [System.NonSerialized]
        private System.Type _typeCache;

        public System.Type Type
        {
            get => _typeCache;
            set
            {
                _typeCache                 = value;
                _typeAssemblyQualifiedName = value?.AssemblyQualifiedName;
            }
        }

        public void OnBeforeSerialize() {
            _typeAssemblyQualifiedName = _typeCache?.AssemblyQualifiedName;
        }

        public void OnAfterDeserialize() {
            _typeCache = string.IsNullOrEmpty(_typeAssemblyQualifiedName)
                ? null
                : System.Type.GetType(_typeAssemblyQualifiedName);
        }
    }
}