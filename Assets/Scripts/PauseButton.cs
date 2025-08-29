using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseButton : MonoBehaviour
{
	// Different sprites for pause/play states
	public Sprite pauseSprite;
	public Sprite pauseHoverSprite;

	public Sprite playSprite;
	public Sprite playHoverSprite;

	// Current sprites on the button
	private Sprite currentSprite;
	private Sprite currentHoverSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(8.5f, -4.0f, 0);
	private Vector3 HiddenPosition = new Vector3(8.5f, -5.5f, 0);

	// Reference to the main script
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start()
	{
		// Set current sprite to pause sprites
		currentSprite = pauseSprite;
		currentHoverSprite = pauseHoverSprite;

		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, timerScript.animationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, timerScript.animationTime);
	}

	void OnMouseEnter()
	{
		this.GetComponent<SpriteRenderer>().sprite = currentHoverSprite;
	}

	void OnMouseDown()
	{
		this.GetComponent<SpriteRenderer>().sprite = currentSprite;
	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		// Pausing the draft
		if(currentSprite == pauseSprite)
		{
			timerScript.GoToDraftState(DraftState.DraftPaused);
			currentSprite = playSprite;
			currentHoverSprite = playHoverSprite;
		}
		else
		{
			// Continue the draft
			timerScript.SwapDraftState();
			currentSprite = pauseSprite;
			currentHoverSprite = pauseHoverSprite;
		}

		// Ensure we get our new hover sprite in place
		this.GetComponent<SpriteRenderer>().sprite = currentHoverSprite;
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = currentSprite;
	}

}
