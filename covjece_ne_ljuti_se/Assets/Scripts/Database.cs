using Db4objects.Db4o;
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
		Client.mConnection.Store(player);
		Client.mConnection.Commit();
	}
	
	public static List<Player> FetchAllPlayers()
	{
		List<Player> players = new List<Player>();

		IObjectSet result = Client.mConnection.QueryByExample(new Player());

		while (result.HasNext())
		{
			players.Add((Player) result.Next());
		}

		return players;
	}

}
