using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubScene : MonoBehaviour {

	public GameObject PlayerListContent;
	public GameObject PlayerListNamePrefab;
	private List<Player> currentListedPlayers;
	private List<GameObject> playerGOs;
	public Text ReadyPlayButtonText;

	void Start () {
		playerGOs = new List<GameObject>();
		currentListedPlayers = new List<Player>();
		if (Client.mConnection == null)
			Client.ConnectToServer("Hub");
		List<Player> players = Database.FetchAllPlayers();
		foreach (Player player in players)
		{
			Debug.Log("Hub: " + player.ScreenName);
		}
		UpdateOnlinePlayersList();
		InvokeRepeating("UpdateOnlinePlayersList", 0.2f, 1.5f);
	}

	private void UpdateOnlinePlayersList()
	{
		List<Player> players = Database.FetchAllPlayers();
		currentListedPlayers.Clear();
		for (int i=0;i<playerGOs.Count;i++){
			//TODO: recycle GOs
			Destroy(playerGOs[i]);
		}
		foreach(Player player in players)
		{
			currentListedPlayers.Add(player);
			GameObject listItem = Instantiate(PlayerListNamePrefab, PlayerListContent.transform);
			playerGOs.Add(listItem);
			listItem.transform.GetChild(0).GetComponent<Text>().text = player.ScreenName;
		}
	}

	private void StartTheGame()
	{
		if (!PlayersReady())
		{
			ReadyPlayButtonText.text = "Waiting for players..";
			Debug.Log("Not all players are ready to play, waiting for players.");
			return;
		}
		SceneManager.LoadScene("Game");
	}

	public void PlayerIsReady()
	{
		Client.RegisterThePlayerAsReady();

		//TODO: disable option to configure the player color and options

		//check if all players are ready if yes start game else wait
		StartTheGame();
	}

	private bool PlayersReady()
	{
		UpdateOnlinePlayersList();
		if(currentListedPlayers.Count < 2)
		{
			return false;
		}
		foreach(Player p in currentListedPlayers)
		{
			if (!p.Ready)
				return false;
		}
		return true;
	}

	public void DisonnectFromServer()
	{
		Debug.Log("Disconnected player "+Client.currentPlayer.ScreenName+" from server");
		Client.RemoveCurrentPlayer();
		UnityEngine.Debug.Log("Disconnected from server");
		SceneManager.LoadScene("Lobby");
	}
	

}
