//#define ENABLE_TESTS
#if ENABLE_TESTS

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Wave.XR.DirectPreview.Editor;

public class DirectPreviewMenuItemTests : MonoBehaviour
{
    [MenuItem("DirectPreviewTest/Start")]
    public static void StartTest()
    {
        DirectPreviewHelper.StartRemoteRenderer();
    }
    [MenuItem("DirectPreviewTest/DumpLogs")]
    public static void LogTest()
    {
        DirectPreviewHelper.RemoteRenderingDumpLogsTest();
    }

    [MenuItem("DirectPreviewTest/Stop")]
    public static void Stop()
    {
        DirectPreviewHelper.StopRemoteRenderingServer();
    }
    [MenuItem("DirectPreviewTest/InstallAndRunAPK")]
    public static void InstallAndRunAPK()
    {
        DirectPreviewHelper.InstallAndStartAPK();
    }
    [MenuItem("DirectPreviewTest/ResetHeadsetIP")]
    public static void ResetHeadsetIP()
    {
        var state = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
        state.DeviceWifiAddress = "";
        DirectPreviewUnityStateStore.Store(state);
    }
}
#endif