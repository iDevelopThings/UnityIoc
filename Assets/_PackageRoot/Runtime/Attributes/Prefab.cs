using System;

namespace UnityIoc.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrefabAttribute : Attribute
    {
        public string Path { get; set; }
    }
}