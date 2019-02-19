using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	
	public string ScreenName { get; set; }

	public Player()
	{

	}

	public Player(string screenName)
	{
		ScreenName = screenName;
	}

	public override string ToString()
	{
		return this.ScreenName;
	}
}
