using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Die : MonoBehaviour {

	public GameObject go;
	public GameObject button;
	private Image image;
	public int current;
	public Sprite currentSprite;
	public List<Sprite> allSprites;
	public bool rolledThisTurn;

	// Use this for initialization
	void Start () {
		current = 6;
		rolledThisTurn = false;
		UnityEngine.Random.Range(1,7);
		image = go.GetComponent<Image>();
	}

	public int Roll()
	{
		current = UnityEngine.Random.Range(1, 7);
		ChangeImage(current);
		rolledThisTurn = true;
		return current;
	}

	private void ChangeImage(int current)
	{
		image.sprite = allSprites[current-1];
	}

	public void ResetRoll()
	{
		rolledThisTurn = false;
	}

	internal void RollSix()
	{
		current = 6;
		ChangeImage(current);
		rolledThisTurn = true;
	}
}
