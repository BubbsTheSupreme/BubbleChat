using System;
using System.IO;
using System.Text;


namespace BubbleChat.Packets;

public class LoginPacket
{
	private MemoryStream memoryStream;

	public LoginPacket(byte packetId)
	{
		memoryStream = new MemoryStream();
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(packetId);
	}

	public LoginPacket WriteValueId(byte id)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public LoginPacket WriteValue(string message)
	{
		byte[] messageBytes = Encoding.ASCII.GetBytes(message); 
		memoryStream.Seek(4, SeekOrigin.Begin);
		memoryStream.Write(messageBytes);
		return this;
	}

	public byte[] Finalize()
	{
		ushort size = (ushort)memoryStream.Position;
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(size));
		byte[] buffer = memoryStream.ToArray();
		memoryStream.Dispose();
		return buffer;
	}

}