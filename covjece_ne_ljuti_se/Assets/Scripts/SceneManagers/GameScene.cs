using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Db4objects.Db4o;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameScene : MonoBehaviour {

	private string playerName = "mon";

	public static GameScene instance;
	public List<Player> Players { get; set; }
	public List<PlayerColor> playerColors;
	public PlayerColor currentPlayer;
	//all the pieces on the board
	public List<Pawn> Pawns;
	public GameObject PlayerListContent;
	public GameObject PlayerListNamePrefab;
	private List<Player> currentListedPlayers;
	private List<GameObject> playerGOs;
	public Text ReadyPlayButtonText;
	public GameObject WaypointsGO;
	public Die die;
	public static bool playable;
	public List<Waypoint> allWaypoints;

	public GameConfiguration gameConfig;

	void Start()
	{
		instance = this;
		playable = false;
		die.button.SetActive(false);
		playerGOs = new List<GameObject>();
		currentListedPlayers = new List<Player>();
		if (Client.mConnection == null)
			Client.ConnectToServer("GameScene");
		List<Player> players = Database.FetchAllPlayers();
		gameConfig = Database.FetchGameConfig();
		if (gameConfig == null)
		{
			UnityEngine.Debug.LogError("GameScene Start() - gameConfig == null");
		}
		else if (Client.currentPlayer.ScreenName == playerName)//Client.currentPlayer.Order == 0)
		{
			gameConfig.NumOfPlayers = players.Count;
			gameConfig.IsRunning = true;
			gameConfig.CurrentTurn = 1;
			Database.UpdateGameConfig(gameConfig);

			//int playerColorPos = 0;
			//foreach (Player player in players)
			//{
			//	UnityEngine.Debug.Log("GameScene: " + player.ToString());
			//	playerColors[playerColorPos].player = player;
			//	player.playerColor = playerColors[playerColorPos].id;
			//	//player.SetColor(playerColors[playerColorPos]);
			//	playerColorPos++;
			//	//Database.AddPlayer(player);
			//	Database.Commit();
			//}
		}

		//TEST
		GameConfiguration gcc = Database.FetchGameConfig();
		print("num of players: "+gcc.NumOfPlayers + ", current turn: " +gcc.CurrentTurn);

		
		
		if (Client.currentPlayer.ScreenName == playerName)
		{
			foreach (PlayerColor pc in playerColors)
			{
				if(pc.id == Client.currentPlayer.Order)
				{
					currentPlayer = pc;
				}
				pc.SetPawnsForStart();
			}
			UpdateAllPawnData();
		}
		ConnectWaypoints();
		UpdateOnlinePlayersList();
		InvokeRepeating("UpdateOnlinePlayersList", 0.2f, 1.5f);
		InvokeRepeating("UpdateGameConfig", 0.2f, 1.4f);
		InvokeRepeating("UpdatePawnPosition", 0.2f, 2f);
	}

	private void UpdateAllPawnData()
	{
		foreach(Pawn p in Pawns)
		{
			Database.UpdatePawnData(p.data);
		}
	}

	public void MakePlayable()
	{
		if (playable)
			return;
		playable = true;
		die.button.SetActive(true);
		//TODO:
	}

	public void MakeUnplayable()
	{
		if (!playable)
			return;

		playable = false;
		die.button.SetActive(false);
		//TODO: 
	}

	public void RollTheDie()
	{
		//TODO: set roll and do the rest
		int currRoll = die.Roll();
		die.button.SetActive(false);
		MakeMoveAvailable();
	}

	//Show the player what moves are possible
	private void MakeMoveAvailable()
	{
		//TODO: visual reference - show possible moves
		//foreach(Pawn p in currentPlayer.pawns)
		//{

		//}
		print("MakeMoveAvailable");
		currentPlayer.pawns[0].Move(currentPlayer.firstPoint);
		Database.UpdatePawnData(currentPlayer.pawns[0].data);
		if (die.current == 6 || die.current == 5 || die.current == 4)
		{
			currentPlayer.pawns[1].Move(currentPlayer.firstPoint.next);
			Database.UpdatePawnData(currentPlayer.pawns[1].data);
		}
	}

	public void NextTurn()
	{
		gameConfig.CurrentTurn++;
		MakeUnplayable();
		Database.UpdateGameConfig(gameConfig);
	}

	private void ConnectWaypoints()
	{
		int children = WaypointsGO.transform.childCount;
		Waypoint prev,curr,next;
		prev = WaypointsGO.transform.GetChild(children - 1).GetComponent<Waypoint>();
		curr = WaypointsGO.transform.GetChild(0).GetComponent<Waypoint>();
		next = WaypointsGO.transform.GetChild(1).GetComponent<Waypoint>();
		curr.id = 0;
		for (int i = 2; i <= children; i++)
		{
			curr.prev = prev;
			curr.next = next;

			prev = curr;
			curr = next;
			curr.id = i-1;
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

	private void UpdateLocalGameConfig()
	{
		GameConfiguration gc = Database.FetchGameConfig();
		gameConfig = gc;
		UpdatePawnPosition();
		print("GameScene UpdateGameConfig - "+Client.currentPlayer.Order +" "+gc.CurrentTurn);
		if(gameConfig.CurrentTurn % gameConfig.NumOfPlayers
			== Client.currentPlayer.Order % gameConfig.NumOfPlayers)
		{
			MakePlayable();
		}
		else
		{
			MakeUnplayable();
		}
	}

	private void UpdatePawnPosition()
	{
		List<PawnData> pawnDatas = Database.FetchAllPawnDatas();
		if(pawnDatas == null)
		{
			UnityEngine.Debug.LogWarning("UpdatePawnPosition - pawnDatas in db == null");
			return;
		}
		MapPawnData(pawnDatas);
	}

	private void MapPawnData(List<PawnData> pawnDatas)
	{
		foreach(PawnData pd in pawnDatas)
		{
			Pawn pawn = Pawns.Find(x => x.data.id == pd.id && x.data.ownerId == pd.ownerId);
			if (pawn.data.currentPosId == pd.currentPosId)
				return;
			pawn.data.currentPosId = pd.currentPosId;
			pawn.Move(FindPosition(pd.currentPosId));
		}
	}

	private Position FindPosition(int currentPosId)
	{
		//home positions are < 0
		if(currentPosId < 0)
		{
			HomePoint hp = currentPlayer.firstHomePoint;
			while (hp != null) {
				if(hp.id == currentPosId)
				{
					return hp;
				}
				hp = (HomePoint) hp.next;
			}
			UnityEngine.Debug.LogError("GameScene findPosition - Couldn't find home position from id");
			return currentPlayer.firstHomePoint;
		}
		return allWaypoints.Find(x => x.id == currentPosId);
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
		//if (Client.currentPlayer.ScreenName == "mon")//Client.currentPlayer.Order == 0)
		//{
		//	int playerColorPos = 0;
		//	foreach (Player player in players)
		//	{
		//		UnityEngine.Debug.Log("GameScene: " + player.ToString());
		//		playerColors[playerColorPos].player = player;
		//		player.playerColor = playerColors[playerColorPos].id;
		//		//player.SetColor(playerColors[playerColorPos]);
		//		playerColorPos++;
		//		Database.AddPlayer(player);
		//		Database.Commit();
		//	}
		//}
		foreach (Player player in players)
		{
			currentListedPlayers.Add(player);
			GameObject listItem = Instantiate(PlayerListNamePrefab, PlayerListContent.transform);
			playerGOs.Add(listItem);
			listItem.transform.GetChild(0).GetComponent<Text>().text = player.ScreenName;
			Image image = listItem.transform.GetChild(1).GetComponent<Image>();
			image.color = playerColors.Find(x => x.id == player.Order).color;

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
