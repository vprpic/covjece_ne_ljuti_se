using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace consoleServer
{
	class Program : ServerConfiguration, IMessageRecipient
	{
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
			new Program().RunServer();
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
			
				IObjectServer db4oServer = Db4oFactory.OpenServer(configuration, FileName, Port);
				db4oServer.GrantAccess(User, Password);

				Console.WriteLine("Server started");
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
				db4oServer.Close();
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
					string[] splitMessage = message.ToString().Split(';');
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
