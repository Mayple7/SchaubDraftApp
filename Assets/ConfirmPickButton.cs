﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConfirmPickButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(6.5f, -3.25f, 0);
	private Vector3 HiddenPosition = new Vector3(12, -3.25f, 0);

	// Reference to the main script
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, timerScript.quickAnimationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, timerScript.quickAnimationTime);
	}

	void OnMouseEnter()
	{
		this.GetComponent<SpriteRenderer>().sprite = hoverSprite;
	}

	void OnMouseDown()
	{
		this.GetComponent<SpriteRenderer>().sprite = clickSprite;
	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		this.GetComponent<SpriteRenderer>().sprite = hoverSprite;

		// Notify our timer script that the pick has been confirmed
		timerScript.PickConfirmed();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}
}
