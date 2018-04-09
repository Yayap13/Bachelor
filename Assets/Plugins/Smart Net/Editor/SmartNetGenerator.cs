using System;
using System.Collections.Generic;
using SmartNet.Messages;
using UnityEditor;
using UnityEditor.Callbacks;

#if !SMART_NET_SYMLINK_PROJECT

namespace SmartNet
{
    public static class SmartNetGenerator
    {
        private const string MessageIdType = "uint";
        private static uint nextId = 0u;

        [DidReloadScripts]
        private static void Regenerate()
        {
            if (!EditorUtility.TypesChanged())
            {
                return;
            }

            ForceRegenerate();
        }

        [MenuItem("Window/Smart Net/Regenerate Message Types")]
        private static void ForceRegenerate()
        {
            EditorApplication.delayCall += () =>
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Code Generation", "Regenerating SmartNet.GeneratedMessageTypes", 0.1f);
                Generate();
                UnityEditor.EditorUtility.DisplayProgressBar("Code Generation", "Regenerating SmartNet.GeneratedMessageTypes", 1f);
                EditorApplication.isPlaying = false;
                AssetDatabase.Refresh();
                UnityEditor.EditorUtility.ClearProgressBar();
            };
        }

        private static void Generate()
        {
            nextId = 0u;
            var writer = new CodeWriter();

            writer.WriteLine("// Generated code. Do not modify.");
            writer.WriteLine("");
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Runtime.CompilerServices;");
            writer.WriteLine("using SmartNet.Messages;");

            if (EditorUtility.NetMessageTypes.Count > 0)
            {
                writer.WriteLine("using System.Collections.Generic;");
            }

            writer.WriteLine("");
            writer.WriteLine("namespace SmartNet");
            writer.WriteLine("{");

            var invalidClassNames = new Dictionary<string, List<Type>>();

            if (EditorUtility.NetMessageTypes.Count > 0)
            {
                writer.WriteLine($"internal abstract class {EditorUtility.InvalidBaseTypeName} : INetMessage");
                writer.WriteLine("{");
                writer.WriteLine("public void OnSerialize(Writer writer)");
                writer.WriteLine("{");
                writer.WriteLine("}");

                writer.WriteLine("public void OnDeserialize(Reader reader)");
                writer.WriteLine("{");
                writer.WriteLine("}");
                writer.WriteLine("}");
                writer.WriteLine("");
            }

            foreach (var type in EditorUtility.NetMessageTypes)
            {
                List<Type> list;

                if (!invalidClassNames.TryGetValue(type.Name, out list))
                {
                    list = new List<Type>();
                    invalidClassNames[type.Name] = list;
                }

                list.Add(type);

                writer.WriteIndent();
                writer.Write("internal class ");
                WriteInvalidClassName(writer, type, list);
                writer.Write(" : SmartNetInvalidBaseType { }\n");
            }

            writer.Write('\n');
            writer.WriteLine("public class GeneratedMessageTypes : MessageTypes");
            writer.WriteLine("{");

            writer.WriteLine("public static void Initialize()");
            writer.WriteLine("{");
            writer.WriteLine("if (Instance == null || Instance.GetType() != typeof(GeneratedMessageTypes))");
            writer.WriteLine("{");
            writer.WriteLine("SetMessageType(new GeneratedMessageTypes());");
            writer.WriteLine("}");
            writer.WriteLine("}");
            writer.Write('\n');

            writer.WriteLine($"public override {MessageIdType} MessageCount {{ get; }} = {EditorUtility.NetMessageTypes.Count};");
            writer.WriteLine("");

            writer.WriteLine("public override Type[] AllTypes { get; } =");
            writer.WriteLine("{");

            for (var i = 0; i < EditorUtility.NetMessageTypes.Count; ++i)
            {
                var type = EditorUtility.NetMessageTypes[i];

                List<Type> list;

                if (!invalidClassNames.TryGetValue(type.Name, out list))
                {
                    list = new List<Type>();
                    invalidClassNames[type.Name] = list;
                }

                writer.WriteIndent();
                writer.Write($"Type.GetType(\"{type.AssemblyQualifiedName}\") ?? typeof(");
                WriteInvalidClassName(writer, type, list);
                writer.Write("),");
                writer.Write('\n');
            }

            writer.WriteLine("};");
            writer.Write('\n');

            writer.WriteLine("public override string[] AllTypeNames { get; } =");
            writer.WriteLine("{");

            for (var i = 0; i < EditorUtility.NetMessageTypes.Count; ++i)
            {
                writer.WriteIndent();
                writer.Write($"\"{EditorUtility.NetMessageTypes[i].FullName}\",");
                writer.Write('\n');
            }

            writer.WriteLine("};");
            writer.Write('\n');

            var idToTypeWriter = new CodeWriter(writer.IndentLevel);
            var typeToIdWriter = new CodeWriter(writer.IndentLevel);
            var idToTypeNameWriter = new CodeWriter(writer.IndentLevel);

            if (EditorUtility.NetMessageTypes.Count > 0)
            {
                idToTypeWriter.WriteLine($"private static readonly Dictionary<{MessageIdType}, Type> MessageIdToType = new Dictionary<{MessageIdType}, Type>()");
                idToTypeWriter.WriteLine("{");

                typeToIdWriter.WriteLine($"private static readonly Dictionary<Type, {MessageIdType}> TypeToMessageId = new Dictionary<Type, {MessageIdType}>()");
                typeToIdWriter.WriteLine("{");

                idToTypeNameWriter.WriteLine($"private static readonly Dictionary<{MessageIdType}, string> MessageIdToTypeName = new Dictionary<{MessageIdType}, string>()");
                idToTypeNameWriter.WriteLine("{");

                var defaultMessages = new DefaultMessageTypes();
                var generatedMessages = new GeneratedMessageTypes();

                foreach (var type in EditorUtility.NetMessageTypes)
                {
                    var id = defaultMessages.GetTypeIdFromType(type);

                    if (id == 0u)
                    {
                        id = generatedMessages.GetTypeIdFromType(type);
                    }

                    if (id == 0u)
                    {
                        id = EditorUtility.GetNextTypeId(ref nextId);
                    }

                    List<Type> list;

                    if (!invalidClassNames.TryGetValue(type.Name, out list))
                    {
                        list = new List<Type>();
                        invalidClassNames[type.Name] = list;
                    }

                    idToTypeWriter.WriteIndent();
                    idToTypeWriter.Write('{');
                    idToTypeWriter.Write($" {id}, Type.GetType(\"{type.AssemblyQualifiedName}\") ?? typeof(");
                    WriteInvalidClassName(idToTypeWriter, type, list);
                    idToTypeWriter.Write(") }");
                    idToTypeWriter.Write(',');
                    idToTypeWriter.Write('\n');

                    typeToIdWriter.WriteIndent();
                    typeToIdWriter.Write('{');
                    typeToIdWriter.Write($" Type.GetType(\"{type.AssemblyQualifiedName}\") ?? typeof(");
                    WriteInvalidClassName(typeToIdWriter, type, list);
                    typeToIdWriter.Write($"), {id} ");
                    typeToIdWriter.Write('}');
                    typeToIdWriter.Write(',');
                    typeToIdWriter.Write('\n');

                    idToTypeNameWriter.WriteIndent();
                    idToTypeNameWriter.Write('{');
                    idToTypeNameWriter.Write($" {id}, \"{type.FullName}\" ");
                    idToTypeNameWriter.Write('}');
                    idToTypeNameWriter.Write(',');
                    idToTypeNameWriter.Write('\n');
                }

                idToTypeWriter.WriteLine("};");
                typeToIdWriter.WriteLine("};");
                idToTypeNameWriter.WriteLine("};");

                writer.Write(idToTypeWriter.ToString());
                writer.Write('\n');
                writer.Write(typeToIdWriter.ToString());
                writer.Write('\n');
                writer.Write(idToTypeNameWriter.ToString());

                writer.WriteLine("");
            }

            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine($"public override Type GetTypeFromId({MessageIdType} typeId)");
            writer.WriteLine("{");

            if (EditorUtility.NetMessageTypes.Count > 0)
            {
                writer.WriteLine("Type type;");
                writer.WriteLine("");
                writer.WriteLine("if (MessageIdToType.TryGetValue(typeId, out type))");
                writer.WriteLine("{");
                writer.WriteLine("return type;");
                writer.WriteLine("}");
                writer.WriteLine("");
            }

            writer.WriteLine("return null;");
            writer.WriteLine("}");
            writer.WriteLine("");

            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine($"public override string GetTypeNameFromId({MessageIdType} typeId)");
            writer.WriteLine("{");
            writer.WriteLine("string name;");
            writer.WriteLine("");
            writer.WriteLine("if(MessageIdToTypeName.TryGetValue(typeId, out name))");
            writer.WriteLine("{");
            writer.WriteLine("return name;");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("return \"UnknownType\";");
            writer.WriteLine("}");
            writer.WriteLine("");

            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine($"public override {MessageIdType} GetTypeIdFromType(Type type)");
            writer.WriteLine("{");

            if (EditorUtility.NetMessageTypes.Count > 0)
            {
                writer.WriteLine($"{MessageIdType} typeId;");
                writer.WriteLine("");
                writer.WriteLine("if (TypeToMessageId.TryGetValue(type, out typeId))");
                writer.WriteLine("{");
                writer.WriteLine("return typeId;");
                writer.WriteLine("}");
                writer.WriteLine("");
            }

            writer.WriteLine($"return {MessageIdType}.MinValue;");
            writer.WriteLine("}");
            writer.WriteLine("");
            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine($"public override {MessageIdType} GetTypeIdGeneric<T>()");
            writer.WriteLine("{");
            writer.WriteLine("return GetTypeId(typeof(T));");
            writer.WriteLine("}");
            writer.WriteLine("");

            writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            writer.WriteLine("public override string GetTypeNameFromType<T>()");
            writer.WriteLine("{");
            writer.WriteLine("return GetTypeNameFromId(GetTypeId<T>());");
            writer.WriteLine("}");

            writer.WriteLine("}");
            writer.WriteLine("}");
            writer.WriteToFile(EditorUtility.UtilityFilePath);
        }

        private static void WriteInvalidClassName(CodeWriter writer, Type type, List<Type> typeList)
        {
            var index = typeList.IndexOf(type);

            for (var i = 0; i < index; ++i)
            {
                writer.Write('_');
            }

            writer.Write($"{EditorUtility.InvalidTypeIdentifier}_{type.Name}");
        }
    }
}

#endif