using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Query;
using System.Collections;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.TA;

namespace consoleServer
{
	class Program : ServerConfiguration, IMessageRecipient
	{
		private static Program instance;
		private static IObjectServer server;
		private static IObjectContainer client;
		/// <summary>
		/// setting the value to true denotes that the server should be closed
		/// </summary>
		private bool stop = false;
		
		/// <summary>
		/// starts a db4o server using the configuration from
		/// ServerConfiguration.
		/// </summary>
		static void Main(string[] args)
		{
			instance = new Program();
			Thread t = new Thread(new ThreadStart(instance.RunServer));
			
			t.Start();

			Console.WriteLine("The server is listening to your commands...");
			string input;
			while (!instance.stop)
			{
				input = Console.ReadLine();
				instance.ProcessCommandLineCommand(input);
			}
			Console.WriteLine("The server stopped listening to your commands.");
		}

		private void ProcessCommandLineCommand(string command)
		{
			string[] splitCommand;
			splitCommand = command.Split(' ');
			switch (splitCommand[0].ToUpper())
			{
				case "STOP":
					instance.Close();
					break;
				case "EXIT":
					instance.Close();
					break;
				case "PRINTALL":
					instance.PrintAll();
					break;
				case "KILLALL":
					instance.KillEverything();
					break;
				default:
					Console.WriteLine("I did not understand the command: " + splitCommand[0]);
					break;
			}
		}

		private void KillEverything()
		{
			bool empty = true;
			try
			{
				IQuery query = client.Query();
				IEnumerable allObjects = query.Execute();

				foreach (Object item in allObjects)
				{
					GenericObject dbObject = (GenericObject)item;
					if (dbObject.GetGenericClass().GetName().ToLower().Contains("player"))
					{
						IReflectField screenNameField = dbObject.GetGenericClass().GetDeclaredField("<ScreenName>k__BackingField");
						Console.WriteLine("Killing: "+ screenNameField.Get(dbObject).ToString());
						client.Delete(dbObject);
						empty = false;
					}
				}
				client.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			if (empty)
			{
				Console.WriteLine("The list of players is already empty.");
			}
		}

		private void PrintAll()
		{
			List<Player> players = GetAllPlayers();
			foreach(Player p in players)
			{
				Console.WriteLine(p.ScreenName);
			}
			if(players == null || players.Count == 0)
			{
				Console.WriteLine("The list of players is empty.");
			}

			//bool empty = true;
			//try
			//{
			//	IQuery query = client.Query();
			//	IEnumerable allObjects = query.Execute();

			//	foreach (Object item in allObjects)
			//	{
			//		GenericObject dbObject = (GenericObject)item; // Note: If db4o finds actuall class, it will be the right class, otherwise GenericObject. You may need to do some checks and casts
			//		dbObject.GetGenericClass().GetDeclaredFields(); // Find out fields
			//		//Console.WriteLine("Full name of type: " + dbObject.GetGenericClass().GetName());
			//		//if (dbObject.GetGenericClass().GetName().ToLower().Contains("player"))
			//		//{
			//		//	foreach (IReflectField a in dbObject.GetGenericClass().GetDeclaredFields())
			//		//	{
			//		//		if (a.GetName().ToString().ToLower().Contains("screenname"))
			//		//		{
			//		//			Console.WriteLine("name: " + a.Get(item).ToString());
			//		//			empty = false;
			//		//		}
			//		//	}
			//		//}
			//		//object fieldData = dbObject.Get(0); // Get the field at index 0. The GetDeclaredFields() tells you which field is at which index
			//		//Console.WriteLine(fieldData.ToString());
			//	}
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine(e.Message);
			//}
			//if (empty)
			//{
			//	Console.WriteLine("The list is empty.");
			//}
		}

		public List<Player> GetAllPlayers()
		{
			List<Player> players = new List<Player>();
			try
			{
				IQuery query = client.Query();
				IEnumerable allObjects = query.Execute();

				foreach (Object item in allObjects)
				{
					GenericObject dbObject = (GenericObject)item;
					if (dbObject.GetGenericClass().GetName().ToLower().Contains("player"))
					{
						IReflectField screenNameField = dbObject.GetGenericClass().GetDeclaredField("<ScreenName>k__BackingField");
						players.Add(new Player(screenNameField.Get(dbObject).ToString()));
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			return players;
		}

		public bool VerifyPlayer(string screenName)
		{
			foreach(Player p in GetAllPlayers())
			{
				if (p.ScreenName.Equals(screenName))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// opens the IObjectServer, and waits forever until Close() is called
		/// or a StopServer message is being received.
		/// </summary>
		public void RunServer()
		{
			Console.WriteLine("Server starting");
			lock(this)
			{
				// Using the messaging functionality to redirect all
				// messages to this.processMessage
				IConfiguration configuration = Db4oFactory.NewConfiguration();
				configuration.ClientServer().SetMessageRecipient(this);


				configuration.Add(new TransparentActivationSupport());
				configuration.Add(new TransparentPersistenceSupport());

				
				server = Db4oFactory.OpenServer(configuration, FileName, Port);
				server.GrantAccess(User, Password);
				server.GrantAccess("server", "serverpass");

				Console.WriteLine("Server started");
				client = Db4oFactory.OpenClient(Host, Port, "server", "serverpass");
				try
				{
					if (!stop){
						// wait forever until Close will change stop variable
						Console.WriteLine("Server waiting...");
						Monitor.Wait(this);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
				server.Close();
				Console.WriteLine("Server closed");
			}
		}
	// end RunServer
	
		/// <summary>
		/// messaging callback
		/// see com.db4o.messaging.MessageRecipient#ProcessMessage()
		/// </summary>
		public void ProcessMessage(IMessageContext context, object message)
		{
			lock (this)
			{
				if (message is string)
				{
					string[] splitMessage = message.ToString().Split(' ');
					switch (splitMessage[0])
					{
						case "STOP":
							Console.WriteLine("Server is closing...");
							stop = true;
							Monitor.PulseAll(this);
							Console.WriteLine("3");
							Thread.Sleep(1000);
							Console.WriteLine("2");
							Thread.Sleep(1000);
							Console.WriteLine("1");
							Thread.Sleep(1000);
							Close();
							break;
						case "MSG":
							Console.WriteLine(splitMessage[1]);
							break;
						default:
							Console.WriteLine("Message was string: " + message.ToString());
							break;
					}
				}
				else
				{
					Console.WriteLine("Message received: " + message.ToString());
				}
			}
		}
		
		// end ProcessMessage

		/// <summary>
		/// closes this server.
		/// </summary>
		public void Close()
	    {
	        lock(this)
	        {
	                stop = true;
	                Monitor.PulseAll(this);
	            }
	    }
	// end Close
	}
}
