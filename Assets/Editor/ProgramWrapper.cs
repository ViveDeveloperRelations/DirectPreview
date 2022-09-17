using System;
using System.Diagnostics;
using System.Reflection;

namespace Editor
{
    public class ProgramWrapper
    {
        private readonly Type m_ProgramType;
        public ProgramWrapper(Type programType)
        {
            m_ProgramType = programType ?? throw new ArgumentNullException(nameof(programType));
        }
        public Object CreateInstance(ProcessStartInfo startInfo)
        {
            return m_ProgramType.GetConstructor(new []{typeof(ProcessStartInfo)}).Invoke(new object[] {startInfo});
        }
    
        public string GetAllOutput(Object objectInstance)
        {
            return m_ProgramType.GetMethod("GetAllOutput", BindingFlags.Public | BindingFlags.Instance).Invoke(objectInstance, null) as string;
        }
        public bool HasExited(Object objectInstance)
        {
            return m_ProgramType.GetProperty("HasExited", BindingFlags.Public | BindingFlags.Instance).GetValue(objectInstance) as bool? ?? false;
        }

        public void Kill(Object objectInstance)
        {
            m_ProgramType.GetMethod("Kill", BindingFlags.Public | BindingFlags.Instance).Invoke(objectInstance, null);
        }
        public int ExitCode(Object objectInstance)
        {
            return m_ProgramType.GetProperty("ExitCode", BindingFlags.Public | BindingFlags.Instance).GetValue(objectInstance) as int? ?? 0;
        }
        public int Id(Object objectInstance)
        {
            return m_ProgramType.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance).GetValue(objectInstance) as int? ?? 0;
        }
        public string GetStandardOutputAsString(Object objectInstance)
        {
            return m_ProgramType.GetProperty("GetStandardOutputAsString", BindingFlags.Public | BindingFlags.Instance).GetValue(objectInstance, null) as string;
        }
    }
    public class ProgramWrapperTwo
    {
        private readonly Type m_ProgramType;
        private readonly System.Object m_ProgramInstance;

        public ProgramWrapperTwo(Type programType,System.Object programInstance)
        {
            m_ProgramType = programType ?? throw new ArgumentNullException(nameof(programType));
            m_ProgramInstance = programInstance ?? throw new ArgumentNullException(nameof(programInstance));
        }

        public string GetAllOutput()
        {
            return m_ProgramType.GetMethod("GetAllOutput", BindingFlags.Public | BindingFlags.Instance).Invoke(m_ProgramInstance, null) as string;
        }
        public bool HasExited()
        {
            return m_ProgramType.GetProperty("HasExited", BindingFlags.Public | BindingFlags.Instance).GetValue(m_ProgramInstance) as bool? ?? false;
        }

        public void Kill()
        {
            m_ProgramType.GetMethod("Kill", BindingFlags.Public | BindingFlags.Instance).Invoke(m_ProgramInstance, null);
        }
        public int ExitCode()
        {
            return m_ProgramType.GetProperty("ExitCode", BindingFlags.Public | BindingFlags.Instance).GetValue(m_ProgramInstance) as int? ?? 0;
        }
        public int Id()
        {
            return m_ProgramType.GetProperty("ID", BindingFlags.Public | BindingFlags.Instance).GetValue(m_ProgramInstance) as int? ?? 0;
        }
        public string GetStandardOutputAsString()
        {
            return m_ProgramType.GetProperty("GetStandardOutputAsString", BindingFlags.Public | BindingFlags.Instance).GetValue(m_ProgramInstance, null) as string;
        }
    }
}