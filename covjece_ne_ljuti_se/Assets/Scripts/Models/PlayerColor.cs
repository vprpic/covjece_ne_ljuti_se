using Db4objects.Db4o.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerColor : MonoBehaviour{
	
	public Color color;
	public HomePoint firstHomePoint;
	public Waypoint firstPoint;
	public int id;
	public List<Pawn> pawns;
	public List<FinishPoint> finishPoints;

	public void SetPawnsForStart()
	{
		if (firstHomePoint == null)
		{
			Debug.LogWarning("PlayerColor SetPawnsForStart() - firstPoint == null");
			return;
		}
		HomePoint home = firstHomePoint;
		foreach (Pawn p in pawns)
		{
			p.SetColor();
			p.Move(home);
			if (home.next != null)
			{
				home = (HomePoint)home.next;
			}
			Database.UpdatePawnData(p.data);
		}
	}

}
