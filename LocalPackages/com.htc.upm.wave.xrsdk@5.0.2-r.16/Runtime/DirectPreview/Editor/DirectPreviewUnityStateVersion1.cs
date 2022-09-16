using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Wave.XR.DirectPreview.Editor
{
    [Serializable]
    public class DirectPreviewUnityStateVersion1
    {
        public int Version = 1;
        public string DeviceWifiAddress;
        public bool EnablePreviewImage;
        //public bool DllTraceLogToFile;
        public bool OutputImageToFile;
        public bool UpdateFrequency;
        
        public ConnectTypeEnum ConnectType = ConnectTypeEnum.USB;
        public FPSOption FPS;
        public TargetSizeRatioOption TargetSizeRatio;
        
        public bool DoesADBExist;
        
        public enum ConnectTypeEnum
        {
            USB=0,
            WIFI=1,
        }
        public enum DeviceTypeEnum
        {
            Others=0,
            FocusPlus=1,
        }
        [Serializable]
        public class TargetSizeRatioOption
        {
            public int TargetSizeRatioInt;
            public static Tuple<int,string>[] TargetSizeRatioOptions = new Tuple<int, string>[]
            {
                new Tuple<int, string>(1, "1"),
                new Tuple<int, string>(2, "0.8"),
                new Tuple<int, string>(3, "0.6"),
                new Tuple<int, string>(4, "0.4"),
                new Tuple<int, string>(5, "0.2"),
            };

            public string GetPrintableRatio()
            {
                return TargetSizeRatioOptions.FirstOrDefault((option) => option.Item1 == TargetSizeRatioInt)?.Item2;
            }
        }
        [Serializable]
        public class FPSOption
        {
            public int FPSOptionInt; //TODO: maybe serialize a tuple to avoid index issues
            public static Tuple<int, string>[] FPS_Pairs = new Tuple<int, string>[]
            {
                new(0, "Runtime Defined"),
                new(15,"15 FPS"),
                new(30,"30 FPS"),
                new(45,"45 FPS"),
                new(60,"60 FPS"),
                new(75,"75 FPS"),
            };
            public string GetPrintableRatio()
            {
                return FPS_Pairs.FirstOrDefault((option) => option.Item1 == FPSOptionInt)?.Item2;
            }
        }

    }
}