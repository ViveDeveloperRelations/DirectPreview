using System;
using System.Diagnostics;
using System.Reflection;

namespace Editor
{
    public class CommandWrapper
    {
        private Type m_CommandType;
        public CommandWrapper(Type CommandType)
        {
            m_CommandType = CommandType ?? throw new ArgumentNullException(nameof(CommandType));
        }

        public string Run(string command, string args, string workingDir, string errorMsg)
        {
            var method = m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) }, null);
            if(method == null)
            {
                throw new InvalidOperationException("Command type does not have a Run method");
            }
            return method.Invoke(null, new object[] { command, args, workingDir, errorMsg }) as string;
        }
        //untested
        public string Run(ProcessStartInfo startInfo,string errorMsg)
        {
            var method = m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(ProcessStartInfo),typeof(Delegate),typeof(string),null }, null);
            return method.Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
        }
        
        /*
            public string Run(ProcessStartInfo startInfo,WaitingForProcessToExit onWaitDelegate, string errorMsg)
            {
                return m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
            }
            */
        public string Run(ProcessStartInfo startInfo,WaitingForProcessToExit onWaitDelegate, string errorMsg)
        {
            return m_CommandType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { startInfo, null, null, errorMsg}) as string;
        }
        //public delegate void WaitingForProcessToExit(UnityEditor.Utils.Program program);
        public delegate void WaitingForProcessToExit(ProgramWrapper program);
    }
}