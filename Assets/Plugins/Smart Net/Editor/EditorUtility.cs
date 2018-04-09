using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SmartNet.Messages;
using UnityEditor;
using UnityEngine;

namespace SmartNet
{
    public static class EditorUtility
    {
        public const string InvalidTypeIdentifier = "SmartNetInvalidType";
        public const string InvalidBaseTypeName = "SmartNetInvalidBaseType";
        public const string InternalMessageNamespace = "SmartNet.Messages";

        public static readonly string UtilityFilePath;
        public static readonly string DefaultIdentityLibraryFilePath;
        public static readonly List<Type> NetMessageTypes;

        static EditorUtility()
        {
            GeneratedMessageTypes.Initialize();

            NetMessageTypes = new List<Type>(AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                .Where(x => typeof(INetMessage).IsAssignableFrom(x) && !x.IsInterface && !x.Name.Contains(InvalidTypeIdentifier) && x.Name != InvalidBaseTypeName));

            var smartNetFolderPath = Directory.GetDirectories(Path.Combine(Application.dataPath, "Plugins"), "Smart Net", SearchOption.AllDirectories).FirstOrDefault();

            if (smartNetFolderPath == null)
            {
                Debug.LogError("Unable to locate Smart Net folder in Assets/Plugins!");
                return;
            }

            UtilityFilePath = $"{smartNetFolderPath}{Path.DirectorySeparatorChar}Generated{Path.DirectorySeparatorChar}GeneratedMessageTypes.cs";
            DefaultIdentityLibraryFilePath = FileUtil.GetProjectRelativePath($"{Application.dataPath}{Path.DirectorySeparatorChar}Library.asset");
        }

        public static uint GetNextTypeId(ref uint nextId)
        {
            ++nextId;

            for (; nextId < uint.MaxValue; ++nextId)
            {
                if (!Default.IsDefaultMessage(nextId) && MessageTypes.GetType(nextId) == null)
                {
                    return nextId;
                }
            }

            Debug.LogError($"Exceeded max INetMessage count of {uint.MaxValue}. Critical failure!");
            return 0u;
        }

        public static bool TypesChanged()
        {
            return TypesChanged(NetMessageTypes);
        }

        public static bool TypesChanged(List<Type> types)
        {
            if (MessageTypes.Count != types.Count)
            {
                return true;
            }

            foreach (var type in types)
            {
                if (MessageTypes.GetTypeId(type) == 0u)
                {
                    return true;
                }
            }

            Dictionary<Type, uint> typeToMessageId = null;

            try
            {
                var fieldInfo = typeof(GeneratedMessageTypes).GetField("TypeToMessageId", BindingFlags.Static | BindingFlags.NonPublic);
                typeToMessageId = (Dictionary<Type, uint>)fieldInfo.GetValue(null);
            }
            catch
            {
                // Don't care
            }

            if (typeToMessageId != null)
            {
                foreach (var kvp in typeToMessageId)
                {
                    if (!types.Contains(kvp.Key))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return types.Count > 0;
            }

            return false;
        }
    }
}