using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using Wave.XR.Loader;

public class WaveQueryXRSettings 
{
    public static bool CheckIsBuildingWaveAndroid()
    {
        return CheckIsBuildingWave(BuildTargetGroup.Android);
    }
    public static bool CheckIsBuildingWaveStandalone()
    {
        return CheckIsBuildingWave(BuildTargetGroup.Standalone);
    }
    static bool CheckIsBuildingWave(BuildTargetGroup buildTargetGroup)
    {
        var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
        if (androidGenericSettings == null)
            return false;

        var androidXRMSettings = androidGenericSettings.AssignedSettings;
        if (androidXRMSettings == null)
            return false;
#pragma warning disable 618
        var loaders = androidXRMSettings.loaders;
#pragma warning restore 618
			
        foreach (var loader in loaders)
        {
            if (loader.GetType() == typeof(WaveXRLoader))
            {
                return true;
            }
        }
        return false;
    }
    //to hook up to wave menu
    public static bool AddIsBuildingWaveAndroid()
    {
        return SetBuildingWave(BuildTargetGroup.Android);
    }
    public static bool AddIsBuildingWaveStandalone()
    {
        return SetBuildingWave(BuildTargetGroup.Standalone);
    }
    static bool SetBuildingWave(BuildTargetGroup buildTargetGroup)
    {
        var androidGenericSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
        var androidXRSettings = androidGenericSettings.AssignedSettings;
			
        if (androidXRSettings == null)
        {
            androidXRSettings = ScriptableObject.CreateInstance<XRManagerSettings>() as XRManagerSettings;
        }
        var didAssign = XRPackageMetadataStore.AssignLoader(androidXRSettings, "Wave.XR.Loader.WaveXRLoader", BuildTargetGroup.Android);
        if (!didAssign)
        {
            Debug.LogError("Fail to add android WaveXRLoader.");
        }
        return didAssign;
    }

}
