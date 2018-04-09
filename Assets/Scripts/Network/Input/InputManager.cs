using System.Collections.Generic;
using SmartNet;
using UnityEngine;

namespace Network
{
	public class InputManager : MonoBehaviour, IStartClient, IStartServer {
		[SerializeField] private int timeStepMilliseconds = 1000 / 20;
		
		private FixedUpdater _fixedUpdater;
		private readonly InputMessage _inputMessage = new InputMessage();
		private int _lastInputTime = 0;

		
		public void OnStartClient()
		{
			_fixedUpdater = new FixedUpdater((int) (NetworkTime.Milliseconds), timeStepMilliseconds, time =>
			{
				_inputMessage.Time = time;
				NetworkClient.Send(_inputMessage, 1);
				NetworkInput.AddKeyMap(_inputMessage.Keymap);
				_inputMessage.Keymap = 0u;
			});
		}
		
		public void OnStartServer ()
		{
			NetworkServer.NewConnectionEvent += NetworkInput.AddConnection;

			NetworkServer.ConnectionDisconnectEvent += NetworkInput.RemoveConnection;
			
			NetworkServer.RegisterHandler<InputMessage>(data =>
			{
//				Debug.Log($"Time: {data.Message.Time}, Keys: {data.Message.Keymap}");
				if (_lastInputTime < data.Message.Time)
				{
					_lastInputTime = data.Message.Time;
					NetworkInput.AddKeyMap(data.Connection, data.Message.Keymap);
				}
			});
		}
	
		public void Update ()
		{
			_fixedUpdater?.Update(NetworkTime.Milliseconds);
			
			if (NetworkServer.Active)
				return;
			
			if (Input.GetKey(KeyCode.W))
			{
				_inputMessage.Keymap |= (uint)Keys.MoveForward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				_inputMessage.Keymap |= (uint)Keys.MoveBackward;
			}
			if (Input.GetKey(KeyCode.A))
			{
				_inputMessage.Keymap |= (uint)Keys.MoveLeft;
			}
			if (Input.GetKey(KeyCode.D))
			{
				_inputMessage.Keymap |= (uint)Keys.MoveRight;	
			}
			if (Input.GetKey(KeyCode.F))
			{
				_inputMessage.Keymap |= (uint)Keys.Use;
			}
			if (Input.GetKey(KeyCode.G))
			{
				_inputMessage.Keymap |= (uint)Keys.Push;
			}
			if (Input.GetKey(KeyCode.H))
			{
				_inputMessage.Keymap |= (uint)Keys.Pull;
			}
		}
	}
}
