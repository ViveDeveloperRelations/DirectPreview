using System;
using System.Diagnostics;
using System.Reflection;

namespace DirectPreviewEditor
{
    public class ProgramWrapper
    {
        private readonly Type m_ProgramType;
        public readonly Object m_ProgramInstance;

        public ProgramWrapper(Type programType,Object programInstance)
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