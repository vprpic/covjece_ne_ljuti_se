using Db4objects.Db4o;
using System;
using System.Collections.Generic;

public class Database{

	private static Database instance;

	public static Database Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Database();
			}
			return instance;
		}
	}

	public Database()
	{
	}

	public IObjectContainer CreateDatabase(string filePath)
	{
		return Db4oFactory.OpenFile(filePath);
	}

	internal static void UpdatePawnData(PawnData pdata)
	{
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("pawnData", timeOutInMilliSec);
			IObjectSet result = Client.mConnection.QueryByExample(new PawnData(pdata.id, pdata.ownerId));
			PawnData data;
			if (result.Count <= 0)
			{
				data = pdata;
			}
			else
			{
				data = (PawnData)result.Next();
			}
			data.currentPosId = pdata.currentPosId;
			data.ownerId = pdata.ownerId;
			Client.mConnection.Ext().Store(pdata, int.MaxValue);
			Client.mConnection.Commit();
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("pawnData");
		}
	}

	/*
	public IObjectContainer AccessLocalServer() {
		const int RunEmbeddedServer = 0;
		var server =  Db4oFactory.OpenServer(FilePath, RunEmbeddedServer);
		return server.OpenClient();
	}
	*/
	/*
	public Database ConnectToRealServer(string username)
	{
		IObjectContainer client = Db4oFactory.OpenClient(ServerConfiguration.Host, ServerConfiguration.Port, "player", "db4o-password");
		//mClient = client;
		return instance;
	}*/

	public static void AddPlayer(Player player)
	{
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("players", timeOutInMilliSec);
			Client.mConnection.Ext().Store(player, int.MaxValue);
			Commit();
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("players");
		}
		
	}

	public static void Commit()
	{
		Client.mConnection.Commit();
	}

	public static GameConfiguration FetchGameConfig()
	{
		GameConfiguration gameConfig = null;
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("gameConfig", timeOutInMilliSec);
			IObjectSet result = Client.mConnection.QueryByExample(new GameConfiguration());
			GameConfiguration gc = (GameConfiguration)result.Next();
			Client.mConnection.Ext().Refresh(gc, int.MaxValue);
			gameConfig = gc;
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("gameConfig");
		}
		return gameConfig;
	}

	public static void UpdateGameConfig(GameConfiguration gc)
	{
		GameConfiguration gcTemp = null;
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("gameConfig", timeOutInMilliSec);
			IObjectSet result = Client.mConnection.QueryByExample(new GameConfiguration());
			if (result.HasNext())
			{
				gcTemp = (GameConfiguration)result.Next();
				gcTemp.CurrentTurn = gc.CurrentTurn;
				gcTemp.IsRunning = gc.IsRunning;
				gcTemp.NumOfPlayers = gc.NumOfPlayers;
			}
			else
			{
				gcTemp = gc;
			}
			Client.mConnection.Ext().Store(gc, int.MaxValue);
			Client.mConnection.Commit();
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("gameConfig");
		}
	}

	public static void RemoveCurrentPlayer(Player currentPlayer)
	{
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("players", timeOutInMilliSec);
			Player player = (Player) Client.mConnection.QueryByExample(currentPlayer).Next();
			Db4oFactory.Configure().ObjectClass(typeof(Player)).CascadeOnDelete(true);
			Client.mConnection.Delete(player);
			Client.mConnection.Commit();
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("players");
		}
		
	}

	public static List<Player> FetchAllPlayers()
	{
		List<Player> players = new List<Player>();

		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("players", timeOutInMilliSec);
			IObjectSet result = Client.mConnection.QueryByExample(new Player());
			while (result.HasNext())
			{
				Player p = (Player) result.Next();
				Client.mConnection.Ext().Refresh(p, int.MaxValue);
				players.Add(p);
			}
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("players");
		}

		return players;
	}

	internal static List<PawnData> FetchAllPawnDatas()
	{
		List<PawnData> pawnDatas = new List<PawnData>();
		const int timeOutInMilliSec = 1000;
		try
		{
			Client.mConnection.Ext().SetSemaphore("pawnData", timeOutInMilliSec);
			IObjectSet result = Client.mConnection.QueryByExample(new PawnData());
			if(result.Count <= 0)
			{
				return null;
			}
			while (result.HasNext())
			{
				PawnData p = (PawnData)result.Next();
				Client.mConnection.Ext().Refresh(p, int.MaxValue);
				pawnDatas.Add(p);
			}
		}
		finally
		{
			Client.mConnection.Ext().ReleaseSemaphore("pawnData");
		}
		return pawnDatas;
	}
}
