using Db4objects.Db4o;
using System;

class LoginManager
{
	string _host;
	int _port;
	static int MAXIMUM_USERS = 4;
	public LoginManager(String host, int port)
	{
		_host = host;
		_port = port;
	}
	public IObjectContainer Login(String username, out int id, String password = "db4o-password")
	{
		IObjectContainer objectContainer;
		id = -1;
		try
		{
			objectContainer = Db4oFactory.OpenClient(_host, _port, username, password);
		}
		catch (System.IO.IOException e)
		{
			UnityEngine.Debug.Log(e.Message);
			return null;
		}
		bool allowedToLogin = false;
		for (int i = 0; i < MAXIMUM_USERS; i++)
		{
			String semaphore = "login_limit_" + (i + 1);
			if (objectContainer.Ext().SetSemaphore(semaphore, 0))
			{
				allowedToLogin = true;
				UnityEngine.Debug.Log("Logged in as " + username);
				UnityEngine.Debug.Log("Acquired semaphore " + semaphore);
				id = i+1;
				break;
			}
		}
		if (!allowedToLogin)
		{
			UnityEngine.Debug.Log("Login not allowed for " + username + ": max clients exceeded");
			objectContainer.Close();
			return null;
		}
		return objectContainer;
	}
}