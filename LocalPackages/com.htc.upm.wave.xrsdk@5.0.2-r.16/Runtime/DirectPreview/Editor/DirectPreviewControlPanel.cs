using System;
using System.Diagnostics;
using System.Linq;
using DirectPreview.Utility;
using DirectPreviewEditor;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.XR.Management;
using Debug = UnityEngine.Debug;

namespace Wave.XR.DirectPreview.Editor
{
    //TODO: should these settings be per-project or per user?
    public class DirectPreviewControlPanel : EditorWindow
    {
        private bool configFoldout = true;
		
        DirectPreviewUnityStateVersion1 m_DirectPreviewState;

        void ShowConfig()
        {
            if (m_DirectPreviewState == null)
                m_DirectPreviewState = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
	        
	        
            m_DirectPreviewState.ConnectType = (DirectPreviewUnityStateVersion1.ConnectTypeEnum)EditorGUILayout.EnumPopup("Connection Type", m_DirectPreviewState.ConnectType);
	        
            if (m_DirectPreviewState.ConnectType == DirectPreviewUnityStateVersion1.ConnectTypeEnum.USB)
            {
                EditorGUILayout.LabelField("Use USB to get data (Pose/Event/...) from device\n" +
                                           "Note: HMD will NOT show images.", GUILayout.Height(40));
            }
            else
            {
                ShowWifiGUI();
            }
            DirectPreviewUnityStateStore.Store(m_DirectPreviewState);
        }

        public static bool HeadsetReachable(string wifiAddress)
        {
            bool canReach = false;
            try
            {
                canReach = DirectPreviewHelper.PingHost( wifiAddress);
            }catch{Debug.Log("PingHost exception");}
            return canReach;
        }
        
        void ShowWifiGUI()
        {
            EditorGUILayout.LabelField("Use Wi-Fi to get data from device and show images on HMD.\n" +
                                       "Suggest to use 5G Wi-Fi to get better performance.", GUILayout.Height(40));
            GUI.enabled = false;
            m_DirectPreviewState.DeviceWifiAddress = EditorGUILayout.TextField("Device Wi-Fi IP: ", m_DirectPreviewState.DeviceWifiAddress).Trim();
            GUI.enabled = true;
            if(GUILayout.Button("Get IP from headset (if connected through unity)"))
            {
                try
                {
                    m_DirectPreviewState.DeviceWifiAddress = DirectPreviewHelper.DeviceIPAddress();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    ShowNotification(new GUIContent("Failed to get IP from headset"));
                }
            }

            if(GUILayout.Button("Test reachability of headset"))
            {
                bool canReach = HeadsetReachable(m_DirectPreviewState.DeviceWifiAddress);
                ShowNotification(new GUIContent(canReach ? "Reachable" : "Not reachable"));
            }

            //m_DirectPreviewState.DllTraceLogToFile = EditorGUI.Toggle(new Rect(0, 100, position.width, 20), "Save log to file", m_DirectPreviewState.DllTraceLogToFile);

            m_DirectPreviewState.EnablePreviewImage = EditorGUILayout.Toggle("Enable preview image: ", m_DirectPreviewState.EnablePreviewImage);
	        

            if (m_DirectPreviewState.EnablePreviewImage)
            {
                PreviewGUI();
            }
        }
        void PreviewGUI()
        {
            var fps_pairs = DirectPreviewUnityStateVersion1.FPSOption.FPS_Pairs;
	        
            m_DirectPreviewState.FPS.FPSOptionInt = EditorGUILayout.IntPopup("Update frequency: ", m_DirectPreviewState.FPS.FPSOptionInt, DirectPreviewUnityStateVersion1.FPSOption.FPS_Printable_Names, DirectPreviewUnityStateVersion1.FPSOption.FPS_Serialized_Int_Values);
	        
            m_DirectPreviewState.DeviceType = (DirectPreviewUnityStateVersion1.DeviceTypeEnum)EditorGUILayout.EnumPopup("Render target device: ", m_DirectPreviewState.DeviceType);
	        
            EditorGUILayout.LabelField("Please re-install Device APK if changed target device.");
	        
            m_DirectPreviewState.TargetSizeRatio.TargetSizeRatioInt = EditorGUILayout.IntPopup("Preview image ratio: ", m_DirectPreviewState.TargetSizeRatio.TargetSizeRatioInt, DirectPreviewUnityStateVersion1.TargetSizeRatioOption.TargetSizeStrings, DirectPreviewUnityStateVersion1.TargetSizeRatioOption.TargetSizeValues);
            
            m_DirectPreviewState.OutputImageToFile = EditorGUILayout.Toggle("Regularly save images: ", m_DirectPreviewState.OutputImageToFile);
        }
        public void UnfocusWindow()
        {
            if(hasFocus)
                GUI.FocusControl(null);
        }
        bool showRenderingServerLogs = false;
        void ShowRemoteRenderingLogs()
        {
            //for some reason we can't get stdout from the process but we can get a pointer to it
            // likely there's something obivious I'm missing
            // but it does seem the process is running and not in a zombie state, but just throws errors if you try to access stdout + stderr between reloads of the appdomain
            showRenderingServerLogs = EditorGUILayout.Foldout(showRenderingServerLogs, "Show rendering server logs");

            if (showRenderingServerLogs)
            {
                var runningProcess = DirectPreviewHelper.RemoteRenderingServer();
                string textOutput = "";
                string textErrors = "";
                try
                {
                    var process = runningProcess.GetProcess();
                    if (process != null)
                    {
                        process.Start(); //not sure why this process needs to be 'started', but here we are
                        textOutput = process.StandardOutput.ReadToEnd();
                        textErrors = process.StandardError.ReadToEnd();
                    }
                }
                catch(Exception e)
                {
                    Debug.Log("Failed to get RR Process");
                    Debug.LogException(e);
                }
                GUILayout.Label("Output:");
                EditorGUILayout.TextArea(textOutput,GUILayout.Height(100));
                GUILayout.Label("Errors:");
                EditorGUILayout.TextArea(textErrors,GUILayout.Height(50));
            }
        }

