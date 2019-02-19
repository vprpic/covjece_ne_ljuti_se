using Db4objects.Db4o;
using System.Collections.Generic;

public class Database{

	private static Database instance;
	public static IObjectContainer mClient;

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
		//dbContainer = CreateDatabase();
	}

	public IObjectContainer CreateDatabase(string filePath)
	{
		return Db4oFactory.OpenFile(filePath);
	}

	/*
	public IObjectContainer AccessLocalServer() {
		const int RunEmbeddedServer = 0;
		var server =  Db4oFactory.OpenServer(FilePath, RunEmbeddedServer);
		return server.OpenClient();
	}
	*/

	public Database ConnectToRealServer(string username)
	{
		IObjectContainer client = Db4oFactory.OpenClient("localhost", 8080, "player", "db4o-password");
		mClient = client;
		return instance;
	}

	public void DisconnectFromServer()
	{
		if(mClient != null)
		{
			mClient.Close();
		}
	}

	public void AddPlayer(Player player)
	{
		mClient.Store(player);
		mClient.Commit();
	}
	
	public List<Player> FetchAllPlayers()
	{
		List<Player> players = new List<Player>();

		IObjectSet result = mClient.QueryByExample(new Player());

		while (result.HasNext())
		{
			players.Add((Player) result.Next());
		}

		return players;
	}

}
