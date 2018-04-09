using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if !SMART_NET_SYMLINK_PROJECT

namespace SmartNet
{
    internal class SmartNetIdentityModificationWatcher : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            var identity = SmartNetIdentityManager.GetIdentity(path);

            if (identity != null)
            {
                EditorApplication.delayCall += () => { SmartNetIdentityManager.Remove(identity); };
            }

            return AssetDeleteResult.DidNotDelete;
        }

        private static void OnWillCreateAsset(string assetPath)
        {
            EditorApplication.delayCall += () => { SmartNetIdentityManager.Add(assetPath); };
        }

        private static AssetMoveResult OnWillMoveAsset(string fromPath, string toPath)
        {
            if (SmartNetIdentityManager.LastKnownPath == fromPath)
            {
                SmartNetIdentityManager.SetLastKnownPath(toPath);
            }

            EditorApplication.delayCall += () =>
            {
                SmartNetIdentityManager.Add(fromPath);
                SmartNetIdentityManager.Add(toPath);
            };

            return AssetMoveResult.DidNotMove;
        }

        private static string[] OnWillSaveAssets(string[] paths)
        {
            EditorApplication.delayCall += () =>
            {
                foreach (var path in paths)
                {
                    SmartNetIdentityManager.Add(path);
                }

                IdentityLibrary.RemoveNullReferences();
            };

            return paths;
        }
    }

    public static class SmartNetIdentityManager
    {
        private const string BaseLastKnownPathKey = "SmartNetLastKnownIdentityLibraryPath";
        public static string LastKnownPath;

        private static string LastKnownPathKey => $"{Application.productName}{BaseLastKnownPathKey}";

        static SmartNetIdentityManager()
        {
            Init();
        }

        private static void Init()
        {
            LastKnownPath = EditorPrefs.GetString(LastKnownPathKey, EditorUtility.DefaultIdentityLibraryFilePath);

            bool create;
            IdentityLibrary library;

            if (!IdentityLibrary.Exists())
            {
                library = AssetDatabase.LoadAssetAtPath<IdentityLibrary>(LastKnownPath);
                create = false;

                if (library == null)
                {
                    library = SearchProjectForLibrary();

                    if (library != null)
                    {
                        LastKnownPath = AssetDatabase.GetAssetPath(library);
                    }
                }

                if (library == null)
                {
                    LastKnownPath = EditorUtility.DefaultIdentityLibraryFilePath;
                    library = ScriptableObject.CreateInstance<IdentityLibrary>();
                    create = true;
                }
            }
            else
            {
                library = IdentityLibrary.Ins;
                LastKnownPath = AssetDatabase.GetAssetPath(IdentityLibrary.Ins);
                create = string.IsNullOrEmpty(LastKnownPath);

                if (create)
                {
                    LastKnownPath = EditorUtility.DefaultIdentityLibraryFilePath;
                }
            }

            if (create)
            {
                Debug.Log($"Creating new IdentityLibrary at {LastKnownPath}.");
                AssetDatabase.CreateAsset(library, LastKnownPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            SetLastKnownPath(LastKnownPath);
        }

        private static IdentityLibrary SearchProjectForLibrary()
        {
            var found = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

            foreach (var path in found)
            {
                var relativePath = FileUtil.GetProjectRelativePath(path);
                var type = AssetDatabase.GetMainAssetTypeAtPath(relativePath);

                if (type != typeof(IdentityLibrary))
                {
                    continue;
                }

                return AssetDatabase.LoadAssetAtPath<IdentityLibrary>(relativePath);
            }

            return null;
        }

        [MenuItem("Scan Project", menuItem = "Window/Smart Net/Scan Project", priority = 1000)]
        public static void ScanProject()
        {
            Init();

            var found = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);

            foreach (var file in found)
            {
                Add(FileUtil.GetProjectRelativePath(file));
            }

            IdentityLibrary.Ins.Library.RemoveAll(x => x == null);
        }

        public static void SetLastKnownPath(string path)
        {
            LastKnownPath = path;
            EditorPrefs.SetString(LastKnownPathKey, path);
        }

        public static bool Add(string file)
        {
            Init();

            var identity = GetIdentity(file);

            if (identity == null)
            {
                return false;
            }

            TryAssignNewId(identity);
            IdentityLibrary.RemoveNullReferences();

            if (IdentityLibrary.Add(identity))
            {
                UnityEditor.EditorUtility.SetDirty(IdentityLibrary.Ins);
                return true;
            }

            return false;
        }

        public static bool Remove(string file)
        {
            Init();

            var identity = GetIdentity(file);

            if (identity == null)
            {
                return false;
            }

            return Remove(identity);
        }

        public static bool Remove(SmartNetIdentity identity)
        {
            Init();

            if (identity == null)
            {
                return false;
            }

            IdentityLibrary.RemoveNullReferences();

            if (IdentityLibrary.Remove(identity))
            {
                UnityEditor.EditorUtility.SetDirty(IdentityLibrary.Ins);
                return true;
            }

            return false;
        }

        public static SmartNetIdentity GetIdentity(string path)
        {
            Init();

            if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(GameObject))
            {
                return null;
            }

            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            return asset == null ? null : asset.GetComponent<SmartNetIdentity>();
        }

        private static void TryAssignNewId(SmartNetIdentity identity)
        {
            Init();

            if (IdentityLibrary.Get(identity.AssetId) != identity)
            {
                AssignAssetId(identity, IdentityLibrary.GetNextPrefabId());
            }
        }

        private static void AssignAssetId(SmartNetIdentity identity, uint id)
        {
            var assetIdField = typeof(SmartNetIdentity).GetField("assetId", BindingFlags.NonPublic | BindingFlags.Instance);

            if (assetIdField == null)
            {
                Debug.LogError($"AssignData failed to locate 'assetId' field on {typeof(SmartNetIdentity).Name}");
                return;
            }

            assetIdField.SetValue(identity, id);
        }
    }
}
#endif