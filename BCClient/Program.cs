using System;
using System.Text;
using BubbleChat.Client;
using BubbleChat.Packets;

namespace BubbleChat;
class Program
{
	static void Main(string[] args)
	{
		BubbleChatClient client = new(12345, "192.168.1.114");
		client.Connect();
		string test = "SYN";
		MessagePacket packet = new((ushort)test.Length);
		packet.WritePacketId(0).WriteMessage(Encoding.ASCII.GetBytes(test));
		client.Send(packet.Finalize());
	}
}
