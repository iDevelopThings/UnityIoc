using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityIoc.Runtime
{
    public struct PrefabInfo
    {
        public Type       Type   { get; set; }
        public string     Path   { get; set; }
        public GameObject Prefab { get; set; }

        public Func<object> Factory { get; set; }

        public PrefabInfo(Type type, string path, Func<object> factory) {
            var pf = Resources.Load<GameObject>(path);
            
            Prefab = pf;
            Path   = path;
            Type   = type;

            factory ??= () => Object.Instantiate(pf);
            Factory =   factory;
        }
    }
}