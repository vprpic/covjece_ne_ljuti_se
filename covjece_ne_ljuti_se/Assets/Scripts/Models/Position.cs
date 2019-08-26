using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour {

	public GameObject go;
	public Position prev;
	public Position next;
	public Pawn occupied;
	public Player player; //if null no player owns this
	public int id;

}
