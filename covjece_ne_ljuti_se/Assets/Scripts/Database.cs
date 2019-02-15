using Db4objects.Db4o;

public class Database{
	
	public static string FilePath = "C:\\Users\\maggi\\Documents\\covjece_ne_ljuti_se\\covjece_ne_ljuti_se\\Assets\\db\\tempdb.yap";

	public IObjectContainer dbContainer;

	public Database()
	{
		dbContainer = CreateDatabase();
	}

	public IObjectContainer CreateDatabase()
	{
		return Db4oFactory.OpenFile(FilePath);
	}

	public IObjectContainer AccessLocalServer() {
		const int RunEmbeddedServer = 0;
		var server =  Db4oFactory.OpenServer(FilePath, RunEmbeddedServer);
		return server.OpenClient();
	}

	public IObjectServer RunAsRealServer() {
		const int RunAsRealServer = 8080;
		var server = Db4oFactory.OpenServer(FilePath, RunAsRealServer);
		server.GrantAccess("player", "db4o-password");
		return server;
	}

	public IObjectContainer ConnectToRealServer(string username)
	{
		using (var db = Db4oFactory.OpenClient("localhost", 0, username, "db4o-password"))
		{
			return db;
		}
	}

}
