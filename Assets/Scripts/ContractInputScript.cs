using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ContractInputScript : MonoBehaviour
{
	// Sprites for found player
	public Sprite singleCheckMark;
	public Sprite multipleCheckMarks;
	public Sprite redXMark;

	// Input field positions
	private Vector3 DisplayPosition = new Vector3(-1.9f, -3, 0);
	private Vector3 HiddenPosition = new Vector3(-15, -3, 0);

	// Reference to the main script
	private DraftTimerScript timerScript;

	public GameObject checkMarkObject;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (gameObject.GetComponent<InputField>().IsInteractable())
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				int numMatches = timerScript.playerDatabase.NumMatchingPlayers(gameObject.GetComponent<InputField>().text);
				// Player Found
				if (numMatches == 1)
				{
					gameObject.GetComponent<InputField>().text = timerScript.playerDatabase.GetSearchResult();
					gameObject.GetComponent<InputField>().caretPosition = gameObject.GetComponent<InputField>().text.Length;
				}
			}
		}
	}

	public void UpdatePlayersMatched()
	{
		if(!timerScript)
		{
			timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		}

		int numMatches = timerScript.playerDatabase.NumMatchingPlayers(gameObject.GetComponent<InputField>().text);

		// Player Found
		if (numMatches == 1)
		{
			checkMarkObject.GetComponent<SpriteRenderer>().sprite = singleCheckMark;
		}
		else if (numMatches == 0)
		{
			checkMarkObject.GetComponent<SpriteRenderer>().sprite = redXMark;
		}
		else
		{
			checkMarkObject.GetComponent<SpriteRenderer>().sprite = multipleCheckMarks;
		}
	}
}
