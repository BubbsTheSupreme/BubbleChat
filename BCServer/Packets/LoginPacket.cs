using System;
using System.IO;
using System.Text;

namespace BubbleChat.Packets;

public class LoginPacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public LoginPacket(byte valueSize)
	{
		ushort packetSize = (ushort)(valueSize + 4);
		buffer = new byte[packetSize];
		memoryStream = new MemoryStream(buffer);
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(packetSize));
	}

	public LoginPacket WritePacketId(byte id)
	{
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public LoginPacket WriteValueId(byte length)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
		memoryStream.WriteByte(length);
		return this;
	}

	public LoginPacket WriteValue(string password)
	{
		byte[] buffer = Encoding.ASCII.GetBytes(password);
		memoryStream.Seek(4, SeekOrigin.Begin);
		memoryStream.Write(buffer);
		return this;
	}

	public byte[] Finalize()
	{
		memoryStream.Flush();
		memoryStream.Dispose();
		return buffer;
	}

}