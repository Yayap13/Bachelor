using SmartNet;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class SmartNetIdentityLibraryManager
{
    [DidReloadScripts]
    private static void OnReload()
    {
        var foundLibrary = false;
        var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
        var so = new SerializedObject(playerSettings);
        var preloadedAssets = so.FindProperty("preloadedAssets");

        if (preloadedAssets == null || !preloadedAssets.isArray)
        {
            Debug.LogError("Error finding property preloadedAssets on PlayerSettings. Did Unity make a real API for this?");
            return;
        }

        for (var i = 0; i < preloadedAssets.arraySize; ++i)
        {
            var asset = preloadedAssets.GetArrayElementAtIndex(i);

            if (asset.objectReferenceValue is IdentityLibrary)
            {
                foundLibrary = true;
                break;
            }
        }

        if (!foundLibrary)
        {
            var index = preloadedAssets.arraySize;
            preloadedAssets.InsertArrayElementAtIndex(index);
            var property = preloadedAssets.GetArrayElementAtIndex(index);
            property.objectReferenceValue = IdentityLibrary.Ins;

            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}