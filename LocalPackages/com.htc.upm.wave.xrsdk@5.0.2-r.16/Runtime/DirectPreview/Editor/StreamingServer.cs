// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using System.Linq;
using System.IO;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace Wave.XR.DirectPreview.Editor
{
	public class ProcessWrapper : IDisposable
	{
		public Action<string> LogCallback = (string str)=>Debug.Log(str);
		public Process process = new Process();
		private bool m_StartedProcess = false;

		public void Run(string command,string args)
		{
			if(m_StartedProcess)
			{
				if(!process.HasExited)
					process.Kill();
				DumpAllProcessInfo();
			}
			process.StartInfo.FileName = command;
			process.StartInfo.Arguments = args;
			LogCallback($"About to execute \"{command} {args}\"");
			try
			{
				process.Start();
				
				process.OutputDataReceived += ProcessOnOutputDataReceived;
				m_StartedProcess = true;
			}
			catch
			{
				LogCallback("Error while starting process");
				throw;
			}
		}

		private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			LogCallback(e.Data);
		}

		void DumpAllProcessInfo()
		{
			string startInfo = process.StartInfo == null ? "no start info available... odd" : $"{process.StartInfo.FileName} {process.StartInfo.Arguments} ";
			LogCallback($"exiting process {startInfo} with code {process.ExitCode}");
			try
			{
				LogCallback($"stdout: {(process.StandardOutput == null ? "" : process.StandardOutput.ReadToEnd())}");
				LogCallback($"stderr: {(process.StandardError == null ? "" : process.StandardError.ReadToEnd())}");
				process.OutputDataReceived -= ProcessOnOutputDataReceived;
				process.OutputDataReceived += ProcessOnOutputDataReceived;
			}
			catch (Exception e)
			{
				LogCallback($"Error while dumping process info: {e.Message}");
			}
		}
		//do we want to restart on recompilation?
		public void Dispose()
		{
			if (process == null) return;
			DumpAllProcessInfo();
			process.OutputDataReceived -= ProcessOnOutputDataReceived;
			process.Dispose();
		}
	}
	public class StreamingServer
	{
		public static ProcessWrapper myProcess = new ProcessWrapper();
		private static string CMD_PATH = "C:\\Windows\\System32\\cmd.exe";
		

		//[UnityEditor.MenuItem("Wave/DirectPreview/Start Streaming Server", priority = 701)]
		static void StartStreamingServerMenu()
		{
			StartStreamingServer();
		}

		//[UnityEditor.MenuItem("Wave/DirectPreview/Stop Streaming Server", priority = 702)]
		static void StopStreamingServerMenu()
		{
			StopStreamingServer();
		}

		public static bool isStreamingServerExist()
		{
			var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary/dpServer.exe");

			UnityEngine.Debug.Log("StreamingServer pull path = " + absolutePath);

			return File.Exists(absolutePath);
		}

		private static bool ProcessExited = false;
		
		// Launch rrServer
		public static void StartStreamingServer()
		{
			if (isStreamingServerExist())
			{
				try
				{
					var absolutePath = Path.GetFullPath("Packages/com.htc.upm.wave.xrsdk/Runtime/DirectPreview/Binary");
					var driveStr = absolutePath.Substring(0, 2);

					UnityEngine.Debug.Log("StreamingServer in " + absolutePath);
					//Get the path of the Game data folder
					string cmd_args = "/c \"" + driveStr + " && cd " + absolutePath + " && dpServer\"";
					myProcess.Run(CMD_PATH,cmd_args);
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e);
				}
			}
			else
			{
				// dpServer is not found //ASINK: this doesn't run when dpserver isn't found
				UnityEngine.Debug.LogError("Streaming server is not found, please update full package from https://developer.vive.com/resources/knowledgebase/wave-sdk/");
			}

		}
		// Stop rrServer
		public static void StopStreamingServer()
		{
			if (isStreamingServerExist())
			{
				try
				{
					UnityEngine.Debug.Log("Stop Streaming Server.");
					string cmd_args = "/c taskkill /F /IM dpServer.exe";
					myProcess.Run(CMD_PATH,cmd_args);
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e);
				}
			}
			else
			{
				// dpServer is not found
				UnityEngine.Debug.LogError("Streaming server is not found, please update full package from https://developer.vive.com/resources/knowledgebase/wave-sdk/");
			}
		}
	}
}
#endif