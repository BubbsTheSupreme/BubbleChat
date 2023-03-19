using System;
using System.IO;
using System.Text;


namespace BubbleChat.Packets;

public class MessagePacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public MessagePacket(ushort packetSize)
	{
		packetSize = (ushort)(packetSize + 4);
		buffer = new byte[packetSize];
		memoryStream = new MemoryStream(buffer);
		memoryStream.Seek(0, SeekOrigin.Begin);
		memoryStream.Write(BitConverter.GetBytes(packetSize));
	}

	public MessagePacket WriteFirstId(byte id)
	{
		memoryStream.Seek(2, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public MessagePacket WriteSecondId(byte id)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
		memoryStream.WriteByte(id);
		return this;
	}

	public MessagePacket WriteMessage(string message)
	{
		byte[] messageBytes = Encoding.ASCII.GetBytes(message); 
		memoryStream.Seek(4, SeekOrigin.Begin);
		memoryStream.Write(messageBytes);
		return this;
	}

	public byte[] Finalize()
	{
		memoryStream.Flush();
		memoryStream.Dispose();
		return buffer;
	}

}