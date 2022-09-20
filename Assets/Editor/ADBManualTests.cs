
#if ENABLE_TESTS
using System.Reflection;
using DirectPreviewEditor;
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
        var adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;

        Debug.Log($"ADB PATH {adbFacade.GetAdbPath()}");
        Debug.Log($"ADB  adb path {adbFacade.GetAdbPath()} {adbFacade.IsAdbAvailable()}");
        Debug.Log($"{adbFacade.Run(new[] {"devices"}, "Error running adb devices")} devices");
        //Debug.Log($"{adbFacade.Run(new[]{"shell ls"},"Error running adb")} ls");

        Debug.Log($"{adbFacade.Run(new[] {"shell", "logcat", "-d"}, "Error running adb222")} logcat");

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
        var adbReflection = new ADBWrapper.AdbReflectionSetup();
        var commandType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command");
        if (commandType == null)
            Debug.Log("CommandType is null");
        var waitingForProcessToExitType = commandType.GetNestedType("WaitingForProcessToExit", BindingFlags.Public);
        //var waitingForProcessToExitType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command.WaitingForProcessToExit");
        if (waitingForProcessToExitType == null)
            Debug.Log("WaitingForProcessToExitType is null");
        //var waitingForProcessToExitInstance = Activator.CreateInstance(waitingForProcessToExitType);
        var commandWrapper =
            new CommandWrapper(adbReflection.AndroidExtensionsAssembly, adbReflection.UnityEditorCoreModule);
        //ambiguous run methods... need to parse those out more carefully :/
        //commandWrapper.Run(adbReflection.AdbFacade.GetAdbPath(), "devices", "", "Error Running Devices");
    }

    [MenuItem("FOOBAR/Test ADB Devices")]
    public static void TestADBDevices()
    {
        var adbReflection = new ADBWrapper.AdbReflectionSetup();

        var adbFacade = adbReflection.AdbFacade;
        //Debug.Log($"{adbFacade.Run(new[]{"devices"},"Error running adb devices")} devices");
        Debug.Log($"is adb available {adbFacade.IsAdbAvailable()} ");
        Debug.Log($"adb path {adbFacade.GetAdbPath()} ");
        var processStatus = adbFacade.GetADBProcessStatus();
        if (processStatus != null)
        {
            var status = processStatus.Status();
            Debug.Log($"ADB Process Status {status.ToString()}");
            var processes = processStatus.Processes();
            if (processes != null)
                foreach (var adbProcFacade in processes)
                {
                    var process = adbProcFacade.Process();
                    Debug.Log(
                        $"Process id {process.Id} {process.ProcessName} {process.StartTime} fullPath {adbProcFacade.FullPath()} external {adbProcFacade.External()}");
                }
        }
        else
        {
            Debug.Log("process status is null");
        }

        Debug.Log($"{adbFacade.Run(new[] {"devices"}, "Error running adb devices")} devices");

        //string shellCommand = @"adb shell ""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
        var shellCommand = @"""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
        var ip = adbFacade.Run(new[] {"shell", shellCommand}, "error running get ip");
        Debug.Log($"Headset ip address {ip}");
    }

    [MenuItem("FOOBAR/Test ADB DeviceCount")]
    public static void AdbDeviceCount()
    {
        var adbReflection = new ADBWrapper.AdbReflectionSetup();

        var adbFacade = adbReflection.AdbFacade;
        Debug.Log($"Is one device connected {adbFacade.OneDeviceConnected()}");
    }
}
#endif // ENABLE_TESTS