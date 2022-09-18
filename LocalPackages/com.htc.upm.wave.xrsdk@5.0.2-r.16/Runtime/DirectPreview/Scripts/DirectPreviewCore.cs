using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

#if UNITY_EDITOR && UNITY_ANDROID
namespace Wave.XR.DirectPreview
{
	public class DirectPreviewCore
	{
		public enum SIM_ConnectType
		{
			SIM_ConnectType_USB = 0,
			SIM_ConnectType_Wifi = 1,
		}

		public delegate void printcallback(string z);

		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetPrintCallback")]
		public static extern void WVR_SetPrintCallback_S(printcallback callback);

		public static void PrintLog(string msg)
		{
			UnityEngine.Debug.Log("WVR_DirectPreview: " + msg);
		}

		[DllImport("wvrunityxr", EntryPoint = "EnableDP")]
		public static extern void EnableDP(bool enable, SIM_ConnectType type, IntPtr addr, bool preview, bool printLog, bool saveImage);

		//[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_Quit_S")]
		//public static extern void WVR_Quit_S();

		//[DllImport("wvrunityxr", EntryPoint = "GetFirstEyePtr")]
		//public static extern IntPtr GetFirstEyePtr();

		//[DllImport("wvrunityxr", EntryPoint = "GetSecondEyePtr")]
		//public static extern IntPtr GetSecondEyePtr();

		//private static string TAG = "DirectPreviewCore:";
		public static bool EnableDirectPreview = false;
		private static Camera camera = null;

		private static string LOG_TAG = "DirectPreviewCore";
		static string wifi_ip_tmp;
		static string wifi_ip_state = "";
		bool enablePreview = false;
		static bool saveLog = false;
		static bool saveImage = false;
		static int connectType = 0;  // USB



		[InitializeOnEnterPlayMode]
		static void OnEnterPlayModeMethod(EnterPlayModeOptions options)
		{
			EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			PrintDebug("OnEnterPlayModeMethod: " + EnableDirectPreview);

			if (EnableDirectPreview)
			{
				PrintDebug("Enable direct preview and add delegate to sceneLoaded");
				SceneManager.sceneLoaded += OnSceneLoaded;
				EditorApplication.wantsToQuit += WantsToQuit;

				PrintDebug("DirectPreviewCore.DP_Init");
				DP_Init();
			} else
			{
				EnableDP(false, (SIM_ConnectType)SIM_ConnectType.SIM_ConnectType_USB, IntPtr.Zero, false, false, false);
				PrintDebug("Enable Direct Preview: " + false);
			}
		}

		public static bool dpServerProcessChecker()
		{
			bool flag = false;
			Process[] processlist = Process.GetProcesses();
			foreach (Process theProcess in processlist)
			{
				if (theProcess.ProcessName == "dpServer")
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public static void DP_Init()
		{
			EnableDirectPreview = EditorPrefs.GetBool("Wave/DirectPreview/EnableDirectPreview", false);
			
			wifi_ip_state = EditorPrefs.GetString("wifi_ip_state");
			bool tPreview = EditorPrefs.GetBool("EnablePreviewImage", true);
			saveLog = EditorPrefs.GetBool("DllTraceLogToFile", false);
			saveImage = EditorPrefs.GetBool("OutputImagesToFile", false);
			connectType = EditorPrefs.GetInt("ConnectType", 1);
			string ipaddr = wifi_ip_state;
			System.IntPtr ptrIPaddr = Marshal.StringToHGlobalAnsi(ipaddr);

			if (EnableDirectPreview)
			{
				PrintDebug("Register direct preview print callback");
				WVR_SetPrintCallback_S(PrintLog);

				//if (connectType == 1)
				//{
				//	if (dpServerProcessChecker())
				//		UnityEngine.Debug.Log("dpServer.exe is running in task list.");
				//	else
				//		UnityEngine.Debug.LogWarning("There's no dpServer.exe running in task list.");
				//}

				EnableDP(true, (SIM_ConnectType)connectType, ptrIPaddr, tPreview, saveLog, saveImage);
				PrintDebug("Enable Direct Preview: " + true + ", connection: " + connectType + ", IP: " + ipaddr + ", preview: " + tPreview + ", log: " + saveLog + ", image: " + saveImage);
			}
		}


		private static void PrintError(string msg)
		{
			UnityEngine.Debug.LogError(LOG_TAG + ": " + msg);
		}

		private static void PrintDebug(string msg)
		{
			UnityEngine.Debug.Log(LOG_TAG + ": " + msg);
		}

		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			PrintDebug("OnSceneLoaded() " + scene.name);
			bool tPreview = EditorPrefs.GetBool("EnablePreviewImage");
			if (EnableDirectPreview && (connectType == 1) && tPreview)
			{
				PrintDebug("OnSceneLoaded() call WVR_PostInit()");
				Camera.main.gameObject.AddComponent<DirectPreviewRender>();
			}
		}

		static bool WantsToQuit()
		{
			UnityEngine.Debug.Log("Editor prevented from quitting. --------");
			SceneManager.sceneLoaded -= OnSceneLoaded;

			return true;
		}
	}
}
#endif
