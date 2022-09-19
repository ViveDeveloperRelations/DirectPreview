using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
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
            CreateNoWindow = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
        };
        return new UniqueNamedProcessPerUnityRun("REMOTE_RENDERING_SERVER",startInfo);;
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
