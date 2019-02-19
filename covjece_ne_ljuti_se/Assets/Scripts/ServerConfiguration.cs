/// <summary>
/// Configuration used for StartServer and StopServer.
/// </summary>
public class ServerConfiguration
{
	/// <summary>
	/// the host to be used.
	/// If you want to run the client server examples on two computers,
	/// enter the computer name of the one that you want to use as server. 
	/// </summary>
	public const string Host = "localhost";

	/// <summary>
	/// the database file to be used by the server.
	/// </summary>
	public const string FileName = "serverdb.db4o";

	/// <summary>
	/// the port to be used by the server.
	/// </summary>
	public const int Port = 8080;

	/// <summary>
	/// the user name for access control.
	/// </summary>
	public const string User = "player";

	/// <summary>
	/// the pasword for access control.
	/// </summary>
	public const string Password = "db4o-password";
}
