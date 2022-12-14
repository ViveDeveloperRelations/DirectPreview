using System;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

// Start a new named process or find the existing version of it, between reloads of the appdomain of unity
//Typically you'll want to use this for processes where shellexecute = false so that the parent process is Unity, and if unity dies/freezes so will the sub-app
//if something like this is desirable across unity runs, probably would want to use editorprefs instead of sessionstate, but maybe that is just a dependecy that needs to be injected here
public class UniqueNamedProcessPerUnityRun
{
    private readonly string ProcessName;
    private readonly ProcessStartInfo StartInfo;
    private Process Process;

    public UniqueNamedProcessPerUnityRun(string processName,ProcessStartInfo startInfo)
    {
        ProcessName = processName;
        StartInfo = startInfo;
        Process = FindProcessFromStateOrInvalidate();
        try
        {
            if(Process.HasExited)
            {
                Process = null;
            }
        }
        catch
        {
            // ignored
        }
    }
    private string StateKey()
    {
        return ProcessName + "_PROCESS_ID";
    }
    const int INVALID_PROCESS_ID = -1;
    private int GetStateInt()
    {
        return SessionState.GetInt(StateKey(), INVALID_PROCESS_ID);
    }
    private void SetStateInt(int value)
    {
        SessionState.SetInt(StateKey(), value);
    }

    private Process FindProcessFromStateOrInvalidate()
    {
        var processID = GetStateInt();
        if (processID == INVALID_PROCESS_ID)
            return null;
        try
        {
            var process = Process.GetProcessById(processID);
            process.Refresh();
            try
            {
                if (process.HasExited)
                {
                    SetStateInt(INVALID_PROCESS_ID);
                    return null;
                }
            }
            catch
            {
            } //has exited can fail in a few spots, ignore this here for now

            //process.StartInfo = StartInfo;
            //GC.KeepAlive(process); //don't dispose of the process if it falls out of scope, we want the process to keep running
            //process.Start(); //not sure why this reference that was looked up needs to be 'started' again, but it does
            return process;
        }
        catch (ArgumentException) //GetProcessById throws "ArgumentException: Can't find process with ID 198528"
        {
            SetStateInt(INVALID_PROCESS_ID);
            return null;
        }
    }
    public int ProcessID()
    {
        if (Process == null) return INVALID_PROCESS_ID;
        try
        {
            if (Process.HasExited){ //this can throw if it hasn't been started
                Process = null;
            }
            return Process.Id;
        }
        catch
        {
            Process = null;
            return INVALID_PROCESS_ID;
        }
    }

    public Process GetProcess()
    {
        return Process;
    }

    public bool IsRunningHelperTest()
    {
        if (Process == null) return false;
        
        try
        {
            if(Process.HasExited)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch
        {
            // ignored
        }

        return false;
    }

    public void Start()
    {
        if(Process != null)
            return;
        Process = new Process(){StartInfo = StartInfo};
        Process.OutputDataReceived += (sender, args) => { Debug.Log("OUTPUT DATA" + args.Data); };
        Process.Exited += (sender, args) =>
        {
            Debug.Log("PROCESS EXITED"); 
            UnityEngine.Debug.Log($"stdout; {Process.StandardOutput.ReadToEnd()}");
            UnityEngine.Debug.Log($"stderr: {Process.StandardError.ReadToEnd()}");
        };
        
        Process.Start(); //this can throw... letting them propagate for now
        Debug.Log("Started process " + ProcessName + " with id " + Process.Id);
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        
        SetStateInt(Process.Id);
        GC.KeepAlive(Process); //don't dispose of the process if it falls out of scope, we want the process to keep running
    }

    public void Stop()
    {
        if (Process == null) return;
        Process.Kill();
        Process.WaitForExit(); //not sure about this one, might make this a flag - but this blocks until it's actually killed. most of the time this would be the expected behavior. add a flag if there's another usecase
        Process = null;
        SetStateInt(INVALID_PROCESS_ID);
    }
}