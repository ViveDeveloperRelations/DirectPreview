
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

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	[InitializeOnLoad]
	public static class CheckIfSimulatorEnabled
	{
		private const string MENU_NAME = "Wave/DirectPreview/EnableDirectPreview";

		private static bool enabled_;
		private const string DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME = "Wave/DirectPreview/ControlPanel";
		/// Called on load thanks to the InitializeOnLoad attribute
		static CheckIfSimulatorEnabled()
		{
			DirectPreviewUnityStateVersion1 state = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
			CheckIfSimulatorEnabled.enabled_ = state.DirectPreviewEnabled;
			//if (CheckIfSimulatorEnabled.enabled_)
			//{
			//	switchGaphicsEmulationInner(true);
			//}

			/// Delaying until first editor tick so that the menu
			/// will be populated before setting check state, and
			/// re-apply correct action
			EditorApplication.delayCall += () =>
			{
				PerformAction(CheckIfSimulatorEnabled.enabled_,state);
			};
		}

		[MenuItem(CheckIfSimulatorEnabled.MENU_NAME, priority = 601)]
		private static void ToggleAction()
		{
			DirectPreviewUnityStateVersion1 state = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
			if (!CheckIfSimulatorEnabled.enabled_)
			{
				if (!state.DirectPreviewEnabled)
				{
					DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
					window.Show();
				}

				//switchGaphicsEmulationInner(true);
			}
			else
			{

				//switchGaphicsEmulationInner(false);
			}
			/// Toggling action
			PerformAction(!CheckIfSimulatorEnabled.enabled_,state);
		}

		public static void PerformAction(bool enabled,DirectPreviewUnityStateVersion1 state)
		{
			/// Set checkmark on menu item
			Menu.SetChecked(CheckIfSimulatorEnabled.MENU_NAME, enabled);
			/// Saving editor state
			state.DirectPreviewEnabled = enabled;
			DirectPreviewUnityStateStore.Store(state);

			CheckIfSimulatorEnabled.enabled_ = enabled;
		}

		[MenuItem(CheckIfSimulatorEnabled.MENU_NAME, validate = true, priority = 601)]
		public static bool ValidateEnabled()
		{
			Menu.SetChecked(CheckIfSimulatorEnabled.MENU_NAME, enabled_);
			return true;
		}

		[MenuItem(CheckIfSimulatorEnabled.DIRECT_PREVIEW_CONTROL_PANEL_MENU_NAME, priority = 602)]
		private static void OptToggleAction()
		{
			DirectPreviewControlPanel window = (DirectPreviewControlPanel)EditorWindow.GetWindow<DirectPreviewControlPanel>("DirectPreview");
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