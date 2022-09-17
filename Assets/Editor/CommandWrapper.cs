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
        public CommandWrapper(Assembly androidExtensionsAssembly)
        {
            if(androidExtensionsAssembly == null)
                throw new ArgumentNullException(nameof(androidExtensionsAssembly));
            const string commandTypeName = "UnityEditor.Android.Command";
            m_CommandType = androidExtensionsAssembly.GetType(commandTypeName) ?? throw new TypeLoadException(commandTypeName);
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

            var waitingForDelegateType = WaitingForProcessDelegateType;
            var myCallbackMethod = typeof(CommandWrapper).GetMethod(nameof(OnWaitForProcessToExitDelegateWrapper), BindingFlags.Public | BindingFlags.Instance);
            Delegate wrappedDelegate = Delegate.CreateDelegate(WaitingForProcessDelegateType, this,myCallbackMethod);
            //Delegate wrappedDelegate = Delegate.CreateDelegate(WaitingForProcessDelegateType, this, nameof(OnWaitForProcessToExitDelegateWrapper));
            var methods = m_CommandType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            MethodInfo method = null;
            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();
                if(methodInfo.Name == "Run" && parameters.Length == 3 && parameters[0].ParameterType == typeof(ProcessStartInfo) && parameters[1].ParameterType == WaitingForProcessDelegateType && parameters[2].ParameterType == typeof(string))
                {
                    //not sure why I have to be this specific, but ok
                    method = methodInfo;
                }
            }
            //var method = m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static,null, new[] {typeof(ProcessStartInfo), WaitingForProcessDelegateType, typeof(string)}, modifiers: null);
            return method.Invoke(null, new object[] { startInfo, wrappedDelegate, errorMsg}) as string;
            //return m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
        }
        public void OnWaitForProcessToExitDelegateWrapper(System.Object program)
        {
            UnityEngine.Debug.Log("OnWaitForProcessToExit");
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
        //public delegate void WaitingForProcessToExit(UnityEditor.Utils.Program program);
        private delegate void WaitingForProcessToExitToPassIntoObjectDelegate(object programInstance);
        public delegate void WaitingForProcessToExit(ProgramWrapper program);
    }
}