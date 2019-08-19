using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Die : MonoBehaviour {

	public GameObject go;
	private Image image;
	public int current;
	public Sprite currentSprite;
	public List<Sprite> allSprites;

	// Use this for initialization
	void Start () {
		current = 6;
		UnityEngine.Random.Range(1,7);
		image = go.GetComponent<Image>();
	}

	public int Roll()
	{
		current = UnityEngine.Random.Range(1, 7);
		ChangeImage(current);
		return current;
	}

	private void ChangeImage(int current)
	{
		image.sprite = allSprites[current-1];
	}
}