        void KillAllRemoteRenderingServersButton()
        {
            if (GUILayout.Button("Kill all rendering servers, useful for debugging"))
            {
                new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "taskkill",
                        Arguments = "/F /IM dpServer.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                }.Start();
            }
        }
        bool allGUIButtonsFoldout = false;
        void ShowButtons()
        {
            if(GUILayout.Button("Start Direct Preview -- beta"))
            {
                DirectPreviewHelper.StartDirectPreview(m_DirectPreviewState);
            }

            //ShowRemoteRenderingLogs(); //still alpha
            
            allGUIButtonsFoldout = EditorGUILayout.Foldout(allGUIButtonsFoldout, "All GUI Buttons For debugging");
            if (!allGUIButtonsFoldout) return;

            UniqueNamedProcessPerUnityRun runningProcess = DirectPreviewHelper.RemoteRenderingServer();
            if (!runningProcess.IsRunningHelperTest())
            {
                if (GUILayout.Button("Start streaming server"))
                {
                    //StreamingServer.StartStreamingServer();
                    DirectPreviewHelper.RemoteRenderingServer().Start();
                }
                KillAllRemoteRenderingServersButton();
            }
            else
            {
                if (GUILayout.Button("Stop streaming server"))
                {
                    DirectPreviewHelper.RemoteRenderingServer().Stop();
                }
                if(GUILayout.Button("Test Dump info from reference"))
                {
                    TestDumpInfoFromReference(runningProcess);
                }

                KillAllRemoteRenderingServersButton();
            }
            
            if(GUILayout.Button("Re-install and run apk"))
            {
                DirectPreviewHelper.InstallAndStartAPK();
            }
        }

        private void TestDumpInfoFromReference(UniqueNamedProcessPerUnityRun runningProcess)
        {
            var process = runningProcess.GetProcess();
            if (process != null)
            {
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                Debug.Log("11Output:" + output);
                Debug.Log("11Error:" + error);
            }
        }

        static bool IsBuildingAgainstPlatforms()
        {
            //FIXME: set up the asmdefs so that the refleciton isn't needed
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName.Contains("Wave.XRSDK.Editor"));
            var waveQueryXRSettings = assembly.GetType("WaveQueryXRSettings"); //todo: namespace aka Wave.XRSDK.Editor.
            bool buildAndroid = Convert.ToBoolean( ReflectionHelpers.InvokePublicStaticMethod(waveQueryXRSettings,"CheckIsBuildingWaveAndroid"));
            bool buildPC = Convert.ToBoolean( ReflectionHelpers.InvokePublicStaticMethod(waveQueryXRSettings,"CheckIsBuildingWaveStandalone"));
            return buildAndroid && buildPC;
        }
        static void SetWaveBuildingAgainstPlatforms()
        {
            //FIXME: set up the asmdefs so that the refleciton isn't needed
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName.Contains("Wave.XRSDK.Editor"));
            var waveQueryXRSettings = assembly.GetType("WaveQueryXRSettings"); //todo: namespace aka Wave.XRSDK.Editor.
            ReflectionHelpers.InvokePublicStaticMethod(waveQueryXRSettings,"AddIsBuildingWaveAndroid");
            ReflectionHelpers.InvokePublicStaticMethod(waveQueryXRSettings,"AddIsBuildingWaveStandalone");
        }
        void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Application is Playing\n" + "Before any DirectPreview operation, please stop playing.", MessageType.None);
                return;
            }
            if (!IsBuildingAgainstPlatforms())
            {
                EditorGUILayout.HelpBox("Please enable wave xr build options in the settings", MessageType.None);
                if(GUILayout.Button("Enable Wave XR Build Options"))
                {
                    SetWaveBuildingAgainstPlatforms();
                }
                return;
            }
            if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorGUILayout.HelpBox("Please switch to Android build target", MessageType.None);
                if(GUILayout.Button("Switch to Android build target"))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }
                return;
            }
            
            configFoldout = EditorGUILayout.Foldout(configFoldout, "Config");
            if (configFoldout)
            {
                EditorGUI.indentLevel++;
                ShowConfig();
                EditorGUI.indentLevel--;
            }

            ShowButtons();

            this.Repaint();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        //use call menu item? or just reflection
    }
}