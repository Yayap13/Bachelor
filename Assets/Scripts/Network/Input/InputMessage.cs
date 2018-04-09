using SmartNet;

namespace Network
{
	public class InputMessage : INetMessage
	{
		public int Time;
		public uint Keymap = 0u;
		
		public void OnSerialize(Writer writer)
		{
			writer.WriteCompressed(Time);
			writer.WriteCompressed(Keymap);
		}

		public void OnDeserialize(Reader reader)
		{
			Time = reader.ReadIntCompressed();
			Keymap = reader.ReadUIntCompressed();
		}
	}
}
