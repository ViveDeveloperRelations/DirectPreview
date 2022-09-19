using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using Wave.XR.DirectPreview.Editor;
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
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = rr_exe,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
        };
        return new UniqueNamedProcessPerUnityRun("REMOTE_RENDERING_SERVER",startInfo);;
    }
    public static void StartDirectPreview(DirectPreviewUnityStateVersion1 mDirectPreviewState)
    {
        var rr_server = RemoteRenderingServer();
        
    }
}
