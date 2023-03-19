using System;
using System.Text;
using System.Net.Sockets;
using BubbleChat.Packets;
using BubbleChat.Client;
using Terminal.Gui;

namespace BubbleChat.Packets.Handler;

static class PacketRouter
{
	public static event Action<View> OnMessageReceived;
	public static event Action<string> OnLoginRequest;
	public static void Process(BubbleChatClient client, byte[] buffer)
	{
		byte packetId = buffer[0];
		switch(packetId)
		{
			case 0: //login status if SUCCESSFUL stay connected else get disconnected
				client.UserId = buffer[1];
				break;
			case 1:
				string message = Encoding.ASCII.GetString(buffer[2..]);
				OnLoginRequest?.Invoke(message.Replace("\0", ""));
				break;
			case 2:
				View messageReceived = new();
				messageReceived.Text = $"{Encoding.ASCII.GetString(buffer[2..])}"; 
				OnMessageReceived?.Invoke(messageReceived);
				break;
		}
	}
}