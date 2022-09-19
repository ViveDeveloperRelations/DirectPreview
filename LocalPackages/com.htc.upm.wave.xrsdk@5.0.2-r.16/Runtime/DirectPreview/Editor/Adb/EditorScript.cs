using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEditor;

public class EditorScript : MonoBehaviour
{
    [MenuItem("MyMenu/Do Something")]
    public static void TestOutput()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine(" \"IP\" : \"" + LocalIPAddress() + "\",");
        sb.AppendLine(" \"Port\" : 6555,");
        sb.AppendLine(" \"HMD\" : \"" + getRenderTarget() + "\",");
        sb.AppendLine(" ");
        sb.AppendLine(" \"RenderWidth\" : 1440,");
        sb.AppendLine(" \"RenderHeight\" : 1600,");
        sb.AppendLine(" \"RenderSizeScale\" : 1.0,");
        sb.AppendLine(" \"RenderOverfillScale\" : 1.3,");
        sb.AppendLine(" ");
        sb.AppendLine(" \"UseAutoPrecdictTime\" : true,");
        sb.AppendLine(" \"CtlPredictRate\" : 6,");
        sb.AppendLine(" \"HmdPredictRatio\" : 0.615,");
        sb.AppendLine(" \"CtlPredictRatio\" : 0.615,");
        sb.AppendLine(" \"HmdPredict\" : 41,");
        sb.AppendLine(" \"ControllerPredict\" : 40,");
        sb.AppendLine(" \"MaxHmdPredictTimeInMs\" : 35,");
        sb.AppendLine(" \"MaxCtlPredictTimeInMs\" : 20,");
        sb.AppendLine(" ");
        sb.AppendLine(" \"RoomHeight\" : 1.6");
        sb.AppendLine("}");
        Debug.Log(sb.ToString());
    }

    public static void Test22()
    {
        string testString1 = @"{
    ""IP"" : """+LocalIPAddress()+@""",
    ""Port"" : 6555,
    ""HMD"" : """+getRenderTarget()+@""",

    ""RenderWidth"" : 1440,
    ""RenderHeight"" : 1600,
    ""RenderSizeScale"" : 1.0,
    ""RenderOverfillScale"" : 1.3,

    ""UseAutoPrecdictTime"" : true,
    ""CtlPredictRate"" : 6,
    ""HmdPredictRatio"" : 0.615,
    ""CtlPredictRatio"" : 0.615,
    ""HmdPredict"" : 41,
    ""ControllerPredict"" : 40,
    ""MaxHmdPredictTimeInMs"" : 35,
    ""MaxCtlPredictTimeInMs"" : 20,

    ""RoomHeight"" : 1.6
}";
    }
    private static string LocalIPAddress()
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
    
    private static string getRenderTarget()
    {
        string ret = "";

        var rd = EditorPrefs.GetInt("DPTargetDevice");

        if (rd == 1)
        {
            ret = "FOCUS";
        }
        else
        {
            ret = "COSMOS";
        }

        if (ret.Equals("FOCUS"))
        {
            UnityEngine.Debug.Log("Render target is FOCUS");
        } else
        {
            UnityEngine.Debug.Log("Render target is Other");
        }

        return ret;
    }
}
