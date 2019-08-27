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
	private PlayerColor currentPlayer;
	//all the pieces on the board
	public List<Pawn> Pawns;
	public GameObject gameOverGO;
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
		else if (Client.currentPlayer.ScreenName == playerName)
		{
			gameConfig.NumOfPlayers = players.Count;
			gameConfig.IsRunning = true;
			gameConfig.CurrentTurn = 1;
			Database.UpdateGameConfig(gameConfig);
		}

		//TEST
		GameConfiguration gcc = Database.FetchGameConfig();
		print("num of players: " + gcc.NumOfPlayers + ", current turn: " + gcc.CurrentTurn);

		currentPlayer = playerColors.Find(x => x.id == Client.currentPlayer.Order);

		if (Client.currentPlayer.ScreenName == playerName)
		{
			foreach (PlayerColor pc in playerColors)
			{
				pc.SetPawnsForStart();
			}
			UpdateAllPawnData();
		}
		ConnectWaypoints();
		UpdateOnlinePlayersList();
		InvokeRepeating("UpdateOnlinePlayersList", 0.2f, 1.5f);
		InvokeRepeating("UpdateLocalGameConfig", 0.2f, 1.4f);
		InvokeRepeating("UpdateLocalPawnPosition", 0.2f, 2f);
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
		if(gameConfig.CurrentTurn < 10)
		{
			die.RollSix();
		}
		else
		{
			die.Roll();
		}
		die.button.SetActive(false);
		MakeMoveAvailable();
	}

	//Show the player what moves are possible
	private void MakeMoveAvailable()
	{
		//TODO: visual reference - show possible moves
		print("MakeMoveAvailable");
		die.rolledThisTurn = true;
		//TODO: if no possible turns go to the next turn
		bool possibleMove = false;
		foreach(Pawn p in currentPlayer.pawns)
		{
			Position tempPosition;
			tempPosition = p.CalculateMove(p.currentPos, die.current);
			if (p.MovePossible(tempPosition))
			{
				possibleMove = true;
			}
		}
		if (!possibleMove)
		{
			NextTurn();
		}
	}

	public void NextTurn(bool gameOver = false)
	{
		die.rolledThisTurn = false;
		gameConfig.CurrentTurn++;
		MakeUnplayable();
		if (gameOver)
		{
			gameConfig.IsRunning = false;
		}
		Database.UpdateGameConfig(gameConfig);
		//UpdateLocalGameConfig();
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
		UpdateLocalPawnPosition();
		print("GameScene UpdateGameConfig - "+Client.currentPlayer.Order +" "+gc.CurrentTurn);
		if( gc.IsRunning &&
			gameConfig.NumOfPlayers > 1 &&
			gameConfig.CurrentTurn % gameConfig.NumOfPlayers
			== Client.currentPlayer.Order % gameConfig.NumOfPlayers)
		{
			MakePlayable();
		}
		else
		{
			MakeUnplayable();
		}
		if (!gc.IsRunning)
		{
			gameOverGO.SetActive(true);
		}
		else
		{
			gameOverGO.SetActive(false);
		}
	}

	private void UpdateLocalPawnPosition()
	{
		List<PawnData> pawnDatas = Database.FetchAllPawnDatas();
		if(pawnDatas == null)
		{
			UnityEngine.Debug.LogWarning("UpdatePawnPosition - pawnDatas in db == null");
			return;
		}
		MapPawnData(pawnDatas);
	}

	private void MapPawnData(List<PawnData> pawnDatasDB)
	{
		foreach(PawnData pd in pawnDatasDB)
		{
			Pawn pawn = Pawns.Find(x => x.data.id == pd.id && x.data.ownerId == pd.ownerId);
			if (pawn.data.currentPosId == pd.currentPosId && pawn.currentPos.id == pd.currentPosId)
				continue;
			pawn.data.currentPosId = pd.currentPosId;
			pawn.Move(FindPosition(pawn));
		}
	}

	private Position FindPosition(Pawn pawn)
	{
		//home positions are from -4 to -1
		if (pawn.data.currentPosId < 0 && pawn.data.currentPosId > -5)
		{
			HomePoint hp = currentPlayer.firstHomePoint;
			while (hp != null)
			{
				if (hp.id == pawn.data.currentPosId)
				{
					return hp;
				}
				hp = (HomePoint)hp.next;
			}
			UnityEngine.Debug.LogError("GameScene findPosition - Couldn't find home position from id");
			return currentPlayer.firstHomePoint;
		}
		//finish positions are from -8 to -5
		else if (pawn.data.currentPosId < -4)
		{
			return pawn.owner.finishPoints.Find(x=>x.id == pawn.data.currentPosId);
		}
		return allWaypoints.Find(x => x.id == pawn.data.currentPosId);
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
