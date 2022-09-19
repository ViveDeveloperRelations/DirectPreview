using System;
using System.Diagnostics;
using System.Reflection;
using DirectPreview.Utility;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class CommandWrapper
    {
        private Type m_CommandType;
        private Type m_ProgramType;
        public CommandWrapper(Assembly androidExtensionsAssembly,Assembly unityEditorCoreModuleAssembly)
        {
            if(androidExtensionsAssembly == null)
                throw new ArgumentNullException(nameof(androidExtensionsAssembly));
            const string commandTypeName = "UnityEditor.Android.Command";
            m_CommandType = androidExtensionsAssembly.GetType(commandTypeName) ?? throw new TypeLoadException(commandTypeName);
            m_ProgramType = unityEditorCoreModuleAssembly.GetType("UnityEditor.Utils.Program") ?? throw new TypeLoadException("UnityEditor.Android.Program");
        }

        /*
         //need to be more careful with the below
        public string Run(string command, string args, string workingDir, string errorMsg)
        {
            return ReflectionHelpers.InvokePublicStaticMethod(m_CommandType, "Run", command, args, workingDir, errorMsg) as string;
        }
        //untested
        public string Run(ProcessStartInfo startInfo,string errorMsg)
        {
            return ReflectionHelpers.InvokePublicStaticMethod(m_CommandType, "Run", startInfo, null, null, errorMsg) as string;
        }
        */
        /*
            public string Run(ProcessStartInfo startInfo,WaitingForProcessToExit onWaitDelegate, string errorMsg)
            {
                return m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
            }
            */
        public string Run(ProcessStartInfo startInfo,WaitingForProcessToExit onWaitDelegate, string errorMsg)
        {
            //Delegate del = Delegate.CreateDelegate(typeof(WaitingForProcessToExit), onWaitDelegate.Target, onWaitDelegate.Method);
            //public delegate void WaitingForProcessToExit(Program program);
            
            //TODO: actually re-wrap onWaitDelegate in a 

            
            //var myCallbackMethod = typeof(CommandWrapper).GetMethod(nameof(OnWaitForProcessToExitDelegateWrapper), BindingFlags.Public | BindingFlags.Instance);
            WrappedWaitingForProcessToExit wrappedWaitingForProcessToExit = new WrappedWaitingForProcessToExit(onWaitDelegate,m_ProgramType);
            var myCallbackMethod = typeof(WrappedWaitingForProcessToExit).GetMethod(nameof(WrappedWaitingForProcessToExit.OnWaitForProcessToExitDelegateWrapper), BindingFlags.Public | BindingFlags.Instance);
            Delegate wrappedDelegate = Delegate.CreateDelegate(WaitingForProcessDelegateType, wrappedWaitingForProcessToExit,myCallbackMethod);
            //Delegate wrappedDelegate = Delegate.CreateDelegate(WaitingForProcessDelegateType, this, nameof(OnWaitForProcessToExitDelegateWrapper));
            
            MethodInfo method = GetRunWithProcessStartInfoParam(WaitingForProcessDelegateType);            
            //var method = m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static,null, new[] {typeof(ProcessStartInfo), WaitingForProcessDelegateType, typeof(string)}, modifiers: null);
            return method.Invoke(null, new object[] { startInfo, wrappedDelegate, errorMsg}) as string;
            //return m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
        }

        private MethodInfo GetRunWithProcessStartInfoParam(Type processingDelegateType)
        {
            var methods = m_CommandType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            MethodInfo method = null;
            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();
                if(methodInfo.Name == "Run" && parameters.Length == 3 && parameters[0].ParameterType == typeof(ProcessStartInfo) && parameters[1].ParameterType == processingDelegateType && parameters[2].ParameterType == typeof(string))
                {
                    //not sure why I have to be this specific, but ok.
                    method = methodInfo;
                }
            }

            return method;
        }
        
        public class WrappedWaitingForProcessToExit
        {
            private WaitingForProcessToExit m_Wrapped;
            private Type m_ProgramType;
            public WrappedWaitingForProcessToExit(WaitingForProcessToExit wrapped,Type programType)
            {
                m_Wrapped = wrapped;
                m_ProgramType = programType;
            }
            public void OnWaitForProcessToExitDelegateWrapper(System.Object program)
            {
                UnityEngine.Debug.Log("OnWaitForProcessToExit");
                try
                {
                    var programWrapped = new ProgramWrapper(m_ProgramType,program);
                    m_Wrapped?.Invoke(programWrapped);
                }
                catch (Exception e)
                {
                    Debug.Log($"Exception in callback {nameof(WrappedWaitingForProcessToExit)} ");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }
        

        private Type m_CachedWaitingForProcessDelegateType;
        private Type WaitingForProcessDelegateType
        {
            get
            {
                if(m_CachedWaitingForProcessDelegateType == null)
                {
                    m_CachedWaitingForProcessDelegateType = m_CommandType.GetNestedType("WaitingForProcessToExit", BindingFlags.Public);
                }
                return m_CachedWaitingForProcessDelegateType;
            }
        }
        public delegate void WaitingForProcessToExit(ProgramWrapper program);
    }
}