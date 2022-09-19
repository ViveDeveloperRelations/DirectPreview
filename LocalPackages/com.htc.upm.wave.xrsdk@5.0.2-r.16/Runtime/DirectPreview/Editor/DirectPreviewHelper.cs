using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using Editor;
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

    private static UniqueNamedProcessPerUnityRun RemoteRenderingServer()
    {
        var rr_path = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary");
        var rr_exe = Path.Combine(rr_path, "dpServer.exe");
        if(!File.Exists(rr_exe))
        {
            UnityEngine.Debug.LogError("DirectPreview server not found");
            throw new Exception("DirectPreview server not found");
        }
        Debug.Log("DirectPreview server found at " + rr_exe);
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = rr_exe,
            UseShellExecute = false,
            WorkingDirectory = rr_path,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
        };
        return new UniqueNamedProcessPerUnityRun("REMOTE_RENDERING_SERVER",startInfo);;
    }
    [MenuItem("FOOBAR/TESTSTART")]
    private static void InstallAndStartAPK()
    {
        const string apkPackageName = "com.htc.vr.directpreview.agent.unity";
        var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/wvr_plugins_directpreview_agent_unity.apk");
        if(!File.Exists(absolutePath)) throw new Exception("DirectPreview agent apk not found");
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var adbFacade = adbReflection.AdbFacade;
        
        
        var canFailShutdownCommands = new[]
        {
            $"shell am force-stop {apkPackageName}",
            $"shell am kill {apkPackageName}", //
            $"shell am broadcast -a {apkPackageName}.SHUTDOWN", //double check fails - StopSimulatorInner2
            $"uninstall {apkPackageName}",
        };
        var commands = new[]
        {
            $"install -r -g -d \"{absolutePath}\"",
            $"shell am start -n com.htc.vr.directpreview.agent.unity/com.vive.rrclient.RRClient"
        };
        Action<string,string,bool> runCommand = (string command,string errorMessage,bool throwOnFail) => {
            try
            {
                adbFacade.Run(new[] {command}, errorMessage);
            }
            catch
            {
                Debug.Log($"{command} failed");
                if(throwOnFail) throw;
            }
        };
        foreach (var canFailShutdownCommand in canFailShutdownCommands)
        {
            runCommand(canFailShutdownCommand,"",false);
        }
        foreach (var startupCommand in commands)
        {
            runCommand(startupCommand,"Failed to run command to start remote rendering"+startupCommand,true);
        }
        
        //adb shell am force-stop com.htc.vr.directpreview.agent.unity
        //adb shell am kill com.htc.vr.directpreview.agent.unity
        //adb shell am broadcast -a com.htc.vr.directpreview.agent.SHUTDOWN
        //uninstall com.htc.vr.directpreview.agent.unity
        //var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/wvr_plugins_directpreview_agent_unity.apk");
        //adb install -r -g \"" + absolutePath + "\""
        //adb shell am start -n com.htc.vr.directpreview.agent.unity/com.vive.rrclient.RRClient
    }
    
    public static void StartDirectPreview(DirectPreviewUnityStateVersion1 mDirectPreviewState)
    {
        //TODO: test that the ports can be acquired at all and look for failure states
        var rr_server = RemoteRenderingServer();
        rr_server.Start();
        
    }
    [MenuItem("ffoo/Start")]
    public static void StartTest()
    {
        var rr_server = RemoteRenderingServer();
        rr_server.Start();
    }
    
    [MenuItem("ffoo/Logs")]
    public static void LogTest()
    {
        var rr_server = RemoteRenderingServer();
        var process = rr_server.GetProcess();
        if(process != null)
        {
            if(process.HasExited)
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
    [MenuItem("ffoo/Stop")]
    public static void StopTest()
    {
        
        var rr_server = RemoteRenderingServer();
        var process = rr_server.GetProcess();
        if(process != null)
        {
            UnityEngine.Debug.Log($"stdout; {process.StandardOutput.ReadToEnd()}");
            UnityEngine.Debug.Log($"stderr: {process.StandardError.ReadToEnd()}");
        }
        rr_server.Stop();
    }
}
