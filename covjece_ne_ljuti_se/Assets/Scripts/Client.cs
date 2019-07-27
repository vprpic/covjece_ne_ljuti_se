using Db4objects.Db4o;
using Db4objects.Db4o.Messaging;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
	public static int MAX_PLAYERS = 4;
	//public static Database db { get; set; }
	public static IObjectContainer mConnection;
	public static Player currentPlayer;
	private static bool owner = false;
	private static GameConfiguration gameConfig;

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
			currentPlayer = new Player(username);
			AddPlayer(currentPlayer);
			//PrintAllPlayers();
			PlayerPrefs.SetString("username", username);
		}
		const int timeOutInMilliSec = 1000;
		try
		{
			mConnection.Ext().SetSemaphore("game_config", timeOutInMilliSec);

			IObjectSet gmcf = mConnection.QueryByExample(new GameConfiguration());
			if (gmcf.HasNext())
			{
				gameConfig = (GameConfiguration)gmcf.Next();
			}
			else
			{
				gameConfig = new GameConfiguration();
				mConnection.Store(gameConfig);
				mConnection.Commit();
			}
		}
		finally
		{
			mConnection.Ext().ReleaseSemaphore("game_config");
		}
	}

	internal static void RegisterThePlayerAsReady()
	{
		currentPlayer.Ready = true;
		mConnection.Store(currentPlayer);
		mConnection.Commit();
		UnityEngine.Debug.Log("Set player " + currentPlayer.ScreenName + " as ready.");
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
			//Database.
			Database.RemoveCurrentPlayer(currentPlayer);
			mConnection.Close();
		}
	}

	public static void CloseServer()
	{
		SendMessageToServer("STOP");
	}

	public static void AddPlayer(Player player)
	{
		Database.AddPlayer(player);
	}

	public static void RemoveCurrentPlayer()
	{
		if (currentPlayer == null)
			return;
		Database.RemoveCurrentPlayer(currentPlayer);
		currentPlayer = null;
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
