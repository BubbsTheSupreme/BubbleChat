using System;
using System.Text;
using BubbleChat.Config;
using BubbleChat.Server;
using BubbleChat.Packets;

namespace BubbleChat.Packets.Handler;

public static class PacketRouter 
{
	public static void Process(ClientConnection client, byte[] buffer)
	{
		byte packetId = buffer[0];
		switch(packetId)
		{
			case 0:
				byte valueId = buffer[1];
				switch (valueId)
				{
					case 0:
						string username = Encoding.ASCII.GetString(buffer[4..]);
						client.Username = username;
						UIDPacket uid = new();
						uid.WritePacketId(0).WriteUserId(client.UserId);
						client.Send(uid.Finalize());
						break;
					case 1:
						string password = Encoding.ASCII.GetString(buffer[4..]);
						Console.WriteLine(password);
						foreach (char c in password.ToCharArray())
						{
							if (c == '\0') Console.WriteLine("\\0");
							else Console.WriteLine(c);
						}
						if (ServerConfig.Password.Equals(password.Trim('\0')))
						{
							Console.WriteLine("[SERVER] Password check successful!");
							UIDPacket packet = new();
							packet.WritePacketId(1).WriteUserId(0);
							client.Send(packet.Finalize());
						} else
						{
							Console.WriteLine("[SERVER] Password check failed..");
							UIDPacket packet = new();
							packet.WritePacketId(1).WriteUserId(1);
							client.Send(packet.Finalize());
						} 
					break;
				}
				break;
			case 1:
				Console.WriteLine($"[{client.Username}] {Encoding.ASCII.GetString(buffer[4..])}");
				foreach (ClientConnection connection in client.Server.Connections)
				if (connection.UserId != buffer[1])
				{
					string new_message = $"[{client.Username}] {Encoding.ASCII.GetString(buffer[4..])}";
					MessagePacket packet = new(2);
					packet.WriteSecondId(buffer[1]).WriteMessage(new_message);
					connection.Send(packet.Finalize());
				}
			break;
		}
	}
}
