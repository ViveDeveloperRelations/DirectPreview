﻿using System;
using System.Diagnostics;
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
        
        //[MenuItem("FOOBAR/TEST")]
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
            
            //CommandWrapper commandWrapper = new CommandWrapper(adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command"));
            //commandWrapper.Run(adbFacade.GetAdbPath(), "devices","","Error Running Devices");
            Debug.Log("TEST BAR");
        }
        //[MenuItem("FOOBAR/CommandTypeTest")]
        public static void TestCommandType()
        {
            AdbReflectionSetup adbReflection = new AdbReflectionSetup();
            var commandType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command");
            if(commandType == null)
                Debug.Log("CommandType is null");
            Type waitingForProcessToExitType = commandType.GetNestedType("WaitingForProcessToExit", BindingFlags.Public);
            //var waitingForProcessToExitType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command.WaitingForProcessToExit");
            if(waitingForProcessToExitType == null)
                Debug.Log("WaitingForProcessToExitType is null");
            //var waitingForProcessToExitInstance = Activator.CreateInstance(waitingForProcessToExitType);
            var commandWrapper = new CommandWrapper(adbReflection.AndroidExtensionsAssembly,adbReflection.UnityEditorCoreModule);
            //ambiguous run methods... need to parse those out more carefully :/
            //commandWrapper.Run(adbReflection.AdbFacade.GetAdbPath(), "devices", "", "Error Running Devices");
        }
        [MenuItem("FOOBAR/Test ADB Devices")]
        public static void TestADBDevices()
        {
            AdbReflectionSetup adbReflection = new AdbReflectionSetup();

            var adbFacade = adbReflection.AdbFacade;
            //Debug.Log($"{adbFacade.Run(new[]{"devices"},"Error running adb devices")} devices");
            Debug.Log($"is adb available {adbFacade.IsAdbAvailable()} ");
            Debug.Log($"adb path {adbFacade.GetAdbPath()} ");
            AdbStatusFacade processStatus = adbFacade.GetADBProcessStatus();
            if (processStatus != null)
            {
                ADBProcessStatus status = processStatus.Status();
                Debug.Log($"ADB Process Status {status.ToString()}");
                var processes = processStatus.Processes();
                if (processes != null)
                {
                    foreach (ADBProcFacade adbProcFacade in processes)
                    {
                        var process = adbProcFacade.Process();
                        Debug.Log($"Process id {process.Id} {process.ProcessName} {process.StartTime} fullPath {adbProcFacade.FullPath()} external {adbProcFacade.External()}");
                    }
                }
            }
            else
            {
                Debug.Log("process status is null");
            }
            Debug.Log($"{adbFacade.Run(new[]{"devices"},"Error running adb devices")} devices");

            //string shellCommand = @"adb shell ""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
            var shellCommand = @"""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
            var ip = adbFacade.Run(new []{"shell",shellCommand}, "error running get ip");
            Debug.Log($"Headset ip address {ip}");
        }

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