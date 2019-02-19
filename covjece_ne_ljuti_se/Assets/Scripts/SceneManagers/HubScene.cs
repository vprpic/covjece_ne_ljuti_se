using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	
}
