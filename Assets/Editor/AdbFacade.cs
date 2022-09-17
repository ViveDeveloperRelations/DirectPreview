using System;
using System.Reflection;
using DirectPreview.Utility;

namespace Editor
{
    public class AdbFacade
    {
        private Type m_ADBType;
        private ReflectionInstanceHelper m_AdbInstance;
        public AdbFacade(Type adbType)
        {
            m_ADBType = adbType ?? throw new ArgumentNullException(nameof(adbType));
            Object newInstanceObject = ReflectionHelpers.InvokePublicStaticMethod(m_ADBType, "GetInstance", null) ??
                                       throw new Exception("Failed to get ADB instance");
            m_AdbInstance = new ReflectionInstanceHelper(m_ADBType,newInstanceObject);
        }
        public bool IsAdbAvailable()
        {
            return Convert.ToBoolean(m_AdbInstance.InvokePublicMethod("IsAdbAvailable"));
        }
        public string GetAdbPath()
        {
            return m_AdbInstance.InvokePublicMethod("GetADBPath") as string;
        }
            
        public string Run(string[] command,string errorMessage)
        {
            return m_AdbInstance.InvokePublicMethod("Run",command,errorMessage) as string;
        }
        /*
            public string Run(string[] command,CommandWrapper.WaitingForProcessToExit onWaitDelegate, string errorMsg)
            {
                return m_ADBType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { command, onWaitDelegate, errorMsg}) as string;
            }
            */
    }
}