using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityIoc.Runtime.Extensions;

namespace UnityIoc.Runtime
{
    public class TypeInformation
    {
        public Type Type { get; set; }

        public PropertyCachedInfo[] Properties { get; set; }

        public struct PropertyCachedInfo
        {
            public MemberInfo Member { get; set; }

            public FieldInfo    Field    => Member as FieldInfo;
            public PropertyInfo Property => Member as PropertyInfo;

            public Type Type => Member.GetFieldOrPropertyType();

            public IEnumerable<Attribute> Attributes { get; set; }

            public Dictionary<Type, Attribute[]> CachedAttributes { get; set; }

            public Attribute[] AllAttributes { get; set; }

            public PropertyCachedInfo(MemberInfo property) {
                Member        = property;
                Attributes    = property.GetCustomAttributes();
                AllAttributes = Attributes.ToArray();
                CachedAttributes = AllAttributes.GroupBy(a => a.GetType())
                   .ToDictionary(g => g.Key, g => g.ToArray());
            }

            public T GetAttribute<T>() where T : Attribute {
                if (CachedAttributes.TryGetValue(typeof(T), out var attributes)) {
                    var attr = attributes.FirstOrDefault();
                    if (attr != null) {
                        return (T) attr;
                    }
                }

                return null;
            }

            public void Set(object instance, object value) {
                if (Field != null) {
                    Field.SetValue(instance, value);
                } else if (Property != null) {
                    Property.SetValue(instance, value);
                }
            }

        }

        public bool IsMonoSingleton => Type.IsSubclassOf(typeof(MonoBehaviour)) && typeof(ISingleton).IsAssignableFrom(Type);

        public delegate object                  SingletonGetterDelegate(object instance);
        public          SingletonGetterDelegate SingletonGetter { get; set; }

        public TypeInformation(Type type) {
            Type = type;

            Properties = Type.GetAllPropertiesAndFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
               .Select(p => new PropertyCachedInfo(p))
               .ToArray();

            if (IsMonoSingleton) {
                var property = Properties.FirstOrDefault(p => p.Member.Name == "Instance").Property;
                if (property != null) {
                    SingletonGetter = (instance) => property.GetValue(instance);
                }
            }
        }


    }
}