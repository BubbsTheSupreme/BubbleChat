using System;
using System.Text;
using System.Collections.Generic;
using BubbleChat.Config;
using BubbleChat.Server;
using BubbleChat.Logging;
using BubbleChat.Packets.Filtering;

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
						if (ServerConfig.Password.Equals(password))
						{
							Console.WriteLine($"[SERVER] {client.Client.AddressFamily} Password check successful!");
							Logger.Info($"[SERVER] {client.Client.AddressFamily} Password check successful!");
							UIDPacket packet = new();
							packet.WritePacketId(1).WriteUserId(0);
							client.Send(packet.Finalize());
						} else
						{
							Console.WriteLine($"[SERVER] {client.Client.AddressFamily} Password check failed..");
							Logger.Info($"[SERVER] {client.Client.AddressFamily} Password check failed..");
							UIDPacket packet = new();
							packet.WritePacketId(1).WriteUserId(1);
							client.Send(packet.Finalize());
							client.Close();
						} 
					break;
				}
				break;
			case 1:
				byte packetAuthorId = buffer[1];
				Console.WriteLine($"Packet Author ID: {packetAuthorId}, Client ID: {client.UserId}");
				if (client.UserId != packetAuthorId)
				{
					MessagePacket packet = new(3);
					packet.WriteSecondId(client.UserId).WriteMessage("[SERVER] YOU HAVE BEEN DISCONNECTED FOR PACKET IMPERSONATION");
					client.Send(packet.Finalize());
					client.Close();
					Logger.Info($"[FALSE PACKET ID] {client.Username} {client.UserId} {client.Client.AddressFamily} has been disconnected.");
				} else
				{
					string message = Encoding.ASCII.GetString(buffer[2..]);
					Tuple<string, bool, string> results = null;
					if (ServerFilter.FilterStatus == "yes")
					{
						results = ServerFilter.Filter(message);
						message = results.Item1;
					}
					if (ServerFilter.FilterStatus == "yes" && results.Item2 == true)
					{
						Logger.Info($"{client.Username} has been removed for usage of banned word or phrase.");
						MessagePacket packet = new(3);
						packet.WriteSecondId(client.UserId).WriteMessage("[SERVER] YOU HAVE BEEN DISCONNECTED FOR USAGE OF BANNED WORD OR PHRASE");
						client.Send(packet.Finalize());
						client.Close();
						Logger.Info($"[SERVER FILTER] {client.Username} {client.UserId} {client.Client.AddressFamily} has been disconnected due to usage of banned word or phrase");
						Logger.Info($"[SERVER FILTER] {client.Username}: {results.Item3}");
					} else 
					{
						Console.WriteLine($"[{client.Username}] {results.Item3}" ); // display original message
						Logger.Info($"[{client.Username}] {results.Item3}"); // log original message
						foreach (ClientConnection connection in client.Server.Connections)
						{				
							if (connection.UserId != packetAuthorId)
							{
								string new_message = $"[{client.Username}] {message}";
								MessagePacket packet = new(2);
								packet.WriteSecondId(packetAuthorId).WriteMessage(new_message);
								connection.Send(packet.Finalize());
							}
						}
					}
				}
				break;
		}
	}
}
