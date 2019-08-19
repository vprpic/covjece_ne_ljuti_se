using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : Position {

	public bool playerStartingPoint; //if false no player starts there, else means that player starts there
	public FinishPoint finishPoint; //if null it doesn't lead to a player's finish point

}
 