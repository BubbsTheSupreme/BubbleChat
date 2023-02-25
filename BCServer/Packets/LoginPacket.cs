using System;
using System.IO;

namespace BubbleChat.Packets;

public class LoginPacket
{
	private MemoryStream memoryStream;
	private byte[] buffer;

	public LoginPacket(byte usernameSize, byte passwordSize)
	{
		ushort packetSize = (ushort)(usernameSize + passwordSize + 3);
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

	public LoginPacket WritePasswordLength(byte length)
	{
		memoryStream.Seek(3, SeekOrigin.Begin);
		memoryStream.WriteByte(length);
		return this;
	}

	public LoginPacket WriteUsernameLength(byte length)
	{
		memoryStream.Seek(4, SeekOrigin.Begin);
		memoryStream.WriteByte(length);
		return this;
	}

	public LoginPacket WritePassword(byte[] password)
	{
		memoryStream.Seek(5, SeekOrigin.Begin);
		memoryStream.Write(password);
		return this;
	}

	public LoginPacket WriteUsername(byte[] username)
	{
		memoryStream.Seek(25, SeekOrigin.Begin);
		memoryStream.Write(username);
		return this;
	}

	public byte[] Finalize()
	{
		memoryStream.Flush();
		memoryStream.Dispose();
		return buffer;
	}

}