using System;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour {

	public GameObject go;
	//public PlayerColor owner;
	public Position currentPos;
	[SerializeField]
	public PawnData data;

	public void SetColor()
	{
		//if (owner == null)
		//{
		//	Debug.LogWarning("Pawn SetColor() - owner or owner playercolor == null");
		//	return;
		//}
		go.GetComponent<Image>().color = GameScene.instance.playerColors.Find(x => x.id == data.ownerId).color;
	}

	//position - the point to which the move the pawn
	public bool Move(Position position)
	{
		if (position == null)
		{
			Debug.LogWarning("Pawn Move() - position == null");
			return false;
		}
		if(position.occupied != null)
		{
			print("Point is occupied");
			return false;
		}
		//TODO: animation
		currentPos.occupied = null;
		go.transform.position = position.transform.position;
		currentPos = position;
		position.occupied = this;
		data.currentPosId = currentPos.id;
		return true;
	}

	public override string ToString()
	{
		return this.data.ownerId + " " + this.data.id + " " + currentPos;
	}
	
}
