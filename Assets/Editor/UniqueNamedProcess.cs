using System;
using System.Diagnostics;
using UnityEditor;

public class UniqueNamedProcess
{
    private readonly string ProcessName;
    private readonly ProcessStartInfo StartInfo;
    private Process Process;

    public UniqueNamedProcess(string processName,ProcessStartInfo startInfo)
    {
        ProcessName = processName;
        StartInfo = startInfo;
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
            return Process.GetProcessById(processID);
        }
        catch (ArgumentException) //GetProcessById throws "ArgumentException: Can't find process with ID 198528"
        {
            SetStateInt(INVALID_PROCESS_ID);
            return null;
        }
    }
    public int ProcessID()
    {
        if (Process != null)
        {
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
            
        var potentialProcess = FindProcessFromStateOrInvalidate();
        return potentialProcess?.Id ?? INVALID_PROCESS_ID;
    }

    public void Start()
    {
        var existingProcess = FindProcessFromStateOrInvalidate();
        if (existingProcess != null)
        {
            Process = existingProcess;
            return;
        }
        Process = new Process(){StartInfo = StartInfo};
        Process.Start(); //this can throw... letting them propagate for now
        SetStateInt(Process.Id);
        GC.KeepAlive(Process); //don't dispose of the process if it falls out of scope, we want the process to keep running
    }

    public void Stop()
    {
        if (Process != null)
        {
            Process.Kill();
            Process = null;
            SetStateInt(INVALID_PROCESS_ID);
            return;
        }

        var existingProcess = FindProcessFromStateOrInvalidate();
        if(existingProcess != null)
        {
            existingProcess.Kill();
            Process = null;
            SetStateInt(INVALID_PROCESS_ID);
        }
    }
}