using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Palmmedia.ReportGenerator.Core;
using UnityEditor;
using UnityEditor.Android;
using Debug = UnityEngine.Debug;

namespace DirectPreviewEditor
{
    public class ADBWrapper
    {
        public static string GetConnectedHeadsetIP()
        {
            AdbReflectionSetup adbReflection = new AdbReflectionSetup();

            var shellCommand = @"""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
            var adbFacade = adbReflection.AdbFacade;
            var ip = adbFacade.Run(new []{"shell",shellCommand}, "error running get ip");
            return ip;
        }


        //scratchpad for prerequisites for wrapping the adb class "UnityEditor.Android.ADB" inside of "UnityEditor.Android.Extensions"
        public class AdbReflectionSetup
        {
            public Assembly AndroidExtensionsAssembly;
            public Type AndroidDeviceType;
            public Type AdbType;
            public AdbFacade AdbFacade;

            public Assembly UnityEditorCoreModule;
            //public System.Object AdbInstance;
            public SETUP_STATUS SetupStatus;
            public enum SETUP_STATUS
            {
                UNINITIALIZED,
                FAILED,
                SUCCESS
            }
            public AdbReflectionSetup()
            {
                try
                {
                    AndroidExtensionsAssembly = GetAndroidExtensionsAssembly();
                    if (AndroidExtensionsAssembly == null)
                        return;
                    AndroidDeviceType = AndroidExtensionsAssembly.GetType("UnityEditor.Android.AndroidDevice");
                    if (AndroidDeviceType == null)
                        return;
                    AdbType = AndroidExtensionsAssembly.GetType("UnityEditor.Android.ADB");
                    if (AdbType == null)
                        return;
                    AdbFacade = new AdbFacade(AdbType);
                    UnityEditorCoreModule = GetUnityEditorCoreModuleAssembly();
                    SetupStatus = UnityEditorCoreModule != null && AdbType != null && AndroidDeviceType != null ? SETUP_STATUS.SUCCESS : SETUP_STATUS.FAILED;
                }catch{
                    SetupStatus = SETUP_STATUS.FAILED;
                }
                //ProgramWrapper programWrapper = new ProgramWrapper(coreModule.GetType("UnityEditor.Utils.Program"),null);
                SetupStatus = SETUP_STATUS.SUCCESS;
            }
            private Assembly GetAndroidExtensionsAssembly()
            {
                var assemblyName = "UnityEditor.Android.Extensions";
                return GetAssembly(assemblyName);
            }
            private Assembly GetUnityEditorCoreModuleAssembly()
            {
                var assemblyName = "UnityEditor.CoreModule";
                return GetAssembly(assemblyName);
            }
            private Assembly GetAssembly(string assemblyName)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName.Contains(assemblyName));
                return assembly;
            }
            
        }
        
        
    }
}