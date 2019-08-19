using System.Collections.Generic;
using UnityEngine;

public class PlayerColor : MonoBehaviour{

	public Player player;
	public Color color;
	public HomePoint firstPoint;
	public List<Pawn> pawns;


	public void SetPawnsForStart()
	{
		if (firstPoint == null)
		{
			Debug.LogWarning("PlayerColor SetPawnsForStart() - firstPoint == null");
			return;
		}
		HomePoint home = firstPoint;
		foreach (Pawn p in pawns)
		{
			p.SetColor();
			p.Move(home);
			if (home.next != null)
			{
				home = (HomePoint)home.next;
			}
		}
	}

}
