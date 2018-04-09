using SmartNet;
using UnityEngine;

namespace Network
{
	public class NetColoredCube : MonoBehaviour, IPreStart, IStartServer, IStopServer, IStartClient, IStopClient
	{
		private SmartNetIdentity _identity;
		private Renderer _renderer;

		public void OnPreStart()
		{
			Debug.Log("OnPreStart");
			_identity = GetComponent<SmartNetIdentity>();
			Debug.Assert(_identity!=null);
			_renderer = GetComponent<Renderer>();
		}
		
		public void OnStartServer()
		{
			Debug.Log("OnStartServer!");
		}

		public void OnStopServer()
		{
			Debug.Log("OnStopServer");
		}

		public void OnStartClient()
		{
			Debug.Log("OnStartClient");
			//_identity.RegisterHandler<ColorCubeMessage>(handler =>
			NetworkClient.RegisterHandler<ColorCubeMessage>(info =>
			{
				_renderer.material.color = info.Message.color;
			});
		}

		public void OnStopClient()
		{
			Debug.Log("OnStopClient");
		}

		private void Update()
		{
			if (_identity.IsServer && Input.GetKeyDown(KeyCode.Space))
			{
				_renderer.material.color = Random.ColorHSV();
				NetworkServer.SendToAll(new ColorCubeMessage(_renderer.material.color), 1);
			}
		}
	}

	public struct ColorCubeMessage : INetMessage
	{
		public Color color { private set; get; }

		public ColorCubeMessage(Color color)
		{
			this.color = color;
		}

		public void OnSerialize(Writer writer)
		{
			writer.WriteCompressed(color.r);
			writer.WriteCompressed(color.g);
			writer.WriteCompressed(color.b);
			writer.WriteCompressed(color.a);
		}

		public void OnDeserialize(Reader reader)
		{
			color = new Color(reader.ReadFloatCompressed(), reader.ReadFloatCompressed(), reader.ReadFloatCompressed(), reader.ReadFloatCompressed());
		}
	}
}
