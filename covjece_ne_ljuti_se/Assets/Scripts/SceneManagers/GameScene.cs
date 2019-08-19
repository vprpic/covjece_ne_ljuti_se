using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Db4objects.Db4o;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameScene : MonoBehaviour {

	public List<Player> Players { get; set; }
	public List<PlayerColor> playerColors;
	//all the pieces on the board
	public List<Pawn> Pawns{ get; set; }
	public GameObject PlayerListContent;
	public GameObject PlayerListNamePrefab;
	private List<Player> currentListedPlayers;
	private List<GameObject> playerGOs;
	public Text ReadyPlayButtonText;
	public GameObject WaypointsGO;

	void Start()
	{
		playerGOs = new List<GameObject>();
		currentListedPlayers = new List<Player>();
		if (Client.mConnection == null)
			Client.ConnectToServer("Hub");
		List<Player> players = Database.FetchAllPlayers();
		int playerColorPos = 0;
		foreach (Player player in players)
		{
			UnityEngine.Debug.Log("Hub: " + player.ScreenName);
			playerColors[playerColorPos].player = player;
			player.playerColor = playerColors[playerColorPos];
			playerColorPos++;
		}
		foreach(PlayerColor pc in playerColors)
		{
			pc.SetPawnsForStart();
		}
		UpdateOnlinePlayersList();
		InvokeRepeating("UpdateOnlinePlayersList", 0.2f, 1.5f);
	}

	private void UpdateOnlinePlayersList()
	{
		List<Player> players = Database.FetchAllPlayers();
		currentListedPlayers.Clear();
		for (int i = 0; i < playerGOs.Count; i++)
		{
			//TODO: recycle GOs
			Destroy(playerGOs[i]);
		}
		foreach (Player player in players)
		{
			currentListedPlayers.Add(player);
			GameObject listItem = Instantiate(PlayerListNamePrefab, PlayerListContent.transform);
			playerGOs.Add(listItem);
			listItem.transform.GetChild(0).GetComponent<Text>().text = player.ScreenName;
		}
	}

	private bool PlayersReady()
	{
		UpdateOnlinePlayersList();
		if (currentListedPlayers.Count < 2)
		{
			return false;
		}
		foreach (Player p in currentListedPlayers)
		{
			if (!p.Ready)
				return false;
		}
		return true;
	}

	public void DisonnectFromServer()
	{
		UnityEngine.Debug.Log("Disconnected player " + Client.currentPlayer.ScreenName + " from server");
		Client.RemoveCurrentPlayer();
		UnityEngine.Debug.Log("Disconnected from server");
		SceneManager.LoadScene("Lobby");
	}

}
