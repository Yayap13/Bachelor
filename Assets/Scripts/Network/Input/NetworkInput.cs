using System;
using System.Collections.Generic;
using SmartNet;

namespace Network
{
	public class NetworkInput
	{
		private uint _lastKeymap = 0u;
		private uint _previousKeymap = 0u;
		private uint _isCurrentlyDownKeymap = 0u;
		
		private static readonly Dictionary<Connection, NetworkInput> ConnexionToNetworkInput = new Dictionary<Connection, NetworkInput>();
		private static readonly NetworkInput Me = new NetworkInput();
		
		public static void AddConnection(Connection connection)
		{
			ConnexionToNetworkInput.Add(connection, new NetworkInput());
		}
		public static void RemoveConnection(Connection connection)
		{
			ConnexionToNetworkInput.Remove(connection);
		}
		public static void AddKeyMap(Connection connection, uint keymap)
		{
			ConnexionToNetworkInput[connection].AddKeymap(keymap);
		}
		public static void AddKeyMap(uint keymap)
		{
			Me.AddKeymap(keymap);
		}
		
		
		public static bool GetKey(Connection connection, Keys key)
		{
			return ConnexionToNetworkInput[connection].GetKeyValue(key);
		}
		public static bool GetKey(Keys key)
		{
			return Me.GetKeyValue(key);
		}
		
		public static bool GetKeyDown(Connection connection, Keys key)
		{
			return ConnexionToNetworkInput[connection].GetKeyDownValue(key);
		}
		public static bool GetKeyDown(Keys key)
		{
			return Me.GetKeyDownValue(key);
		}
		
		public static bool GetKeyUp(Connection connection, Keys key)
		{
			return ConnexionToNetworkInput[connection].GetKeyUpValue(key);
		}
		public static bool GetKeyUp(Keys key)
		{
			return Me.GetKeyUpValue(key);
		}
		
	
		private void AddKeymap(uint keymap)
		{
			_previousKeymap = _lastKeymap;
			_lastKeymap = keymap;
		}
		
		private bool GetKeyValue(Keys key)
		{
			return (_lastKeymap & (uint)key) != 0;
		}
		private bool GetKeyDownValue(Keys key)
		{
			if (((_lastKeymap & (uint)key) != 0) && ((_previousKeymap & (uint)key) == 0))
			{
				if ((_isCurrentlyDownKeymap & (uint) key) == 0)
				{
					_previousKeymap |= (uint) key;
					return true;
				}
			}
			return false;
		}
		private bool GetKeyUpValue(Keys key)
		{
			if (((_lastKeymap & (uint) key) == 0) && ((_previousKeymap & (uint) key) != 0))
			{
				_previousKeymap &= ~((uint) key); // Clear that bit. (Replace bit at that position with 0)
				return true;
			}
			return false;
		}
	}
	
	[Flags]
	public enum Keys : uint //Can have a maximum of 32 different inputs
	{
		None = 0u, // 1
		MoveForward = 1u, // 2
		MoveBackward = 2u, // 3
		MoveLeft = 4u, // 4
		MoveRight = 8u, // 5
		MoveJump = 16u, // 6
		Use = 32u, // 7
		Push = 64u, // 8
		Pull = 128u // 9
	}
}
