using System;
using System.Linq;
using System.Reflection;

namespace DirectPreviewEditor
{
    //scratchpad for prerequisites for wrapping the adb class "UnityEditor.Android.ADB" inside of "UnityEditor.Android.Extensions"
    public class AdbReflectionSetup
    {
        public Assembly AndroidExtensionsAssembly;
        public Type AndroidDeviceType;
        public Type AdbType;
        public AdbFacade AdbFacade;

        public Assembly UnityEditorCoreModule;
        public SETUP_STATUS SetupStatus;
        public enum SETUP_STATUS
        {
            UNINITIALIZED,
            FAILED,
            SUCCESS
        }
        public AdbReflectionSetup()
        {
            try
            {
                AndroidExtensionsAssembly = GetAndroidExtensionsAssembly();
                if (AndroidExtensionsAssembly == null)
                    return;
                AndroidDeviceType = AndroidExtensionsAssembly.GetType("UnityEditor.Android.AndroidDevice");
                if (AndroidDeviceType == null)
                    return;
                AdbType = AndroidExtensionsAssembly.GetType("UnityEditor.Android.ADB");
                if (AdbType == null)
                    return;
                AdbFacade = new AdbFacade(AdbType);
                UnityEditorCoreModule = GetUnityEditorCoreModuleAssembly();
                SetupStatus = UnityEditorCoreModule != null && AdbType != null && AndroidDeviceType != null ? SETUP_STATUS.SUCCESS : SETUP_STATUS.FAILED;
            }catch{
                SetupStatus = SETUP_STATUS.FAILED;
            }
            SetupStatus = SETUP_STATUS.SUCCESS;
        }
        private Assembly GetAndroidExtensionsAssembly()
        {
            var assemblyName = "UnityEditor.Android.Extensions";
            return GetAssembly(assemblyName);
        }
        private Assembly GetUnityEditorCoreModuleAssembly()
        {
            var assemblyName = "UnityEditor.CoreModule";
            return GetAssembly(assemblyName);
        }
        private Assembly GetAssembly(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName.Contains(assemblyName));
            return assembly;
        }
            
    }
}