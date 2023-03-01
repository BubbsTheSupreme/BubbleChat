using System;
using System.Text;
using Terminal.Gui;
using BubbleChat.Client;
using BubbleChat.Packets;
using BubbleChat.Packets.Handler;
using System.Collections.Generic;


namespace BubbleChat.UI;

public class UserInterface
{
	Dictionary<string, Window> windows = new();
	BubbleChatClient Client;
	Toplevel top;
	ListView messagesContainer;
	List<string> messages;

	public UserInterface() //Messy Boilerplate
	{
		Client = new();
		PacketRouter.OnMessageReceived += ChatUpdate;
		BubbleChatClient.OnDisconnect += ErrorMessage;

		Application.Init();
		top  = Application.Top;

		windows.Add("Connect", ConnectScreen());
		windows.Add("Chat", ChatScreen());

		MenuBar menu = new(new MenuBarItem [] 
		{
			new MenuBarItem("Client", new MenuItem [] 
			{
				new MenuItem("_Quit", "", () => {
					if (Client.IsConnected() == true)
						Client.Disconnect();
					top.Running = false;
				}),
				new MenuItem("_Disconnect", "", () => 
				{
					if (Client.IsConnected() == true)
						Client.Disconnect();
					windows["Chat"].Visible = false;
					windows["Connect"].Visible = true;
				})

			})
		});

		windows["Chat"].Visible = false;
		windows["Connect"].Visible = true;

		top.Add(menu);
		top.Add(windows["Chat"]);
		top.Add(windows["Connect"]);
		Application.Run();
	}

	public Window ChatScreen()
	{
		var win = new Window("BubbleChat") 
		{
			X = 0,
			Y = 1,
			Width = Dim.Fill(),
			Height = Dim.Fill(),
		};

		messagesContainer = new() 
		{
			X = 0,
			Y = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill(),
		};
		win.Add(messagesContainer);
		
		messagesContainer.SetSource(messages);

		return win;
	}
	public void ChatUpdate(View message)
	{
		try
		{
			Application.MainLoop.Invoke(() =>
			{
				messagesContainer.Add(message);
				messagesContainer.SetNeedsDisplay();
			});
		}
		catch (Exception e)
		{
			top.Running = false;
			Console.WriteLine(e);
			Client.Disconnect();
		}
	}

	public Window ConnectScreen()
	{
		
		var win = new Window ("BubbleChat") {
			X = 0,
			Y = 1,
			Width = Dim.Fill(),
			Height = Dim.Fill(),
		};

		var label = new Label(3, 2, "Username: ");
		win.Add(label);
		var username = new TextField(16, 2, 40, "");
		win.Add(username);
		var label2 = new Label(3, 4, "Password: ");
		win.Add(label2);
		var password = new TextField(16, 4, 40, "");
		win.Add(password);
		var label3 = new Label(3, 6, "Server IP: ");
		win.Add(label3);
		var serverIp = new TextField(16, 6, 40, "");
		win.Add(serverIp);
		var label4 = new Label(3, 8, "Server Port: ");
		win.Add(label4);
		var serverPort = new TextField(16, 8, 40, "");
		win.Add(serverPort);
		var button = new Button("Connect") {
            X = 3,
            Y = 14
        };
		win.Add(button);

		button.Clicked += () => {
			Client.Connect(ushort.Parse(serverPort.Text.ToString()), serverIp.Text.ToString());

			windows["Chat"].Visible = true; 
			windows["Connect"].Visible = false;
			Application.Refresh();
			
			MessagePacket packet = new(3);
			packet.WritePacketId(0).WriteMessage("SYN");
			Client.Send(packet.Finalize());
		};

		return win;
	}


	public void ErrorMessage(string message)
	{
		MessageBox.Query("Error Occurred", message, "OK");
	}
}