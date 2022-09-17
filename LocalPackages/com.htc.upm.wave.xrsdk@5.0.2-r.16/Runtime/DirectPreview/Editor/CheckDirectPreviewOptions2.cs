﻿
// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEditor;
using UnityEngine;
using System;

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	//TODO: should these settings be per-project or per user?
	public class DirectPreviewControlPanel2 : EditorWindow
	{
        private bool configFoldout = true;
		
        DirectPreviewUnityStateVersion1 m_DirectPreviewState;

        public class DirectPreviewUnityStateStore
        {
	        const string DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY = "DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY";
	        public static DirectPreviewUnityStateVersion1 DeserializeDirectPreviewUnityStateVersionOrDefault()
	        {
		        DirectPreviewUnityStateVersion1 previewState = null;
	        
		        if (EditorPrefs.HasKey(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY))
		        {
			        try
			        {
				        previewState = JsonUtility.FromJson<DirectPreviewUnityStateVersion1>(EditorPrefs.GetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY));
				        if (previewState.Version != DirectPreviewUnityStateVersion1.KNOWN_VERSION)
				        {
					        Debug.LogError("Deserialized a state object with an unsupported version " + previewState.Version);
					        previewState = null;
				        }
			        }
			        catch (Exception e)
			        {
				        UnityEngine.Debug.LogError("Failed to deserialize DirectPreviewUnityStateVersion1 from EditorPrefs. " + e + " raw json string" + EditorPrefs.GetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY));
			        }
		        }
		        if(previewState == null)
			        previewState = DirectPreviewUnityStateVersion1.GetDefault();
		        
		        return previewState;
	        }
	        public static void Store(DirectPreviewUnityStateVersion1 previewState)
	        {
		        EditorPrefs.SetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY, JsonUtility.ToJson(previewState));
	        }
	        
        }
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

        void ShowWifiGUI()
        {
	        EditorGUILayout.LabelField("Use Wi-Fi to get data from device and show images on HMD.\n" +
                    "Suggest to use 5G Wi-Fi to get better performance.", GUILayout.Height(40));
					
	        m_DirectPreviewState.DeviceWifiAddress = EditorGUILayout.TextField("Device Wi-Fi IP: ", m_DirectPreviewState.DeviceWifiAddress);

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


        void ShowButtons()
        {
            if (GUILayout.Button("Start streaming server"))
            {
                StreamingServer.StartStreamingServer();
                configFoldout = false;
            }
            if (GUILayout.Button("Stop streaming server"))
            {
                StreamingServer.StopStreamingServer();
            }
            if (GUILayout.Button("Start Device APK"))
            {
                DirectPreviewAPK.StartSimulator();
                configFoldout = false;
            }
            if (GUILayout.Button("Stop Device APK"))
            {
                DirectPreviewAPK.StopSimulator();
            }
            if (GUILayout.Button("Install Device APK"))
            {
                DirectPreviewAPK.InstallSimulator();
            }
        }

        void OnGUI()
		{
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Application is Playing\n" + "Before any DirectPreview operation, please stop playing.", MessageType.None);
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
	}

	[InitializeOnLoad]
	public static class CheckIfSimulatorEnabled2
	{
		private const string MENU_NAME = "Wave/DirectPreview/EnableDirectPreview2";

		private static bool enabled_;
		private const string DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME = "Wave/DirectPreview/ControlPanel2";
		/// Called on load thanks to the InitializeOnLoad attribute
		static CheckIfSimulatorEnabled2()
		{
			CheckIfSimulatorEnabled2.enabled_ = EditorPrefs.GetBool(CheckIfSimulatorEnabled2.MENU_NAME, false);
			//if (CheckIfSimulatorEnabled.enabled_)
			//{
			//	switchGaphicsEmulationInner(true);
			//}

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfSimulatorEnabled2.enabled_);
			};
		}

		[MenuItem(CheckIfSimulatorEnabled2.MENU_NAME, priority = 601)]
		private static void ToggleAction()
		{
			if (!CheckIfSimulatorEnabled2.enabled_)
			{

				if (!EditorPrefs.GetBool(CheckIfSimulatorEnabled2.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME))
				{
                    DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
					window.Show();
                    EditorPrefs.SetBool(CheckIfSimulatorEnabled2.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME, true);
				}

				//switchGaphicsEmulationInner(true);
			}
			else
			{

				//switchGaphicsEmulationInner(false);
			}
			/// Toggling action
			PerformAction(!CheckIfSimulatorEnabled2.enabled_);
		}

		public static void PerformAction(bool enabled)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfSimulatorEnabled2.MENU_NAME, enabled);
			/// Saving editor state
			EditorPrefs.SetBool(CheckIfSimulatorEnabled2.MENU_NAME, enabled);

			CheckIfSimulatorEnabled2.enabled_ = enabled;
		}

		[MenuItem(CheckIfSimulatorEnabled2.MENU_NAME, validate = true, priority = 601)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfSimulatorEnabled2.MENU_NAME, enabled_);
			return true;
		}

		[MenuItem(CheckIfSimulatorEnabled2.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME, priority = 602)]
		private static void OptToggleAction()
		{
			DirectPreviewControlPanel2 window = (DirectPreviewControlPanel2)EditorWindow.GetWindow<DirectPreviewControlPanel2>("DirectPreview");
            window.Show();
        }
		//// Switch to emulation mode.
		//public static void switchGaphicsEmulationInner(bool isSwitchGaphicsEmulation)
		//{
		//	//UnityEngine.Debug.Log("switch to multi-pass: " + isSwitchGaphicsEmulation);
		//	//EditorPrefs.SetBool("isMirrorToDevice", isSwitchGaphicsEmulation);
		//	/*try
		//	{
		//		if ( isSwitchGaphicsEmulation == true ) {
		//			UnityEngine.Debug.Log("switchGaphicsEmulationInner to D3D");
		//			// Switch to no emulator
		//			EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
		//			// Switch to multipass
		//			EditorPrefs.SetBool("isMirrorToDevice", true);
		//		} else {
		//			UnityEngine.Debug.Log("switchGaphicsEmulationInner to OpenGL ES 3.0");
		//			// Set Graphic emulation back to OpenGL
		//			EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/OpenGL ES 3.0");
		//			// Set back to auto of singlepass
		//			EditorPrefs.SetBool("isMirrorToDevice", false);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		UnityEngine.Debug.LogError(e);
		//	}*/
		//}
	}
}
#endif