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
using Wave.XR.DirectPreview.Editor;

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

		public static bool EnableDirectPreview = false;

		private static string LOG_TAG = "DirectPreviewCore";
		static bool saveLog = false;
		static bool saveImage = false;
		static int connectType = 0;  // USB



		[InitializeOnEnterPlayMode]
		static void OnEnterPlayModeMethod(EnterPlayModeOptions options)
		{
			var directPreviewState = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
			EnableDirectPreview = directPreviewState.DirectPreviewEnabled;
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
			var dpSerializedState = DirectPreviewUnityStateStore.DeserializeDirectPreviewUnityStateVersionOrDefault();
			EnableDirectPreview = dpSerializedState.DirectPreviewEnabled; // Should this be per-project and per-user? or per-user only?

			string wifi_ip_state = dpSerializedState.DeviceWifiAddress;
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
				//TODO: re-call EnableDP when the state options change
				//TODO: find out if this needs to be called across assembly reloads or on init
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
				//FIXME: assumes there's a camera.main on scene load, sometimes this is loaded later and/or swapped out at runtime. also multiple scene loads could cause multple attaches
				var camera = Camera.main;
				if (camera != null)
				{
					DirectPreviewRender directPreviewRender = camera.gameObject.GetComponent<DirectPreviewRender>();
					if(directPreviewRender == null)
					{
						directPreviewRender = camera.gameObject.AddComponent<DirectPreviewRender>();
					}
					//set up state if desired on directpreview, including a callback if the camera is disabled or the tag is changed from camera.main
				}
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
