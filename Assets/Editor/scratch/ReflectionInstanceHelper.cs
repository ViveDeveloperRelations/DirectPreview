using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DirectPreview.Utility;
using UnityEngine;

namespace DirectPreview.Utility
{
    public class ReflectionInstanceHelper 
    {
        private Type m_Type;
        public readonly object Instance;
        public ReflectionInstanceHelper(Type type, object instance)
        {
            m_Type = type ?? throw new ArgumentNullException(nameof(type));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        
#region Fields
        public object GetPublicField(string fieldName)
        {
            return ReflectionHelpers.GetPublicField(Instance, fieldName);
        }
        public void SetPublicField(string fieldName, object value)
        {
            ReflectionHelpers.SetPrivateField(Instance, fieldName, value);
        }

        public object GetPrivateField(string fieldName)
        {
            return ReflectionHelpers.GetPrivateField(Instance, fieldName);
        }

        public void SetPrivateStaticField(string fieldName, object value)
        {
            ReflectionHelpers.SetPrivateStaticField(m_Type,fieldName, value);
        }

        public object GetPrivateStaticField(string fieldName)
        {
            return ReflectionHelpers.GetPrivateStaticField(m_Type, fieldName);
        }
#endregion //fields

#region Properties

        public object GetPublicPropertyObject(string propertyName)
        {
            return ReflectionHelpers.GetPublicPropertyObject(Instance, propertyName);
        }
        public void SetPrivateProperty(string propertyName, object value)
        {
            ReflectionHelpers.SetPrivateProperty(Instance, propertyName, value);
        }
        public object GetPrivateProperty(string propertyName)
        {
            return ReflectionHelpers.GetPrivateProperty(Instance, propertyName);
        }
        public void SetPrivateStaticProperty(string propertyName, object value)
        {
            ReflectionHelpers.SetPrivateStaticProperty(m_Type, propertyName, value);
        }
        public object GetPrivateStaticProperty(string propertyName)
        {
            return ReflectionHelpers.GetPrivateStaticProperty(m_Type, propertyName);
        }
#endregion //Properties

        #region Methods
        public object InvokePrivateStaticMethod(string methodName, params object[] args)
        {
            return ReflectionHelpers.InvokePrivateStaticMethod(m_Type, methodName, args);    
        }
        public object InvokePrivateMethod(string methodName, params object[] args)
        {
            return ReflectionHelpers.InvokePrivateMethod(Instance,methodName, args);
        }
        public object InvokePublicMethod(string methodName, params object[] args)
        {
            return ReflectionHelpers.InvokePublicMethod(Instance,methodName, args);
        }
        
        public void InvokePublicMethodVoidReturn(string methodName, params object[] args)
        {
            ReflectionHelpers.InvokePublicMethodVoidReturn(Instance, methodName, args);
        }

        public object InvokePublicStaticMethod(string methodName, params object[] args)
        {
            return ReflectionHelpers.InvokePublicStaticMethod(m_Type, methodName, args);
        }

        public void InvokePublicStaticMethodVoidReturn(string methodName, params object[] args)
        {
            ReflectionHelpers.InvokePublicStaticMethodVoidReturn(m_Type, methodName, args);
        }
        #endregion
    }
    
}
