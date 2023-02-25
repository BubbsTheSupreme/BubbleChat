using System;
using System.IO;

namespace BubbleChat.Packets;

public class MessagePacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public MessagePacket(ushort packetSize)
	{
		packetSize = (ushort)(packetSize + 3);
		buffer = new byte[packetSize];
		memoryStream = new MemoryStream(buffer);
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(packetSize));
	}

	public MessagePacket WritePacketId(byte id)
	{
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public MessagePacket WritePassword(byte[] password)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
		memoryStream.Write(password);
		return this;
	}

	public byte[] Finalize()
	{
		memoryStream.Flush();
		memoryStream.Dispose();
		return buffer;
	}

}