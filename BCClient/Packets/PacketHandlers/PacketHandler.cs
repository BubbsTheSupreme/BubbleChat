using System;
using System.Text;
using System.Net.Sockets;
using BubbleChat.Packets;
using Terminal.Gui;

namespace BubbleChat.Packets.Handler;

static class PacketRouter
{
	public static event Action<View> OnMessageReceived;
	public static void Process(Socket socket, byte[] buffer)
	{
		byte packetId = buffer[0];
		switch(packetId)
		{
			case 0:
				View text = new();
				text.Text = $"{Encoding.ASCII.GetString(buffer[1..])}";
				OnMessageReceived?.Invoke(text);

				string ack = "[CLIENT] ACK";
				MessagePacket packet = new((ushort)ack.Length);
				packet.WritePacketId(0).WriteMessage(ack);
				socket.Send(packet.Finalize());
				break;
			case 1:
			break;
			case 2:
			break;
		}
	}
}