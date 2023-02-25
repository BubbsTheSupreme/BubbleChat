using System;
using System.IO;

namespace BubbleChat.Packets;

public class DisconnectPacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public DisconnectPacket(ushort packetSize)
	{
		packetSize = (ushort)(packetSize + 2);
		buffer = new byte[packetSize];
		memoryStream = new MemoryStream(buffer);
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(packetSize));
	}

	public DisconnectPacket WritePacketId(byte id)
	{
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public byte[] Finalize()
	{
		memoryStream.Flush();
		memoryStream.Dispose();
		return buffer;
	}

}