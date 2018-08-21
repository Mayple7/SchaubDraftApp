using TMPro;
using UnityEngine;

public class TradePlayerSelectionScript : MonoBehaviour
{
	// Sprites for button states
	public Sprite unselectedSprite;
	public Sprite selectedSprite;

	// Reference to the main script
	private DraftTimerScript timerScript;

	private bool buttonSelected;

	private PickInfo thisPickInfo;

	// Use this for initialization
	void Start ()
	{
		this.GetComponentInChildren<MeshRenderer>().sortingLayerName = "TradeStuffs";
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Set which pick this button is associated with and reset the selected status.
	public void SetPickInfo(PickInfo pickInfo)
	{
		this.thisPickInfo = pickInfo;
		if (this.buttonSelected)
		{
			this.OnMouseUpAsButton();
		}

		if (pickInfo == null)
		{
			this.GetComponentInChildren<TextMeshPro>().text = "DO NOT SELECT";
			return;
		}

		// Set the button's text.
		if (pickInfo.playerPicked == null)
		{
			this.GetComponentInChildren<TextMeshPro>().text = string.Format("Round {0}, Pick {1}", pickInfo.roundNumber + 1, pickInfo.pickNumber + 1);
		}
		else
		{
			this.GetComponentInChildren<TextMeshPro>().text = pickInfo.playerPicked.playerName;
		}
	}

	public PickInfo GetSelectedPick()
	{
		if (buttonSelected)
		{
			return this.thisPickInfo;
		}

		return null;
	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		// No pick info for this button.
		if (this.thisPickInfo == null)
		{
			buttonSelected = false;
		}
		else
		{
			buttonSelected = !buttonSelected;
		}

		// Selected player.
		if (buttonSelected)
		{
			this.GetComponent<SpriteRenderer>().sprite = selectedSprite;
		}
		else
		{
			// Unselecting the player.
			this.GetComponent<SpriteRenderer>().sprite = unselectedSprite;
		}
	}
}
