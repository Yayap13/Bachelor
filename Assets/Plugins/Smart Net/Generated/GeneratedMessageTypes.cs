// Generated code. Do not modify.

using System;
using System.Runtime.CompilerServices;
using SmartNet.Messages;
using System.Collections.Generic;

namespace SmartNet
{
	internal abstract class SmartNetInvalidBaseType : INetMessage
	{
		public void OnSerialize(Writer writer)
		{
		}
		public void OnDeserialize(Reader reader)
		{
		}
	}
	
	internal class SmartNetInvalidType_SnapshotMessage : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_StateSyncMessage : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_InputMessage : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_ColorCubeMessage : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_AssetSpawn : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_RemoveOwner : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_SetOwner : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_UpdateTime : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_IdentityMessage : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_OwnerSpawn : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_WorldDestroy : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_AddPlayer : SmartNetInvalidBaseType { }
	internal class SmartNetInvalidType_WorldSpawn : SmartNetInvalidBaseType { }

	public class GeneratedMessageTypes : MessageTypes
	{
		public static void Initialize()
		{
			if (Instance == null || Instance.GetType() != typeof(GeneratedMessageTypes))
			{
				SetMessageType(new GeneratedMessageTypes());
			}
		}

		public override uint MessageCount { get; } = 13;
		
		public override Type[] AllTypes { get; } =
		{
			Type.GetType("Gameplay.SnapshotMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SnapshotMessage),
			Type.GetType("Gameplay.StateSyncMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_StateSyncMessage),
			Type.GetType("Network.InputMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_InputMessage),
			Type.GetType("Network.ColorCubeMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_ColorCubeMessage),
			Type.GetType("SmartNet.Messages.AssetSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AssetSpawn),
			Type.GetType("SmartNet.Messages.RemoveOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_RemoveOwner),
			Type.GetType("SmartNet.Messages.SetOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SetOwner),
			Type.GetType("SmartNet.Messages.UpdateTime, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_UpdateTime),
			Type.GetType("SmartNet.Messages.IdentityMessage, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_IdentityMessage),
			Type.GetType("SmartNet.Messages.OwnerSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_OwnerSpawn),
			Type.GetType("SmartNet.Messages.WorldDestroy, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldDestroy),
			Type.GetType("SmartNet.Messages.AddPlayer, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AddPlayer),
			Type.GetType("SmartNet.Messages.WorldSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldSpawn),
		};

		public override string[] AllTypeNames { get; } =
		{
			"Gameplay.SnapshotMessage",
			"Gameplay.StateSyncMessage",
			"Network.InputMessage",
			"Network.ColorCubeMessage",
			"SmartNet.Messages.AssetSpawn",
			"SmartNet.Messages.RemoveOwner",
			"SmartNet.Messages.SetOwner",
			"SmartNet.Messages.UpdateTime",
			"SmartNet.Messages.IdentityMessage",
			"SmartNet.Messages.OwnerSpawn",
			"SmartNet.Messages.WorldDestroy",
			"SmartNet.Messages.AddPlayer",
			"SmartNet.Messages.WorldSpawn",
		};

		private static readonly Dictionary<uint, Type> MessageIdToType = new Dictionary<uint, Type>()
		{
			{ 36, Type.GetType("Gameplay.SnapshotMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SnapshotMessage) },
			{ 37, Type.GetType("Gameplay.StateSyncMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_StateSyncMessage) },
			{ 35, Type.GetType("Network.InputMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_InputMessage) },
			{ 34, Type.GetType("Network.ColorCubeMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_ColorCubeMessage) },
			{ 33, Type.GetType("SmartNet.Messages.AssetSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AssetSpawn) },
			{ 4, Type.GetType("SmartNet.Messages.RemoveOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_RemoveOwner) },
			{ 5, Type.GetType("SmartNet.Messages.SetOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SetOwner) },
			{ 6, Type.GetType("SmartNet.Messages.UpdateTime, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_UpdateTime) },
			{ 2, Type.GetType("SmartNet.Messages.IdentityMessage, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_IdentityMessage) },
			{ 3, Type.GetType("SmartNet.Messages.OwnerSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_OwnerSpawn) },
			{ 7, Type.GetType("SmartNet.Messages.WorldDestroy, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldDestroy) },
			{ 1, Type.GetType("SmartNet.Messages.AddPlayer, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AddPlayer) },
			{ 8, Type.GetType("SmartNet.Messages.WorldSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldSpawn) },
		};

		private static readonly Dictionary<Type, uint> TypeToMessageId = new Dictionary<Type, uint>()
		{
			{ Type.GetType("Gameplay.SnapshotMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SnapshotMessage), 36 },
			{ Type.GetType("Gameplay.StateSyncMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_StateSyncMessage), 37 },
			{ Type.GetType("Network.InputMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_InputMessage), 35 },
			{ Type.GetType("Network.ColorCubeMessage, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_ColorCubeMessage), 34 },
			{ Type.GetType("SmartNet.Messages.AssetSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AssetSpawn), 33 },
			{ Type.GetType("SmartNet.Messages.RemoveOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_RemoveOwner), 4 },
			{ Type.GetType("SmartNet.Messages.SetOwner, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_SetOwner), 5 },
			{ Type.GetType("SmartNet.Messages.UpdateTime, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_UpdateTime), 6 },
			{ Type.GetType("SmartNet.Messages.IdentityMessage, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_IdentityMessage), 2 },
			{ Type.GetType("SmartNet.Messages.OwnerSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_OwnerSpawn), 3 },
			{ Type.GetType("SmartNet.Messages.WorldDestroy, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldDestroy), 7 },
			{ Type.GetType("SmartNet.Messages.AddPlayer, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_AddPlayer), 1 },
			{ Type.GetType("SmartNet.Messages.WorldSpawn, SmartNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") ?? typeof(SmartNetInvalidType_WorldSpawn), 8 },
		};

		private static readonly Dictionary<uint, string> MessageIdToTypeName = new Dictionary<uint, string>()
		{
			{ 36, "Gameplay.SnapshotMessage" },
			{ 37, "Gameplay.StateSyncMessage" },
			{ 35, "Network.InputMessage" },
			{ 34, "Network.ColorCubeMessage" },
			{ 33, "SmartNet.Messages.AssetSpawn" },
			{ 4, "SmartNet.Messages.RemoveOwner" },
			{ 5, "SmartNet.Messages.SetOwner" },
			{ 6, "SmartNet.Messages.UpdateTime" },
			{ 2, "SmartNet.Messages.IdentityMessage" },
			{ 3, "SmartNet.Messages.OwnerSpawn" },
			{ 7, "SmartNet.Messages.WorldDestroy" },
			{ 1, "SmartNet.Messages.AddPlayer" },
			{ 8, "SmartNet.Messages.WorldSpawn" },
		};
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Type GetTypeFromId(uint typeId)
		{
			Type type;
			
			if (MessageIdToType.TryGetValue(typeId, out type))
			{
				return type;
			}
			
			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string GetTypeNameFromId(uint typeId)
		{
			string name;
			
			if(MessageIdToTypeName.TryGetValue(typeId, out name))
			{
				return name;
			}
			
			return "UnknownType";
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override uint GetTypeIdFromType(Type type)
		{
			uint typeId;
			
			if (TypeToMessageId.TryGetValue(type, out typeId))
			{
				return typeId;
			}
			
			return uint.MinValue;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override uint GetTypeIdGeneric<T>()
		{
			return GetTypeId(typeof(T));
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string GetTypeNameFromType<T>()
		{
			return GetTypeNameFromId(GetTypeId<T>());
		}
	}
}
