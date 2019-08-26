using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

	public GameObject go;
	public Position prev;
	public Position next;
	public Pawn occupied;
	[SerializeField]
	public PlayerColor playerColor; //if null no playerColor owns this
	public int id;

}
