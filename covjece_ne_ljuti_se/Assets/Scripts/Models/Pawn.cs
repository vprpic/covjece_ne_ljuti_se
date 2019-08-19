using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour {

	public GameObject go;
	public PlayerColor owner;
	public Position currentPos;

	private void Start()
	{
		
	}

	public void SetColor()
	{
		if (owner == null)
		{
			Debug.LogWarning("Pawn SetColor() - owner or owner playercolor == null");
			return;
		}
		go.GetComponent<Image>().color = owner.color;
	}

	//position - the point to which the move the pawn
	public void Move(Position position)
	{
		if (position == null)
		{
			Debug.LogWarning("Pawn Move() - position == null");
			return;
		}
		//TODO: animation
		go.transform.position = position.transform.position;
		currentPos = position;
		position.occupied = this;
	}
}
