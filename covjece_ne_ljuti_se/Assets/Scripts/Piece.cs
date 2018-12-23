using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

	//the only place where the position of the piece on the board is saved
	public int position { get; set; } //negative if the piece is off the board
	public int ownerId { get; set; } //TODO: onClick check if owner == current player

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
