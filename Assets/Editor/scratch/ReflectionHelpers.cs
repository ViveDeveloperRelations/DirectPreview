using System;
using System.Reflection;

namespace DirectPreview.Utility
{
    public class ReflectionHelpers
    {

#region Fields
        private static FieldInfo GetField(object obj, string fieldName, BindingFlags flags)
        {
            var type = obj.GetType();
            //do we need to check the above for null? I don't think so, but I'm not sure.
            var field = GetFieldFromType(type, fieldName, flags);
            if (field == null) throw new Exception("Field " + fieldName + "not found on type " + type.Name);
            return field;
        }

        private static FieldInfo GetFieldFromType(Type type, string fieldName, BindingFlags flags)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var field = type.GetField(fieldName, flags);
            if (field == null) throw new Exception("Field " + fieldName + "not found on type " + type.Name);
            return field;
        }
        private static void SetField(object obj, string fieldName, BindingFlags flags,
            object value) 
        {
            var type = obj.GetType();
            //do we need to check the above for null? I don't think so, but I'm not sure.
            var field = GetFieldFromType(type, fieldName, flags);
            field.SetValue(obj, value);
        }
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            SetField(obj, fieldName, BindingFlags.NonPublic | BindingFlags.Instance, value);
        }

        public static object GetPrivateField(object obj, string fieldName)
        {
            return GetField(obj, fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }

        public static void SetPrivateStaticField(Type type, string fieldName, object value)
        {
            GetFieldFromType(type, fieldName, BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
        }

        public static object GetPrivateStaticField(Type type, string fieldName)
        {
            return GetFieldFromType(type, fieldName, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }
#endregion //fields

#region Properties

        private static PropertyInfo GetProperty(object obj, string propertyName, BindingFlags flags)
        {
            var type = obj.GetType();
            //do we need to check the above for null? I don't think so, but I'm not sure.
            var property = GetPropertyFromType(type, propertyName, flags);
            return property;
        }
        private static PropertyInfo GetPropertyFromType(Type type, string propertyName, BindingFlags flags)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var property = type.GetProperty(propertyName, flags);
            if (property == null) throw new Exception("Field " + propertyName + "not found on type " + type.Name);
            return property;
        }

        public static object GetPublicPropertyObject(object obj, string propertyName)
        {
            return GetProperty(obj,propertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(obj);
        }
        public static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            GetProperty(obj,propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }

        public static object GetPrivateProperty(object obj, string propertyName)
        {
            return GetProperty(obj, propertyName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
        public static void SetPrivateStaticProperty(Type type, string propertyName, object value)
        {
            GetPropertyFromType(type, propertyName, BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
        }

        public static object GetPrivateStaticProperty(Type type, string propertyName)
        {
            return GetPropertyFromType(type, propertyName, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }
#endregion //Properties

#region Methods
        private static MethodInfo GetMethodInfoFromType(Type type, BindingFlags flags, string methodName,params object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var methodInfo = type.GetMethod(methodName, flags);
            if (methodInfo == null) throw new Exception("Method " + methodName + "not found on type " + type.Name);
            return methodInfo;
        }
        public static object InvokePrivateMethod(object obj, string methodName, params object[] args)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            return GetMethodInfoFromType(obj.GetType(),BindingFlags.Instance| BindingFlags.NonPublic, methodName,args).Invoke(obj, args);
        }

        public static object InvokePrivateStaticMethod(Type type, string methodName, params object[] args)
        {
            return GetMethodInfoFromType(type, BindingFlags.Static | BindingFlags.NonPublic, methodName, args).Invoke(null, args);
        }

        public static object InvokePublicMethod(object obj, string methodName, params object[] args)
        {
            return GetMethodInfoFromType(obj.GetType(), BindingFlags.Instance | BindingFlags.Public, methodName, args).Invoke(obj, args);
        }

        public static void InvokePublicMethodVoidReturn(object obj, string methodName, params object[] args)
        {
            GetMethodInfoFromType(obj.GetType(), BindingFlags.Instance | BindingFlags.Public, methodName, args).Invoke(obj, args);
        }

        public static object InvokePublicStaticMethod(Type type, string methodName, params object[] args)
        {
            return GetMethodInfoFromType(type, BindingFlags.Static | BindingFlags.Public, methodName, args).Invoke(null, args);
        }

        public static void InvokePublicStaticMethodVoidReturn(Type type, string methodName, params object[] args)
        {
            GetMethodInfoFromType(type, BindingFlags.Static | BindingFlags.Public, methodName, args).Invoke(null, args);
        }
#endregion
    }
}