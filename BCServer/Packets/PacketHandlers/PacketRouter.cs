using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using BubbleChat.Server;
using BubbleChat.Packets;

namespace BubbleChat.Packets.Handler;

static class PacketRouter
{
	public static void Process(ClientConnection client, byte[] buffer)
	{
		byte packetId = buffer[0];
		switch(packetId)
		{
			case 0:
				Console.WriteLine($"[{client.ClientIp}] {Encoding.ASCII.GetString(buffer[1..])}");
				string ack = "ACK";
				MessagePacket packet = new((ushort)ack.Length);
				packet.WritePacketId(0).WriteMessage(Encoding.ASCII.GetBytes("ACK"));
				client.Send(packet.Finalize());
				break;
			case 1:
			break;
			case 2:
			break;
		}
	}
}