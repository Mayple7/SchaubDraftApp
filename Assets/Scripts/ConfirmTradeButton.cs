using UnityEngine;
using DG.Tweening;

public class ConfirmTradeButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

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
