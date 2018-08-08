using UnityEngine;
using DG.Tweening;

public class TradeButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(-7.425f, -3.75f, 0);
	private Vector3 HiddenPosition = new Vector3(-11, -3.75f, 0);

	// Reference to the main scripts
	private ContractPlayerScript contractScript;
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start()
	{
		contractScript = GameObject.Find("ContractPlayerFlow").GetComponent<ContractPlayerScript>();
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, contractScript.animationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, contractScript.animationTime);
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
		//contractScript.GoToNextState();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}
}
