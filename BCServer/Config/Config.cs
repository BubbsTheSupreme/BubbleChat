using System;
using System.Xml;
using System.Collections.Generic;


namespace BubbleChat.Config;

public static class ServerConfig
{
	private static string PasswordStatus;
	private static string Password;
	private static ushort UserCount;
	private static ushort Port;
	private static string LogFilePath;
	private static string AccessListStatus;
	private static Dictionary<string, string> Admins = new Dictionary<string, string>();
	private static List<string> AccessList = new List<string>();
	private static XmlDocument XmlConfig = new();

	public static void InitConfig(string path)
	{
		Console.WriteLine("Loading config.xml data...");
		XmlConfig.Load(path);

		var password = XmlConfig.SelectSingleNode("//config/password");
		var userCount = XmlConfig.SelectSingleNode("//config/usercount");
		var port = XmlConfig.SelectSingleNode("//config/port");
		var logFilePath = XmlConfig.SelectSingleNode("//config/logfilepath");
		var admins = XmlConfig.SelectSingleNode("//config/admins");
		var accessList = XmlConfig.SelectSingleNode("//config/accesslist");

		Password = password.InnerText;
		PasswordStatus = password.Attributes["status"].InnerText;
		UserCount = (ushort)Int16.Parse(userCount.InnerText);
		Port = (ushort)Int16.Parse(port.InnerText);
		LogFilePath = logFilePath.InnerText;
		AccessListStatus = accessList.Attributes["type"].InnerText;
		foreach (XmlNode node in accessList.ChildNodes)
		{
			AccessList.Add(node.InnerText);
		}
		foreach (XmlNode node in admins)
		{
			Admins.Add(
				admins.Attributes["username"].InnerText, 
				admins.Attributes["ip"].InnerText
			);
		}
	}
}