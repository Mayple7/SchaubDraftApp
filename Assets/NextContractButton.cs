using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NextContractButton : MonoBehaviour
{
	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(7.5f, -3.75f, 0);
	private Vector3 HiddenPosition = new Vector3(11, -3.75f, 0);

	// Reference to the main script
	private ContractPlayerScript contractScript;

	// Use this for initialization
	void Start()
	{
		contractScript = GameObject.Find("ContractPlayerFlow").GetComponent<ContractPlayerScript>();
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
		contractScript.GoToNextState();
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

}
