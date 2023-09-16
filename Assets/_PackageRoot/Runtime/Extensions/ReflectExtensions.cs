using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityIoc.Runtime.Extensions
{

    public static class ReflectExtensions
    {
        public static IEnumerable<MemberInfo> GetAllPropertiesAndFields(this Type type, BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) {
            var members = new List<MemberInfo>();
            members.AddRange(type.GetProperties(flags));
            members.AddRange(type.GetFields(flags));
            return members;
        }

        public static T GetFieldOrPropertyValue<T>(this Type obj, object inst, string name, int maxDepth = 2, int depth = 0) {
            var field = obj.GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null) {
                return (T) field.GetValue(inst);
            }

            var property = obj.GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property != null) {
                return (T) property.GetValue(inst);
            }

            if (obj.BaseType == null) return default(T);

            return depth < maxDepth
                ? GetFieldOrPropertyValue<T>(obj.BaseType, inst, name, maxDepth, depth + 1)
                : default(T);
        }

        public static bool SetFieldOrPropertyValue<T>(this Type obj, object inst, string name, T value, int maxDepth = 2, int depth = 0) {
            var field = obj.GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null) {
                field.SetValue(inst, value);
                return true;
            }

            var property = obj.GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property != null) {
                property.SetValue(inst, value);
                return true;
            }

            if (obj.BaseType == null) return false;

            if (depth < maxDepth) {
                return SetFieldOrPropertyValue<T>(obj.BaseType, inst, name, value, maxDepth, depth + 1);
            }

            return false;
        }

        public static Type GetFieldOrPropertyType(this MemberInfo member) {
            if (member is FieldInfo field) {
                return field.FieldType;
            }

            if (member is PropertyInfo property) {
                return property.PropertyType;
            }

            return null;
        }
    }
}