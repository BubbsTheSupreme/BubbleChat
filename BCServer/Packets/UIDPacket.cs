using System;
using System.IO;
using System.Text;

namespace BubbleChat.Packets;

public class UIDPacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public UIDPacket()
	{
		ushort packetSize = 4;
		buffer = new byte[packetSize];
		memoryStream = new MemoryStream(buffer);
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(packetSize));
	}

	public UIDPacket WritePacketId(byte id)
	{
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public UIDPacket WriteUserId(byte id)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
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