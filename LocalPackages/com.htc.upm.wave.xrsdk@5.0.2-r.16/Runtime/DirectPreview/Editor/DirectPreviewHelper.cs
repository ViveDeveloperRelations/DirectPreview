using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using DirectPreviewEditor;
using UnityEditor;
using Wave.XR.DirectPreview.Editor;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;

public class DirectPreviewHelper
{
    public static bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
        try
        {
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

        }
        catch
        {
        } //usually an invalid address will cause an exception in dispose

        return pingable;
    }

    public static UniqueNamedProcessPerUnityRun RemoteRenderingServer()
    {
        var rr_path = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary");
        var rr_exe = Path.Combine(rr_path, "dpServer.exe");
        if (!File.Exists(rr_exe))
        {
            UnityEngine.Debug.LogError("DirectPreview server not found");
            throw new Exception("DirectPreview server not found");
        }
        
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = rr_exe,
            UseShellExecute = false,
            WorkingDirectory = rr_path,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
        };
        return new UniqueNamedProcessPerUnityRun("REMOTE_RENDERING_SERVER", startInfo);
    }

    private static string AdbRunCommand(AdbFacade adbFacade,string command, string errorMessage, bool throwOnFail)
    {
        try
        {
            return adbFacade.Run(new[] {command}, errorMessage);
        }
        catch
        {
            Debug.Log($"{command} failed");
            if (throwOnFail) throw;
            return string.Empty;
        }
    }
    
    public static void InstallAndStartAPK()
    {
        
        var absolutePath =
            Path.GetFullPath(
                "Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/wvr_plugins_directpreview_agent_unity.apk");
        if (!File.Exists(absolutePath)) throw new Exception("DirectPreview agent apk not found");
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;

        
        var commands = new[]
        {
            $"install -r -g -d \"{absolutePath}\"",
            $"shell am start -n com.htc.vr.directpreview.agent.unity/com.vive.rrclient.RRClient"
        };
        

        foreach (var startupCommand in commands)
        {
            AdbRunCommand(adbFacade,startupCommand, "Failed to run command to start remote rendering" + startupCommand, true);
        }

        //adb shell am force-stop com.htc.vr.directpreview.agent.unity
        //adb shell am kill com.htc.vr.directpreview.agent.unity
        //adb shell am broadcast -a com.htc.vr.directpreview.agent.SHUTDOWN
        //uninstall com.htc.vr.directpreview.agent.unity
        //var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/wvr_plugins_directpreview_agent_unity.apk");
        //adb install -r -g \"" + absolutePath + "\""
        //adb shell am start -n com.htc.vr.directpreview.agent.unity/com.vive.rrclient.RRClient
    }

    private static void StopAndUninstallOldAPK()
    {
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;
        const string apkPackageName = "com.htc.vr.directpreview.agent.unity";
        var canFailShutdownCommands = new[]
        {
            $"shell am force-stop {apkPackageName}",
            $"shell am kill {apkPackageName}", //
            $"shell am broadcast -a {apkPackageName}.SHUTDOWN", //double check fails - StopSimulatorInner2
            $"uninstall {apkPackageName}",
        };
        foreach (var canFailShutdownCommand in canFailShutdownCommands)
        {
            AdbRunCommand(adbFacade,canFailShutdownCommand, "", false);
        }
    }

    private static string DeviceIPAddress()
    {
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;
        var shellCommand = @"""ip addr show wlan0  | grep 'inet ' | cut -d ' ' -f 6 | cut -d / -f 1""";
        var deviceIP = adbFacade.Run(new[] {"shell", shellCommand}, "error running get ip");
        return deviceIP;
    }
    public static void StartDirectPreview(DirectPreviewUnityStateVersion1 directPreviewState)
    {
        StopAndUninstallOldAPK();

        
        if (directPreviewState.ConnectType == DirectPreviewUnityStateVersion1.ConnectTypeEnum.WIFI)
        {
            var deviceIPAddress = DeviceIPAddress();
            if(directPreviewState.DeviceWifiAddress != deviceIPAddress) //should this silently correct like this or do something else?
            {
                DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
                window.UnfocusWindow(); // unfocus from other items, as this can prevent the ip text field from updating
                var oldIP = directPreviewState.DeviceWifiAddress ?? string.Empty;
                directPreviewState.DeviceWifiAddress = deviceIPAddress.Trim();
                Debug.Log("Auto fixed the ip address of the device from " + oldIP + " to " + deviceIPAddress);
                DirectPreviewUnityStateStore.Store(directPreviewState);
            }
            OnDeviceJsonConfig.WriteConfig(OnDeviceJsonConfig.RemoteRenderingDeviceIPAddress(), directPreviewState.DeviceType);
        }
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;
        adbFacade.Run(new[] {"push","\""+ OnDeviceJsonConfig.JsonConfigPath() +"\" /sdcard/DirectPreview/config.json"}, "Failed to push json config file");
        
        //TODO: test that the ports can be acquired at all and look for failure states
        var rr_server = RemoteRenderingServer();
        //rr_server.Stop();
        rr_server.Start();
        
        InstallAndStartAPK(); //todo: try catch with an editor popup dialog with information on failure
    }

    public static void StartRemoteRenderer()
    {
        var rr_server = RemoteRenderingServer();
        rr_server.Start();
    }

    
    public static void RemoteRenderingDumpLogsTest()
    {
        var rr_server = RemoteRenderingServer();
        var process = rr_server.GetProcess();
        if (process != null)
        {
            if (process.HasExited)
            {
                UnityEngine.Debug.Log("Process has exited");
            }
            else
            {
                UnityEngine.Debug.Log("Process is running");
            }

            UnityEngine.Debug.Log($"stdout; {process.StandardOutput.ReadToEnd()}");
            UnityEngine.Debug.Log($"stderr: {process.StandardError.ReadToEnd()}");
        }
        else
        {
            Debug.Log("Couldn't find process");
        }
    }
    
    public static void StopRemoteRenderingServer()
    {

        var rr_server = RemoteRenderingServer();
        var process = rr_server.GetProcess();
        if (process != null)
        {
            UnityEngine.Debug.Log($"stdout; {process.StandardOutput.ReadToEnd()}");
            UnityEngine.Debug.Log($"stderr: {process.StandardError.ReadToEnd()}");
        }

        rr_server.Stop();
    }

    public static class OnDeviceJsonConfig
    {
        public static string RemoteRenderingDeviceIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        //FIXME: test idiomatic json serialization techniques and decouple the file
        public static void WriteConfig(string remoteRenderingServerIP,DirectPreviewUnityStateVersion1.DeviceTypeEnum deviceType)
        {
            string fileName = "";
            fileName = JsonConfigPath();

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var sr = File.CreateText(fileName);

            sr.WriteLine("{");
            sr.WriteLine(" \"IP\" : \"" + remoteRenderingServerIP + "\",");
            sr.WriteLine(" \"Port\" : 6555,");
            var deviceTypeStringForJson = deviceType == DirectPreviewUnityStateVersion1.DeviceTypeEnum.Others ? "COSMOS" : "FOCUS";
            sr.WriteLine(" \"HMD\" : \"" + deviceTypeStringForJson + "\",");
            sr.WriteLine(" ");
            sr.WriteLine(" \"RenderWidth\" : 1440,");
            sr.WriteLine(" \"RenderHeight\" : 1600,");
            sr.WriteLine(" \"RenderSizeScale\" : 1.0,");
            sr.WriteLine(" \"RenderOverfillScale\" : 1.3,");
            sr.WriteLine(" ");
            sr.WriteLine(" \"UseAutoPrecdictTime\" : true,"); //FIXME: spelling
            sr.WriteLine(" \"CtlPredictRate\" : 6,");
            sr.WriteLine(" \"HmdPredictRatio\" : 0.615,");
            sr.WriteLine(" \"CtlPredictRatio\" : 0.615,");
            sr.WriteLine(" \"HmdPredict\" : 41,");
            sr.WriteLine(" \"ControllerPredict\" : 40,");
            sr.WriteLine(" \"MaxHmdPredictTimeInMs\" : 35,");
            sr.WriteLine(" \"MaxCtlPredictTimeInMs\" : 20,");
            sr.WriteLine(" ");
            sr.WriteLine(" \"RoomHeight\" : 1.6");
            sr.WriteLine("}");
            sr.Close();
        }

        public static string JsonConfigPath()
        {
            var absolutePath =
                Path.GetFullPath(
                    "Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/DirectPreviewConfig.json");
            UnityEngine.Debug.Log("configPath = " + absolutePath);

            return absolutePath;
        }
    }
    
}
