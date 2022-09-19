using System;
using System.Diagnostics;
using System.Reflection;
using DirectPreview.Utility;

namespace Editor
{
    public class AdbFacade
    {
        private Type m_ADBType;
        private ReflectionInstanceHelper m_AdbInstance;
        private Type m_AdbStatusType;
        private Type m_AdbProcType;

        public AdbFacade(Type adbType)
        {
            m_ADBType = adbType ?? throw new ArgumentNullException(nameof(adbType));
            Object newInstanceObject = ReflectionHelpers.InvokePublicStaticMethod(m_ADBType, "GetInstance", null) ??
                                       throw new Exception("Failed to get ADB instance");
            m_AdbInstance = new ReflectionInstanceHelper(m_ADBType, newInstanceObject) ??
                            throw new Exception("Failed to get ADB instance");
            m_AdbStatusType = m_ADBType.Assembly.GetType("UnityEditor.Android.ADB+ADBStatus") ??
                              throw new Exception("Failed to get ADBStatus type");
            m_AdbProcType = m_ADBType.Assembly.GetType("UnityEditor.Android.ADB+ADBProc") ?? //does this need an inner class lookup instead?
                            throw new Exception("Failed to get ADBProcess type");
        }

        public bool IsAdbAvailable()
        {
            return Convert.ToBoolean(m_AdbInstance.InvokePrivateMethod("IsADBAvailable"));
        }

        public string GetAdbPath()
        {
            return m_AdbInstance.InvokePublicMethod("GetADBPath") as string;
        }

        public string Run(string[] command, string errorMessage)
        {
            return m_AdbInstance.InvokePublicMethod("Run", command, errorMessage) as string;
        }
        /*
            public string Run(string[] command,CommandWrapper.WaitingForProcessToExit onWaitDelegate, string errorMsg)
            {
                return m_ADBType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { command, onWaitDelegate, errorMsg}) as string;
            }
            */
        
        public AdbStatusFacade GetADBProcessStatus()
        {
            return new AdbStatusFacade(m_AdbStatusType,m_AdbInstance.InvokePrivateMethod("GetADBProcessStatus"));
        }
    }
    public class ADBProcFacade
    {
        private readonly ReflectionInstanceHelper m_AdbProcFacade;

        public ADBProcFacade(Type adbProcType, Object adbProcObject)
        {
            if(adbProcType == null) throw new ArgumentNullException(nameof(adbProcType));
            if(adbProcObject == null) throw new ArgumentNullException(nameof(adbProcObject));
            m_AdbProcFacade = new ReflectionInstanceHelper(adbProcType,adbProcObject);
        }
        public Process Process()
        {
            return m_AdbProcFacade.GetPublicField("procHandle") as Process;
        }
        public string FullPath()
        {
            return m_AdbProcFacade.GetPublicField("fullPath") as string;
        }
        public bool External()
        {
            return Convert.ToBoolean(m_AdbProcFacade.GetPublicField("external"));
        }            
    }

    public enum ADBProcessStatus
    {
        Offline,
        External,
        MultiInstance,
        Online,
    }

    public class AdbStatusFacade //adb status is a struct have to handle this differently
    {
        private readonly Type m_AdbStatusType;
        private readonly ReflectionInstanceHelper m_AdbStatusInstance;
        private readonly Type m_AdbProcType;
        private readonly Type m_AdbProcessStatusType;

        public AdbStatusFacade(Type adbStatusType,Object adbStatusObject)
        {
            m_AdbStatusType = adbStatusType ?? throw new ArgumentNullException(nameof(adbStatusType));
            m_AdbStatusInstance = new ReflectionInstanceHelper(adbStatusType,adbStatusObject) ??
                                  throw new Exception("Failed to get ADBStatus instance");
            //FIXME: pass this in 
            m_AdbProcType = m_AdbStatusType.Assembly.GetType("UnityEditor.Android.ADB+ADBProc") ?? //does this need an inner class lookup instead?
                            throw new Exception("Failed to get ADBProcess type");
        }

        public ADBProcFacade[] Processes()
        {
            object[] processes = m_AdbStatusInstance.GetPublicField("processes") as object[];
            if (processes == null)
            {
                //check that this happens when it should
                return null;
            }
            var facades = new ADBProcFacade[processes.Length];
            for (int i = 0; i < processes.Length; i++)
            {
                facades[i] = new ADBProcFacade(m_AdbProcType, processes[i]);
            }
            return facades;
        }

        public ADBProcessStatus Status()
        {
            var statusRawReturn = m_AdbStatusInstance.GetPublicPropertyObject("status");
            Enum.TryParse(statusRawReturn.ToString(), out ADBProcessStatus status);
            return status;
            /*
            int intValue = statusRawReturn is int ? (int) statusRawReturn : -1;
            var status = statusRawReturn;
            
            if (status == null)
            {
                throw new Exception("Internal exception invalid property type");
            }
            return (ADBProcessStatus) status;
            */
            //return Convert.ChangeType(status, typeof(ADBProcessStatus));
            //return m_AdbStatusInstance.GetPublicField("status") as ProcessStatus? ?? ProcessStatus.Offline;
        }

    }
}