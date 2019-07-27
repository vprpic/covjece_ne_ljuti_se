using Db4objects.Db4o;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiguration {

	public bool IsRunning { get; set; }
	public int NumOfPlayers { get; set; }
	public int CurrentTurn { get; set; }

	public GameConfiguration()
	{
	}
}
