﻿using Db4objects.Db4o;
using Db4objects.Db4o.Messaging;
using System;

public class Client : ServerConfiguration
{
	public static Database db { get; set; }
	private static bool owner = false;

	public static Database ConnectToServer(string username)
	{
		db = Database.Instance;
		db.ConnectToRealServer(username);
		return db;
	}
	
	public static void SendMessageToServer(string message)
	{
		IObjectContainer objectContainer = null;
		try
		{
			// connect to the server
			objectContainer = Db4oFactory.OpenClient(Host, Port, User, Password);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}

		if (objectContainer != null)
		{
			// get the messageSender for the IObjectContainer 
			IMessageSender messageSender = objectContainer.Ext()
				.Configure().ClientServer().GetMessageSender();

			// send an instance of a StopServer object
			messageSender.Send(message);

			// close the IObjectContainer
			objectContainer.Close();
		}
	}

	public static void CloseServer()
	{
		SendMessageToServer("STOP");
	}

	public static void AddPlayer(int id, string screenName)
	{
		db.AddPlayer(new Player(id, screenName));
	}

	public static void PrintAllPlayers()
	{
		foreach (Player player in db.FetchAllPlayers())
		{
			UnityEngine.Debug.Log(player);
		}
	}


}
