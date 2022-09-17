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
        private object m_Instance;
        public ReflectionInstanceHelper(Type type, object instance)
        {
            m_Type = type ?? throw new ArgumentNullException(nameof(type));
            m_Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        
#region Fields
        public void SetPrivateField(string fieldName, object value)
        {
            ReflectionHelpers.SetPrivateField(m_Instance, fieldName, value);
        }

        public object GetPrivateField(string fieldName)
        {
            return ReflectionHelpers.GetPrivateField(m_Instance, fieldName);
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

        public object GetPublicPropertyObject(object obj, string propertyName)
        {
            return ReflectionHelpers.GetPublicPropertyObject(m_Instance, propertyName);
        }
        public void SetPrivateProperty(string propertyName, object value)
        {
            ReflectionHelpers.SetPrivateProperty(m_Instance, propertyName, value);
        }
        public object GetPrivateProperty(string propertyName)
        {
            return ReflectionHelpers.GetPrivateProperty(m_Instance, propertyName);
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
        public object InvokePublicMethod(string methodName, params object[] args)
        {
            return ReflectionHelpers.InvokePublicMethod(m_Instance,methodName, args);
        }

        public void InvokePublicMethodVoidReturn(string methodName, params object[] args)
        {
            ReflectionHelpers.InvokePublicMethodVoidReturn(m_Instance, methodName, args);
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
