using Db4objects.Db4o;
using Db4objects.Db4o.Messaging;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
	public static int MAX_PLAYERS = 4;
	//public static Database db { get; set; }
	public static IObjectContainer mConnection;
	private static bool owner = false;

	private void Start()
	{
		DontDestroyOnLoad(this);
	}

	public static void ConnectToServer(string username)
	{
		LoginManager loginManager = new LoginManager(ServerConfiguration.Host, ServerConfiguration.Port);
		int playerId;
		IObjectContainer connection = loginManager.Login("player", out playerId);
		mConnection = connection;
		if (playerId <= -1)
		{
			//TODO: error while connecting, show UI error
			UnityEngine.Debug.Log("Error while connecting");
		}
		else
		{
			AddPlayer(username);
			//PrintAllPlayers();
			PlayerPrefs.SetString("username", username);
			SceneManager.LoadScene("Hub");
		}
	}
	
	public static void SendMessageToServer(string message)
	{
		IObjectContainer objectContainer = null;
		try
		{
			// connect to the server
			objectContainer = Db4oFactory.OpenClient(ServerConfiguration.Host, ServerConfiguration.Port, ServerConfiguration.User, ServerConfiguration.Password);
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

	public static void DisconnectFromServer()
	{
		if (mConnection != null)
		{
			mConnection.Close();
		}
	}

	public static void CloseServer()
	{
		SendMessageToServer("STOP");
	}

	public static void AddPlayer(string screenName)
	{
		Database.AddPlayer(new Player(screenName));
	}

	public static void PrintAllPlayers()
	{
		foreach (Player player in Database.FetchAllPlayers())
		{
			UnityEngine.Debug.Log(player);
		}
	}

	private void OnApplicationQuit()
	{
		DisconnectFromServer();
	}


}
