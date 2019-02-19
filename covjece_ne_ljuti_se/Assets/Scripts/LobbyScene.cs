using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class LobbyScene : MonoBehaviour {
	private InputField usernameInput;
	private string mUsername;

	// Use this for initialization
	void Start () {
		usernameInput = GameObject.Find("YourUsernameInput").GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ConnectToServer()
	{
		Random.InitState(22);
		mUsername = usernameInput.text.ToString();
		Client.ConnectToServer(mUsername);
		UnityEngine.Debug.Log("Connected to server");
		//Client.AddPlayer(Random.Range(0,1000), username);
		//Client.DisconnectFromServer();
		//Debug.Log("Disconnected from server");
	}

	public void StartConsoleServer()
	{
		var stringPath = Application.dataPath + "/consoleServer/consoleServer.exe";
		UnityEngine.Debug.Log(stringPath);
		//var myProcess = new Process();
		Process.Start(stringPath);
		/*myProcess.StartInfo.FileName = "";
		myProcess.StartInfo.Arguments = stringPath;
		myProcess.Start();*/
	}

	public void CloseServer()
	{
		//Client.AddPlayer(Random.Range(0, 1000), mUsername);
		Client.CloseServer();
		UnityEngine.Debug.Log("Server closed");
	}

	public void TestConnection()
	{
		Client.SendMessageToServer("MSG;Listing all players...");
		Client.PrintAllPlayers();
	}
}
