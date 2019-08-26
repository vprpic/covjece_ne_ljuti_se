using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PawnData  {

	public int id;
	public int ownerId;
	public int currentPosId;

	public PawnData(int _id)
	{
		id = _id;
	}

	public PawnData(int _id, int _ownerId)
	{
		id = _id;
		ownerId = _ownerId;
	}

	public PawnData()
	{

	}
}
