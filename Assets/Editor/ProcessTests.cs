#define ENABLE_TESTS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Editor;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class ProcessTests
{
#if ENABLE_TESTS
    [MenuItem("Tests/Process/TestADBDevicesRunFromPath")]
    public static void TestADBDevicesRunFromPath()
    {
        var setup = GetSetup();
        TestADBDevicesRunFromPath(setup.Item1, setup.Item2);
    }
    [MenuItem("Tests/Process/BlockingTestBrokenForNow")]
    public static void BlockingTestBrokenForNow()
    {
        var setup = GetSetup();
        BlockingTestBrokenForNow(setup.Item1);
    }
    [MenuItem("Tests/Process/SimpleHelloWorldCMD")]
    public static void SimpleHelloWorldCMD()
    {
        var setup = GetSetup();
        SimpleHelloWorldCMD(setup.Item1);
    }
    [MenuItem("Tests/Process/SimpleTimeout")]
    public static void SimpleTimeout()
    {
        var setup = GetSetup();
        SimpleTimeout(setup.Item1);
    }
    [MenuItem("Tests/Process/SimpleHelloWorldCMDBlocksUntilKillSubprocessWithSlashC")]
    public static void SimpleHelloWorldCMDBlocksUntilKillSubprocessWithSlashC()
    {
        var setup = GetSetup();
        SimpleHelloWorldCMDBlocksUntilKillSubprocessWithSlashC(setup.Item1);
    }
    [MenuItem("Tests/Process/SimpleHelloWorldCMDBlocksUntilKillSubprocess")]
    public static void SimpleHelloWorldCMDBlocksUntilKillSubprocess()
    {
        var setup = GetSetup();
        SimpleHelloWorldCMDBlocksUntilKillSubprocess(setup.Item1);
    }
    
    [MenuItem("Tests/Process/SimpleHelloWorldCMDNoShellExecute")]
    public static void SimpleHelloWorldCMDNoShellExecute()
    {
        var setup = GetSetup();
        SimpleHelloWorldCMDNoShellExecute(setup.Item1);
    }

    [MenuItem("Tests/Process/SimpleHelloWorld")]
    public static void SimpleHelloWorld()
    {
        var setup = GetSetup();
        SimpleHelloWorld(setup.Item1);
    }
#endif
    private static (CommandWrapper,ADBWrapper.AdbReflectionSetup) GetSetup()
    {
        ADBWrapper.AdbReflectionSetup adbReflection = new ADBWrapper.AdbReflectionSetup();
        var commandType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command");
        if(commandType == null)
            Debug.Log("CommandType is null");
        Type waitingForProcessToExitType = commandType.GetNestedType("WaitingForProcessToExit", BindingFlags.Public);
        //var waitingForProcessToExitType = adbReflection.AndroidExtensionsAssembly.GetType("UnityEditor.Android.Command.WaitingForProcessToExit");
        if(waitingForProcessToExitType == null)
            Debug.Log("WaitingForProcessToExitType is null");
        //var waitingForProcessToExitInstance = Activator.CreateInstance(waitingForProcessToExitType);
        var commandWrapper = new CommandWrapper(adbReflection.AndroidExtensionsAssembly,adbReflection.UnityEditorCoreModule);
        return (commandWrapper,adbReflection);
    }
    static void TestADBDevicesRunFromPath(CommandWrapper commandWrapper, ADBWrapper.AdbReflectionSetup adbReflection)
    {
        ProcessStartInfo si = new ProcessStartInfo()
        {
            FileName = adbReflection.AdbFacade.GetAdbPath(),
            Arguments = "devices",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        commandWrapper.Run(si, StandardDuringWait(),"Error Running Devices with processStartInfo");
    }
    public static CommandWrapper.WaitingForProcessToExit StandardDuringWait(){
        return (ProgramWrapper program) =>
        {
            Debug.Log("INNER PROGRAM WRAPPER LOG");
            Debug.Log(program.GetAllOutput());
        };
    }

    static void BlockingTestBrokenForNow(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/c \"echo hello && timeout 10 /nobreak&& echo world\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        string outputString = commandWrapper.Run(si, StandardDuringWait(),"Error running wait and hello world");            
        Debug.Log("After Running blocking test and output is: " + outputString);
    }
#if ENABLE_TESTS
    [MenuItem("Tests/Process/NonBlockingStartProcessThroughProcessDotStart")]
#endif
    static void NonBlockingStartProcessThroughProcessDotStart()
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/c \"echo hello && timeout 10 && echo world\"",
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = true,
            CreateNoWindow = false,
        };
        var proccess = new Process()
        {
            StartInfo = si,
        };
        proccess.Start();
        proccess.Exited += (object sender, EventArgs e) =>
        {
            Debug.Log("Process Exited");
            Debug.Log($"stdout; {proccess.StandardOutput.ReadToEnd()}");
            Debug.Log($"stderr: {proccess.StandardError.ReadToEnd()}");
        };
    }
    static void SimpleHelloWorldCMD(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/c \"echo hello\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = true,
            CreateNoWindow = true,
        };

        var output = commandWrapper.Run(si, StandardDuringWait(),"Error running wait and hello world");            
        Debug.Log("After Running blocking test and output is: " + output);
    }
    static void SimpleTimeout(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\timeout.exe",
            Arguments = "10",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        CommandWrapper.WaitingForProcessToExit duringWait = (ProgramWrapper program) =>
        {
            Debug.Log("INNER PROGRAM WRAPPER LOG");
            Debug.Log(program.GetAllOutput());
        };
        var output= commandWrapper.Run(si, duringWait,"Error running wait and hello world");        
        Debug.Log("After Running blocking test and output is: " + output);
    }
    static void SimpleHelloWorldCMDBlocksUntilKillSubprocess(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/k \"echo hello\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = true,
            CreateNoWindow = false,
        };
        var output= commandWrapper.Run(si, StandardDuringWait(),"Error running wait and hello world");     
        Debug.Log("After Running blocking test and output is: " + output);
    }
    static void SimpleHelloWorldCMDBlocksUntilKillSubprocessWithSlashC(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/c \"echo hello\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = true,
            CreateNoWindow = false,
        };
        var output = commandWrapper.Run(si, StandardDuringWait(),"Error running wait and hello world");
        Debug.Log("After Running blocking test and output is: " + output);
    }
    static void SimpleHelloWorldCMDNoShellExecute(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\cmd.exe",
            Arguments = "/c \"echo hello\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        CommandWrapper.WaitingForProcessToExit duringWait = (ProgramWrapper program) =>
        {
            Debug.Log("INNER PROGRAM WRAPPER LOG");
            Debug.Log(program.GetAllOutput());
        };
        var output = commandWrapper.Run(si, duringWait,"Error running wait and hello world");            
        Debug.Log("After Running blocking test and output is: " + output);
    }
    static void SimpleHelloWorld(CommandWrapper commandWrapper)
    {
        var si = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\print.exe",
            Arguments = "hello",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        var output = commandWrapper.Run(si, StandardDuringWait(),"Error running wait and hello world");            
        Debug.Log("After Running blocking test and output is: " + output);
    }

}
