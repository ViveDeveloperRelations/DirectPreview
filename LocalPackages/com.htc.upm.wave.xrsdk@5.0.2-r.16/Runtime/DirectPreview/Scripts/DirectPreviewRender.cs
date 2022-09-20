using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR && UNITY_ANDROID
namespace Wave.XR.DirectPreview
{
	[DisallowMultipleComponent]
	public class DirectPreviewRender : MonoBehaviour
	{
		[DllImport("wvr_plugins_directpreview", EntryPoint = "WVR_SetRenderImageHandles")]
		public static extern bool WVR_SetRenderImageHandles(IntPtr[] ttPtr);

#pragma warning disable 414
		private static string TAG = "DirectPreviewRender:";
		static bool leftCall = false;
		static bool rightCall = false;
#pragma warning restore 414

		static bool isLeftReady = false;
		static bool isRightReady = false;
		static RenderTexture rt_L;
		static RenderTexture rt_R;
		static IntPtr[] rt = new IntPtr[2];
		static int mFPS = 60;
		static long lastUpdateTime = 0;
		new Camera camera;

		//is this only used to produce the same value as Time.deltaTime or is it more of a StopWatch type of thing? Those might seem more appropriate
		public static long getCurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
		}

		Material mat;

		private void Start()
		{
			camera = GetComponent<Camera>();
			lastUpdateTime = 0;

			if (mFPS == 0)
			{
				if (Application.targetFrameRate > 0 && Application.targetFrameRate < 99)
				{
					mFPS = Application.targetFrameRate;
				}
				else
				{
					mFPS = 75;
				}
				UnityEngine.Debug.LogWarning("mFPS is changed to " + mFPS);
			}
		}

		private void OnDisable()
		{
			isLeftReady = false;
			isRightReady = false;
		}

		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			//Debug.Log("vrUsage=" + src.vrUsage + ", width=" + src.width + ", height=" + src.height + ", name=" + src.name + ", frame=" + frame + ", eye=" + camera.stereoActiveEye);
			//Debug.Log("src native ptr: " + src.GetNativeTexturePtr() + ", eye=" + camera.stereoActiveEye);
			if(src.height % 2 != 0)
			{
				Debug.LogError($"src height is not even number height{src.height} width {src.width}"); //debug cases where this happens
			}
			Graphics.Blit(src, dest);

			var height = src.height;
			if ((height % 2) != 0)
			{
				UnityEngine.Debug.LogWarning("RenderTexture height is odd, skip.");
				return;
			}

			long currentTime = getCurrentTimeMillis();
			if (currentTime - lastUpdateTime >= (1000 / mFPS))
			{
				if (!isLeftReady && camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
				{
					rt[0] = src.GetNativeTexturePtr();
					UnityEngine.Debug.LogWarning(camera.stereoActiveEye + ", rt[0] : " + rt[0]);
					isLeftReady = true;
				}

				if (isLeftReady && !isRightReady && camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
				{
					rt[1] = src.GetNativeTexturePtr();

					UnityEngine.Debug.LogWarning(camera.stereoActiveEye + ", rt[1] : " + rt[1]);
					isRightReady = true;
				}

				if (isLeftReady && isRightReady)
				{
					lastUpdateTime = currentTime;
					if (WVR_SetRenderImageHandles(rt))
					{
						// Debug.LogWarning("callback successfully");
					}
					else
					{
						//UnityEngine.Debug.LogError("WVR_SetRenderImageHandles fail");
					}
					//are these boolean flags accessed by the plugin - if so there seems to be a potential race with the above call if it's running on a different thread.
					//if not, then they seem unnecessary except in local scope
					isLeftReady = false;
					isRightReady = false;
				}
			}

		}
	}
}
#endif