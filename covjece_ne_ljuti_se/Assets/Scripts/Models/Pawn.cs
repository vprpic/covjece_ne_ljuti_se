using System;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour {

	public GameObject go;
	public PlayerColor owner;
	public Position currentPos;
	[SerializeField]
	public PawnData data;

	public void SetColor()
	{
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
			if (position.occupied.owner.id == this.owner.id)
			{
				print("Point is occupied by same color");
				return false;
			}
			//position is occupied by other color
			SendPawnHome(position.occupied);
		}
		//TODO: animation
		currentPos.occupied = null;
		go.transform.position = position.transform.position;
		currentPos = position;
		position.occupied = this;
		data.currentPosId = currentPos.id;
		return true;
	}

	private void SendPawnHome(Pawn pawn)
	{
		HomePoint temp = pawn.owner.firstHomePoint;
		//find empty home spot
		while(temp.occupied != null || temp.next != null)
		{
			temp = (HomePoint)temp.next;
		}
		if(temp == null)
		{
			UnityEngine.Debug.LogError("SendPawnHome - HomePoint is null!");
			return;
		}
		//set current spot to free
		pawn.currentPos.occupied = null;
		////set home spot to occupied
		//temp.occupied = this;
		//move pawn to position
		if (!Move(temp))
		{
			UnityEngine.Debug.LogError("SendPawnHome - Move home failed!");
		}
		Database.UpdatePawnData(pawn.data);
	}

	public override string ToString()
	{
		return this.data.ownerId + " " + this.data.id + " " + currentPos;
	}

	public void OnPawnPressed()
	{
		//if it's not the current player's turn
		if(GameScene.instance.gameConfig.CurrentTurn % GameScene.instance.gameConfig.NumOfPlayers 
			!= Client.currentPlayer.Order % GameScene.instance.gameConfig.NumOfPlayers)
		{
			return;
		}
		//if the die hasn't been rolled this turn || if the player isn't the owner of the pawn
		if (!GameScene.instance.die.rolledThisTurn || Client.currentPlayer.Order != owner.id)
		{
			return;
		}
		Position position = CalculateMove(currentPos, GameScene.instance.die.current);

		if (!Move(position))
			return;

		Database.UpdatePawnData(this.data);

		GameScene.instance.NextTurn();
	}

	//returns null if move impossible
	public Position CalculateMove(Position currentPos, int dieRoll)
	{
		//if the pawn is at a homepoint and the player rolled a 6 it can move to the first point
		//here we do not care if the position is occupied or not
		//(tempPos.next.occupied != null && tempPos.next.occupied.owner.id == this.owner.id)
		if (currentPos is HomePoint )
		{
			if ( dieRoll == 6)
			{
				return owner.firstPoint;
			}
			else
			{
				return null;
			}
		}
		else
		{
			Position tempPos = currentPos;
			// move by 'dieRoll' amount of spaces, keep checking if you need to turn to the finishPoint
			// and if the position is occupied by your own team
			for (int i = 0; i < dieRoll; i++)
			{
				//if the next one is occupied by the same color return null
				if( tempPos == null || tempPos.next == null
					|| (tempPos.next.occupied != null && tempPos.next.occupied.owner.id == this.owner.id))
				{
					//if it's the waypoint that leads to the finishpoint for this playerColor then move to it
					if (tempPos is Waypoint && tempPos.playerColor != null && tempPos.playerColor.id == this.owner.id && ((Waypoint)tempPos).finishPoint != null)
					{
						tempPos = ((Waypoint)tempPos).finishPoint;
					}
					else
					{
						return null;
					}
				}
				//if it's the waypoint that leads to the finishpoint for this playerColor then move to it
				if(tempPos is Waypoint && tempPos.playerColor != null && tempPos.playerColor.id == this.owner.id && ((Waypoint)tempPos).finishPoint != null)
				{
					tempPos = ((Waypoint)tempPos).finishPoint;
				}
				else
				{
					tempPos = tempPos.next;
				}
			}
			return tempPos;
		}
	}

	public bool MovePossible(Position moveToPoint)
	{
		if(moveToPoint == null ||
			//the position is occupied by the same color
			moveToPoint.occupied != null && moveToPoint.occupied.owner.id == this.owner.id)
		{
			return false;
		}
		return true;
	}
}
