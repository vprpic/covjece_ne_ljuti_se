﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

	//in range 0-3 -> max number of players is 4
	public int OwnerId { get; set; }
	public string ScreenName { get; set; }

	public override string ToString()
	{
		return this.ScreenName;
	}
}
