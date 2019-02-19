using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Db4objects.Db4o;

public class Board : MonoBehaviour {
	
	//if there is a lobby with multiple games
	//int boardId;
	//number of the current turn
	public int TurnNumber { get; set; }

	public List<Player> Players { get; set; }
	//all the pieces on the board
	public List<Piece> Pieces { get; set; }

	//string filePath = "D:\\Unity Projects\\Covjece ne ljuti se\\covjece_ne_ljuti_se\\Assets\\db\\tempdb.yap";

	// Use this for initialization
	void Start () {
		/*Players = new List<Player>();
		Pieces = new List<Piece>();
		Player player1 = new Player();
		player1.ScreenName = "player1";
		//Database db = new Database();
		
		IObjectContainer db = Db4oFactory.OpenFile(filePath);
		try
		{
			db.Store(player1);
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.Log("can't store player1 " + e.Message);
		}

		Player player2 = new Player();
		player2.ScreenName = "player2";

		try
		{
			db.Store(player2);
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.Log("can't store player2 " + ex.Message);
		}
		UnityEngine.Debug.Log("i'm actually alive");
		Player proto = new Player();
		proto.ScreenName = null;
		proto.OwnerId = 0;
		IObjectSet result = db.QueryByExample(proto);
		foreach (object item in result)
		{
			UnityEngine.Debug.Log("Deleting: "+item.ToString());
			db.Delete(item);
		}
		db.Commit();
		db.Close();
		*/
	}

	// Update is called once per frame
	void Update () {
		
	}
}
