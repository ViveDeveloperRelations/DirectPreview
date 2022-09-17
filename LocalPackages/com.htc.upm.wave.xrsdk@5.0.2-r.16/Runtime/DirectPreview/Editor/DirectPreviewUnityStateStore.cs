using System;
using UnityEditor;
using UnityEngine;

namespace Wave.XR.DirectPreview.Editor
{
    public class DirectPreviewUnityStateStore
    {
        const string DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY = "DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY";
        public static DirectPreviewUnityStateVersion1 DeserializeDirectPreviewUnityStateVersionOrDefault()
        {
            DirectPreviewUnityStateVersion1 previewState = null;
	        
            if (EditorPrefs.HasKey(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY))
            {
                try
                {
                    previewState = JsonUtility.FromJson<DirectPreviewUnityStateVersion1>(EditorPrefs.GetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY));
                    if (previewState.Version != DirectPreviewUnityStateVersion1.KNOWN_VERSION)
                    {
                        Debug.LogError("Deserialized a state object with an unsupported version " + previewState.Version);
                        previewState = null;
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Failed to deserialize DirectPreviewUnityStateVersion1 from EditorPrefs. " + e + " raw json string" + EditorPrefs.GetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY));
                }
            }
            if(previewState == null)
                previewState = DirectPreviewUnityStateVersion1.GetDefault();
		        
            return previewState;
        }
        public static void Store(DirectPreviewUnityStateVersion1 previewState)
        {
            EditorPrefs.SetString(DIRECT_PREVIEW_STATE_V1_SERIALIZED_KEY, JsonUtility.ToJson(previewState));
        }
	        
    }
}