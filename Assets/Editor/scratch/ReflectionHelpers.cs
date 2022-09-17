using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectPreview.Utility
{
    public class ReflectionHelpers
    {
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(field == null) throw new Exception("Field "+ fieldName +"not found");
            field.SetValue(obj, value);
        }

        public static object GetPrivateField(object obj, string fieldName)
        {
            Type type = obj.GetType();
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(field == null) throw new Exception("Field "+ fieldName +"not found");

            return field.GetValue(obj);
        }

        public static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(property == null)
                throw new Exception("Property "+ propertyName +"not found");

            property.SetValue(obj, value);
        }

        public static object GetPrivateProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(property == null)
                throw new Exception("Property "+ propertyName +"not found");
            return property.GetValue(obj);
        }

        public static void SetPrivateStaticField(System.Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if(field == null) throw new Exception("Field "+ fieldName +"not found");
            field.SetValue(null, value);
        }

        public static object GetPrivateStaticField(System.Type type, string fieldName)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if(field == null) throw new Exception("Field "+ fieldName +"not found");
            return field.GetValue(null);
        }

        public static void SetPrivateStaticProperty(System.Type type, string propertyName, object value)
        {
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if(property == null)
                throw new Exception("Property "+ propertyName +"not found");
            property.SetValue(null, value);
        }

        public static object GetPrivateStaticProperty(System.Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if(property == null)
                throw new Exception("Property "+ propertyName +"not found");
            return property.GetValue(null);
        }
        
        public static object InvokePrivateMethod(object obj, string methodName, params object[] args)
        {
            Type type = obj.GetType();
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            return method.Invoke(obj, args);
        }
        public static object InvokePrivateStaticMethod(System.Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            return method.Invoke(null, args);
        }
        public static object InvokePublicMethod(object obj, string methodName, params object[] args)
        {
            Type type = obj.GetType();
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            return method.Invoke(obj, args);
        }
        public static void InvokePublicMethodVoidReturn(object obj, string methodName, params object[] args)
        {
            Type type = obj.GetType();
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            method.Invoke(obj, args);
        }
        public static object InvokePublicStaticMethod(System.Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            return method.Invoke(null, args);
        }
        public static void InvokePublicStaticMethodVoidReturn(System.Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if(method == null)
                throw new Exception("Method "+ methodName +"not found");
            method.Invoke(null, args);
        }
        
    }
}

