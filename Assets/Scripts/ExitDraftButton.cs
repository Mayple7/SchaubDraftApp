using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExitDraftButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;
	
	// Button positions
	private Vector3 DisplayPosition = new Vector3(0, -3.5f, 0);
	private Vector3 HiddenPosition = new Vector3(0, -6, 0);

	// Reference to the main script
	private DraftTimerScript timerScript = null;

	// Use this for initialization
	void Start()
	{
		GameObject draftTimer = GameObject.Find("DraftTimer");
		if (draftTimer != null)
		{
			timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Show()
	{
		if (timerScript == null)
		{
			this.transform.DOMove(DisplayPosition, 1.0f);
		}
		else
		{
			this.transform.DOMove(DisplayPosition, timerScript.animationTime);
		}
	}

	public void Hide()
	{
		if (timerScript == null)
		{
			this.transform.DOMove(HiddenPosition, 1.0f);
		}
		else
		{
			this.transform.DOMove(HiddenPosition, timerScript.animationTime);
		}
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
		Application.Quit();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

}
