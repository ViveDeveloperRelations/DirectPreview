using System;
using System.Linq;
using System.Reflection;
using Palmmedia.ReportGenerator.Core;
using UnityEditor;
using UnityEditor.Android;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class ADBWrapper
    {
        
        [MenuItem("FOOBAR/TEST")]
        public static void SetupAndroidLogcatWithReflection()
        {
            //TODO: should this respond if hte user changes their sdk settings?
            /*
            Assembly androidExtensionsAssembly = GetAndroidExtensionsAssembly();
            if (androidExtensionsAssembly == null)
                return;
            Type androidDeviceType = androidExtensionsAssembly.GetType("UnityEditor.Android.AndroidDevice");
            if (androidDeviceType == null)
                return;
            Type adbType = androidExtensionsAssembly.GetType("UnityEditor.Android.ADB");
            if (adbType == null)
                return;
            System.Object adbInstance = adbType.GetMethod("GetInstance",BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            string adbPath = adbType.GetMethod("GetADBPath", BindingFlags.Public | BindingFlags.Instance).Invoke(adbInstance, null) as string;
            */
            AdbReflectionSetup adbReflection = new AdbReflectionSetup();
            var adbFacade = adbReflection.AdbFacade;

            Debug.Log($"ADB PATH {adbFacade.GetAdbPath()}");
            Debug.Log($"ADB  adb path {adbFacade.GetAdbPath()} {adbFacade.IsAdbAvailable()}");
            Debug.Log($"{adbFacade.Run(new[]{"devices"},"Error running adb devices")} devices");
            //Debug.Log($"{adbFacade.Run(new[]{"shell ls"},"Error running adb")} ls");

            Debug.Log($"{adbFacade.Run(new[]{"shell", "logcat", "-d"},"Error running adb222")} logcat");

/*
 //not a real error, it runs though
            try
            {
                Debug.Log($"{adbFacade.Run(new[]{"shell", "ls"},"Error running adb111")} logcat");
            }catch(Exception e)
            {
                Debug.LogException(e);
            }*/
            //Debug.Log($"{adbFacade.Run(new[]{"shell", "ls"},"Error running adb")} ls");
            CommandWrapper commandWrapper = new CommandWrapper(adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command"));
            commandWrapper.Run(adbFacade.GetAdbPath(), "devices","","Error Running Devices");
            Debug.Log("TEST BAR");
        }

        public class AdbReflectionSetup
        {
            public Assembly AndroidExtensionsAssembly;
            public Type AndroidDeviceType;
            public Type AdbType;
            public AdbFacade AdbFacade;

            public Assembly UnityEditorCoreModule;
            //public System.Object AdbInstance;
            public AdbReflectionSetup()
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
                //ProgramWrapper programWrapper = new ProgramWrapper(coreModule.GetType("UnityEditor.Utils.Program"),null);
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