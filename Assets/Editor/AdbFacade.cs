using System;
using System.Reflection;

namespace Editor
{
    public class AdbFacade
    {
        private Type m_ADBType;
        private System.Object m_ADBInstance;
        public AdbFacade(Type adbType)
        {
            m_ADBType = adbType ?? throw new ArgumentNullException(nameof(adbType));
            m_ADBInstance = m_ADBType.GetMethod("GetInstance",BindingFlags.Public | BindingFlags.Static).Invoke(null, null) ?? throw new ArgumentNullException(nameof(m_ADBInstance));
        }

            
        public bool IsAdbAvailable()
        {
            return m_ADBType.GetMethod("IsADBAvailable",  BindingFlags.NonPublic | BindingFlags.Instance).Invoke(m_ADBInstance, null) as bool? ?? false;
        }
        public string GetAdbPath()
        {
            return m_ADBType.GetMethod("GetADBPath", BindingFlags.Public | BindingFlags.Instance).Invoke(m_ADBInstance, null) as string;
        }
            
        public string Run(string[] command,string errorMessage)
        {
            return m_ADBType.GetMethod("Run", BindingFlags.Public | BindingFlags.Instance).Invoke(m_ADBInstance, new object[] { command, errorMessage }) as string;
        }
        /*
            public string Run(string[] command,CommandWrapper.WaitingForProcessToExit onWaitDelegate, string errorMsg)
            {
                return m_ADBType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { command, onWaitDelegate, errorMsg}) as string;
            }
            */
    }
}