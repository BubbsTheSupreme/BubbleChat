using System;
using System.Xml;
using System.Collections.Generic;


namespace BubbleChat.Config;

public static class ServerConfig
{
	public static string PasswordStatus;
	public static string Password;
	public static ushort Port;
	public static string LogFilePath;
	public static string AccessListStatus;
	public static Dictionary<string, string> Admins = new Dictionary<string, string>();
	public static List<string> AccessList = new List<string>();
	public static XmlDocument XmlConfig = new();

	public static void InitConfig(string path)
	{
		Console.WriteLine("Loading config.xml data...");
		XmlConfig.Load(path);

		var password = XmlConfig.SelectSingleNode("//config/password");
		var port = XmlConfig.SelectSingleNode("//config/port");
		var logFilePath = XmlConfig.SelectSingleNode("//config/logfilepath");
		var admins = XmlConfig.SelectSingleNode("//config/admins");
		var accessList = XmlConfig.SelectSingleNode("//config/accesslist");

		Password = password.Attributes["password"].InnerText;
		PasswordStatus = password.Attributes["status"].InnerText;
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