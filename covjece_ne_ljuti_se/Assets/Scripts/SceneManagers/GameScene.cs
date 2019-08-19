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
	public Die die;

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
		ConnectWaypoints();
		UpdateOnlinePlayersList();
		InvokeRepeating("UpdateOnlinePlayersList", 0.2f, 1.5f);
	}

	public void RollTheDie()
	{
		//TODO: set roll and do the rest
		int currRoll = die.Roll();
	}

	private void ConnectWaypoints()
	{
		int children = WaypointsGO.transform.childCount;
		Waypoint prev,curr,next;
		prev = WaypointsGO.transform.GetChild(children - 1).GetComponent<Waypoint>();
		curr = WaypointsGO.transform.GetChild(0).GetComponent<Waypoint>();
		next = WaypointsGO.transform.GetChild(1).GetComponent<Waypoint>();
		for (int i = 2; i <= children; i++)
		{
			curr.prev = prev;
			curr.next = next;

			prev = curr;
			curr = next;
			if(i <children)
				next = WaypointsGO.transform.GetChild(i).GetComponent<Waypoint>();
			else
			{
				next = WaypointsGO.transform.GetChild(0).GetComponent<Waypoint>();
			}
			//print("For loop " + i + ":  "+ WaypointsGO.transform.GetChild(i).name);
			//print("prev " + prev.transform.name);
			//print("curr " + curr.transform.name);
			//print("next " + next.transform.name);
		}
		curr.prev = prev;
		curr.next = next;
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
