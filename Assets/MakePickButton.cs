using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MakePickButton : MonoBehaviour
{
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	public float AnimationTime = 0.5f;

	private Vector3 DisplayPosition = new Vector3(0, -3, 0);
	private Vector3 HiddenPosition = new Vector3(0, -6, 0);

	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, AnimationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, AnimationTime);
	}

	// Right button down
	void OnMouseEnter()
	{
		if (gameObject.GetComponent<SpriteRenderer>().enabled)
		{
			this.GetComponent<SpriteRenderer>().sprite = hoverSprite;
		}
	}

	void OnMouseDown()
	{
		if (gameObject.GetComponent<SpriteRenderer>().enabled)
		{
			this.GetComponent<SpriteRenderer>().sprite = clickSprite;
		}
	}

	private void OnMouseUpAsButton()
	{
		this.GetComponent<SpriteRenderer>().sprite = hoverSprite;

		if (!timerScript)
		{
			timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		}

		// Notify our timer script that the pick has been confirmed
		timerScript.PickIsIn();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}
	
}
