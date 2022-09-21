using System.Collections;
using System.Diagnostics;
using System.Net.NetworkInformation;
using UnityEditor;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;

public class PingExample 
{
    static IEnumerator PingHostUnity(string ip,float maxTimeToRunInSeconds)
    {
        //in the editor, invalid ips seems to always fail
        var ping = new UnityEngine.Ping(ip);
        var sw = Stopwatch.StartNew();
        
        while (!ping.isDone && sw.Elapsed.TotalSeconds < maxTimeToRunInSeconds)
        {
            yield return null;
        }
        Debug.Log(ping.time);
    }

    [MenuItem("Tests/Ping Examples")]
    static void PingExamples()
    {
        //the invalid case breaks unity int he eidtor 
        var addressesToPing = new[] {"google.com", "invalid"};
        foreach (var address in addressesToPing)
        {
            Debug.Log($"testing against address {address}");

            var pingUnityEnumerator = PingHostUnity(address,maxTimeToRunInSeconds:0.5f);
            while(pingUnityEnumerator.MoveNext())
            {
                ;//block editor while waiting for ping
            }
            bool pingable = PingHost(address);
            Debug.Log("C# ping pingable: " + pingable);
        }
    }
    public static bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
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

        return pingable;
    }
}

  


