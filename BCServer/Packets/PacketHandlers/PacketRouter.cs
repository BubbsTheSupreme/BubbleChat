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
						string username = Encoding.ASCII.GetString(buffer[2..]);
						client.Username = username;
						UIDPacket uid = new();
						uid.WritePacketId(0).WriteUserId(client.UserId);
						client.Send(uid.Finalize());
						break;
					case 1:
						string password = Encoding.ASCII.GetString(buffer[2..]);
						if (ServerConfig.Password.Equals(password.Replace("\0", "")))
						{
							Console.WriteLine("[SERVER] Password check successful!");
							string message = "PASSED";
							MessagePacket responsePacket = new((ushort)message.Length);
							responsePacket.WriteFirstId(1).WriteSecondId(0).WriteMessage(message);
							client.Send(responsePacket.Finalize());
						} else
						{
							Console.WriteLine("[SERVER] Password check failed..");
							string message = "FAILED";
							MessagePacket responsePacket = new((ushort)message.Length);
							responsePacket.WriteFirstId(1).WriteSecondId(0).WriteMessage(message);
							client.Send(responsePacket.Finalize());
						} 
					break;
				}
				break;
			case 1:
				Console.WriteLine($"[{client.Username}] {Encoding.ASCII.GetString(buffer[2..])}");
				foreach (ClientConnection connection in client.Server.Connections)
				if (connection.UserId != buffer[1])
				{
					string new_message = $"[{client.Username}] {Encoding.ASCII.GetString(buffer[2..])}";
					MessagePacket packet = new((ushort)new_message.Length);
					packet.WriteFirstId(1).WriteSecondId(buffer[1]).WriteMessage(new_message);
					connection.Send(packet.Finalize());
				}
			break;
		}
	}
}
