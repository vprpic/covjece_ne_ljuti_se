using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubScene : MonoBehaviour {
	
	void Start () {
		Client.ConnectToServer("Hub");
		List<Player> players = Database.FetchAllPlayers();
		foreach(Player player in players)
		{
			Debug.Log("Hub: "+player.ScreenName);
		}
	}
	
}
