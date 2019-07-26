using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubScene : MonoBehaviour {

	public GameObject PlayerListContent;
	public GameObject PlayerListNamePrefab;

	void Start () {
		if(Client.mConnection == null)
			Client.ConnectToServer("Hub");
		List<Player> players = Database.FetchAllPlayers();
		foreach(Player player in players)
		{
			Debug.Log("Hub: "+player.ScreenName);
		}
		UpdateOnlinePlayersList();
	}

	private void UpdateOnlinePlayersList()
	{
		List<Player> players = Database.FetchAllPlayers();
		foreach(Player player in players)
		{
			GameObject listItem = Instantiate(PlayerListNamePrefab, PlayerListContent.transform);
			listItem.transform.GetChild(0).GetComponent<Text>().text = player.ScreenName;

		}
	}

	private void StartTheGame()
	{
		SceneManager.LoadScene("Hub");
	}

	private void PlayerIsReady()
	{
		Client.RegisterThePlayerAsReady();

		//TODO: disable option to configure the player color and options


		//check if all players are ready if yes start game else wait
		//if (CheckIfPlayersReady())
		//{
		//	StartTheGame();
		//}
		//else
		//{
		//	Debug.Log("Not all players are ready to play, waiting for players.");
		//}
	}

	public void DisonnectFromServer()
	{
		Client.RemoveCurrentPlayer();
		UnityEngine.Debug.Log("Connected to server");
		Debug.Log("Disconnected player "+Client.currentPlayer.ScreenName+" from server");
		SceneManager.LoadScene("Lobby");
	}



}
