using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System;

public class LobbyScene : MonoBehaviour {
	private InputField usernameInput;
	private string mUsername;
	public GameObject error;
	public Text errorText;

	// Use this for initialization
	void Start () {
		usernameInput = GameObject.Find("YourUsernameInput").GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ConnectToServer()
	{
		//UnityEngine.Random.InitState(22);
		mUsername = usernameInput.text.ToString();
		Client.ConnectToServer(mUsername);
		UnityEngine.Debug.Log("Connected to server");
		//Client.AddPlayer(Random.Range(0,1000), username);
		//Client.DisconnectFromServer();
		//Debug.Log("Disconnected from server");
		SceneManager.LoadScene("Hub");
	}

	public void StartConsoleServer()
	{
		SetErrorText("Started console server.");
		//var stringPath = Application.dataPath + @"/consoleServer/consoleServer.exe";
		var stringPath = Environment.CurrentDirectory + @"\consoleServer\consoleServer.exe";
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
	
	private void SetErrorText(string errorString, bool visible = true)
	{
		error.SetActive(visible);
		errorText.text = errorString;
	}
}
