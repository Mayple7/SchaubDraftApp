using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NextTradeButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(7.5f, -3.0f, 0);
	private Vector3 HiddenPosition = new Vector3(11, -3.0f, 0);

	// Reference to the main script
	private TradeButton tradeButtonScript;
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start()
	{
		tradeButtonScript = GameObject.Find("TradeButton").GetComponent<TradeButton>();
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

		// Notify the trade button script that the next part can begin.
		tradeButtonScript.TryGoNext();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

}
