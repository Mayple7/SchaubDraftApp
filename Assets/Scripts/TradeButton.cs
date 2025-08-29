using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TradeButton : MonoBehaviour
{
	private enum TradeState
	{
		NotTrading,
		ChooseFirstPlayer,
		ChooseSecondPlayer,
		ChooseFirstPlayerTrade,
		ChooseSecondPlayerTrade,
		ConfirmTrade
	}

	// Sprites for button states
	public Sprite defaultSprite;
	public Sprite hoverSprite;
	public Sprite clickSprite;

	// Button positions
	private Vector3 DisplayPosition = new Vector3(-8.15f, -4.1f, 0);
	private Vector3 HiddenPosition = new Vector3(-10, -4.1f, 0);

	// Reference to the main scripts
	private ContractPlayerScript contractScript;
	private DraftTimerScript timerScript;

	// Objects for smooth movement
	private GameObject leftSide;
	private Vector3 leftSideHidePosition = new Vector3(-8.5f, 0, 0);
	private GameObject rightSide;
	private Vector3 rightSideHidePosition = new Vector3(8.5f, 0, 0);

	// Text for trade scenario.
	private GameObject tradeTextObject;
	private Vector3 tradeTextObjectShowPosition = new Vector3(0, 4.0f, 0);
	private Vector3 tradeTextObjectHidePosition = new Vector3(0, 6.0f, 0);
	private GameObject tradeText;

	// If we're in the trading process.
	private TradeState tradeState;

	private DrafterEnum firstDrafter;
	private DrafterEnum secondDrafter;

	// Black Screen
	private GameObject blackScreen;
	private float blackScreenAlpha = 0.0f;

	// Trade Player Buttons
	public GameObject nextTradeButton;
	public GameObject parentTradePlayerButtonsObject;
	private Vector3 tradePlayerButtonArrayShowPosition = new Vector3(0, 0, 0);
	private Vector3 tradePlayerButtonArrayHidePosition = new Vector3(0, -8.0f, 0);
	private List<TradePlayerSelectionScript> tradePlayerSelectionScriptList = new List<TradePlayerSelectionScript>();

	// Trade capital.
	private List<PickInfo> firstDrafterTradeCapital = new List<PickInfo>();
	private List<PickInfo> secondDrafterTradeCapital = new List<PickInfo>();

	// Trade Confirmation.
	private GameObject parentConfirmationScreen;
	private Vector3 confirmationScreenShow = new Vector3(0, 0, 0);
	private Vector3 confirmationScreenHide = new Vector3(0, -10.0f, 0);
	public GameObject tradeCapitalNameplate;
	public List<GameObject> tradeNameplates = new List<GameObject>();

	void Start()
	{
		this.contractScript = GameObject.Find("ContractPlayerFlow").GetComponent<ContractPlayerScript>();
		this.timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		this.blackScreen = GameObject.Find("TradeBlackScreen");
		this.parentConfirmationScreen = GameObject.Find("ConfirmationScreen");
		foreach (var mesh in this.parentConfirmationScreen.GetComponentsInChildren<MeshRenderer>())
		{
			mesh.sortingLayerName = "TradeStuffs";
		}

		this.leftSide = GameObject.Find("LeftSide");
		this.rightSide = GameObject.Find("RightSide");

		this.tradeText = GameObject.Find("TradeTextObject");
		this.tradeText.GetComponent<MeshRenderer>().sortingLayerName = "TradeStuffs";

		// SUPER UGLY LOLZ
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop1").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop2").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop3").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop4").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop5").GetComponent<TradePlayerSelectionScript>());

		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop6").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop7").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop8").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop9").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop10").GetComponent<TradePlayerSelectionScript>());

		// Extra 2 buttons in case of giving up keepers
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop11").GetComponent<TradePlayerSelectionScript>());
		tradePlayerSelectionScriptList.Add(GameObject.Find("TradeBackdrop12").GetComponent<TradePlayerSelectionScript>());

	}

	void Update()
	{
		// Update the black screen alpha.
		Color currentColor = new Color(1.0f, 1.0f, 1.0f, blackScreenAlpha);
		blackScreen.GetComponent<SpriteRenderer>().color = currentColor;
	}

	public void TryGoNext()
	{
		// First player done picking trade picks.
		if (this.tradeState == TradeState.ChooseFirstPlayerTrade)
		{
			// Save all trade capital for player 1.
			this.SaveTradeCapital(true);
			this.SwitchToTradeState(TradeState.ChooseSecondPlayerTrade);
		}
		else if (this.tradeState == TradeState.ChooseSecondPlayerTrade)
		{
			// Save all trade capital for player 2.
			if (this.SaveTradeCapital(false))
			{
				this.SwitchToTradeState(TradeState.ConfirmTrade);
			}
		}
		else if (this.tradeState == TradeState.ConfirmTrade)
		{
			this.ExecuteTrade();
			this.OnMouseUpAsButton();
		}
	}

	private void SwitchToTradeState(TradeState newTradeState)
	{
		this.tradeState = newTradeState;

		switch(this.tradeState)
		{
			case TradeState.NotTrading:
				timerScript.SwapDraftState();

				// Reset all trade stuffs.
				this.leftSide.transform.DOMove(this.leftSideHidePosition, timerScript.quickAnimationTime);
				this.rightSide.transform.DOMove(this.rightSideHidePosition, timerScript.quickAnimationTime).OnComplete(this.ResetTradeStuffs);
				this.tradeText.transform.DOMove(this.tradeTextObjectHidePosition, timerScript.quickAnimationTime);
				this.parentTradePlayerButtonsObject.transform.DOMove(this.tradePlayerButtonArrayHidePosition, timerScript.quickAnimationTime);
				this.parentConfirmationScreen.transform.DOMove(this.confirmationScreenHide, timerScript.quickAnimationTime);
				this.nextTradeButton.GetComponent<NextTradeButton>().Hide();

				foreach (var nameplate in this.tradeNameplates)
				{
					GameObject.Destroy(nameplate);
				}
				this.tradeNameplates.Clear();

				this.firstDrafterTradeCapital.Clear();
				this.secondDrafterTradeCapital.Clear();
				break;
			case TradeState.ChooseFirstPlayer:
				// Show all the drafter stuff.
				this.leftSide.transform.DOMove(Vector3.zero, timerScript.quickAnimationTime);
				this.rightSide.transform.DOMove(Vector3.zero, timerScript.quickAnimationTime);

				this.tradeText.transform.DOMove(this.tradeTextObjectShowPosition, timerScript.quickAnimationTime);
				this.tradeText.GetComponent<TextMeshPro>().text = "Choose First Drafter:";
				break;
			case TradeState.ChooseSecondPlayer:
				this.tradeText.GetComponent<TextMeshPro>().text = "Choose Second Drafter:";
				break;
			case TradeState.ChooseFirstPlayerTrade:
				// Hide all the drafter stuff.
				this.leftSide.transform.DOMove(this.leftSideHidePosition, timerScript.quickAnimationTime);
				this.rightSide.transform.DOMove(this.rightSideHidePosition, timerScript.quickAnimationTime);

				this.SetupPlayerButtonsFor(this.firstDrafter);

				this.nextTradeButton.GetComponent<NextTradeButton>().Show();
				break;
			case TradeState.ChooseSecondPlayerTrade:
				this.parentTradePlayerButtonsObject.transform.DOMove(this.tradePlayerButtonArrayHidePosition, timerScript.quickAnimationTime).OnComplete(()=>this.SetupPlayerButtonsFor(this.secondDrafter));
				break;
			case TradeState.ConfirmTrade:
				// Hide button array.
				// Show trade confirmation.
				this.tradeText.transform.DOMove(this.tradeTextObjectHidePosition, timerScript.quickAnimationTime);
				this.parentTradePlayerButtonsObject.transform.DOMove(this.tradePlayerButtonArrayHidePosition, timerScript.quickAnimationTime).OnComplete(this.BuildAndShowConfirmation);
				this.nextTradeButton.GetComponent<NextTradeButton>().Hide();
				break;
		}
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

		// Start the trade process.
		if (this.tradeState == TradeState.NotTrading)
		{
			timerScript.GoToDraftState(DraftState.DraftPaused);
			this.SwitchToTradeState(TradeState.ChooseFirstPlayer);
			
			DOTween.To(() => this.blackScreenAlpha, x => this.blackScreenAlpha = x, 0.75f, timerScript.animationTime);
		}
		// End or cancel the trade process.
		else
		{
			this.SwitchToTradeState(TradeState.NotTrading);

			DOTween.To(() => this.blackScreenAlpha, x => this.blackScreenAlpha = x, 0.0f, timerScript.animationTime);
		}
	}

	private void OnMouseExit()
	{
		this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
	}

	private void ResetTradeStuffs()
	{
		// Reset draft button locations.
		foreach (var buttonScript in this.leftSide.GetComponentsInChildren<TradeNameplateButton>())
		{
			buttonScript.ResetToStartPosition();
		}

		foreach (var buttonScript in this.rightSide.GetComponentsInChildren<TradeNameplateButton>())
		{
			buttonScript.ResetToStartPosition();
		}

		this.firstDrafter = DrafterEnum.TotalDrafters;
		this.secondDrafter = DrafterEnum.TotalDrafters;
	}

	public void SetDrafter(DrafterEnum drafter)
	{
		// First drafter chosen
		if (this.tradeState == TradeState.ChooseFirstPlayer)
		{
			this.firstDrafter = drafter;
			GameObject.Find("FirstDrafterName").GetComponent<TextMeshPro>().text = timerScript.DrafterNames[(int)this.firstDrafter] + " Trades:";

			this.SwitchToTradeState(TradeState.ChooseSecondPlayer);
		}
		else if (this.tradeState == TradeState.ChooseSecondPlayer)
		{
			this.secondDrafter = drafter;
			GameObject.Find("SecondDrafterName").GetComponent<TextMeshPro>().text = timerScript.DrafterNames[(int)this.secondDrafter] + " Trades:";

			this.SwitchToTradeState(TradeState.ChooseFirstPlayerTrade);
		}
	}

	private void SetupPlayerButtonsFor(DrafterEnum drafter)
	{
		int tradePlayerScriptListIndex = 0;

		// Get all the relevant main draft pick infos.
		foreach (var pickRound in timerScript.pickInfo)
		{
			foreach (var pickInfo in pickRound)
			{
				// Pick is owned by this drafter
				if (pickInfo.drafterID == drafter)
				{
					tradePlayerSelectionScriptList[tradePlayerScriptListIndex].SetPickInfo(pickInfo);
					++tradePlayerScriptListIndex;
				}
			}
		}

		// Empty out these buttons
		while (tradePlayerScriptListIndex < tradePlayerSelectionScriptList.Count)
		{
			tradePlayerSelectionScriptList[tradePlayerScriptListIndex].SetPickInfo(null);
			++tradePlayerScriptListIndex;
		}

		// Show the array.
		this.parentTradePlayerButtonsObject.transform.DOMove(this.tradePlayerButtonArrayShowPosition, timerScript.quickAnimationTime);

		this.tradeText.GetComponent<TextMeshPro>().text = string.Format("Choose who {0} is giving up:", timerScript.DrafterNames[(int)drafter]);
	}

	private bool SaveTradeCapital(bool isFirstDrafter)
	{
		foreach (var tradeScript in this.tradePlayerSelectionScriptList)
		{
			PickInfo selectedPick = tradeScript.GetSelectedPick();
			if (selectedPick != null)
			{
				if (isFirstDrafter)
				{
					this.firstDrafterTradeCapital.Add(selectedPick);
				}
				else
				{
					this.secondDrafterTradeCapital.Add(selectedPick);
				}
			}
		}

		if (!isFirstDrafter && this.firstDrafterTradeCapital.Count != this.secondDrafterTradeCapital.Count)
		{
			this.secondDrafterTradeCapital.Clear();
		}

		return isFirstDrafter || this.firstDrafterTradeCapital.Count == this.secondDrafterTradeCapital.Count;
	}

	private void BuildAndShowConfirmation()
	{
		float startYPos = -8.0f;
		float currentYPos = startYPos;

		print("===== FIRST DRAFTER =====");
		foreach (var pickInfo in this.firstDrafterTradeCapital)
		{
			GameObject currentNameplate = Instantiate(tradeCapitalNameplate, new Vector3(-5.5f, currentYPos, 0), Quaternion.identity);
			currentNameplate.GetComponentInChildren<MeshRenderer>().sortingLayerName = "TradeStuffs";
			currentNameplate.transform.parent = this.parentConfirmationScreen.transform;
			this.tradeNameplates.Add(currentNameplate);

			if (pickInfo.playerPicked != null)
			{
				currentNameplate.GetComponentInChildren<TextMeshPro>().text = pickInfo.playerPicked.playerName;

				print(string.Format("{0} is giving up: {1}", timerScript.DrafterNames[(int)this.firstDrafter], pickInfo.playerPicked.playerName));
			}
			else
			{
				currentNameplate.GetComponentInChildren<TextMeshPro>().text = string.Format("Round {0}, Pick {1}", pickInfo.roundNumber + 1, pickInfo.pickNumber + 1);

				print(string.Format("{0} is giving up: Round {1}, Pick {2}", timerScript.DrafterNames[(int)this.firstDrafter], pickInfo.roundNumber + 1, pickInfo.pickNumber + 1));
			}

			currentYPos -= 1.5f;
		}

		// Reset Y Position.
		currentYPos = startYPos;

		print("===== SECOND DRAFTER =====");
		foreach (var pickInfo in this.secondDrafterTradeCapital)
		{
			GameObject currentNameplate = Instantiate(tradeCapitalNameplate, new Vector3(5.5f, currentYPos, 0), Quaternion.identity);
			currentNameplate.GetComponentInChildren<MeshRenderer>().sortingLayerName = "TradeStuffs";
			currentNameplate.transform.parent = this.parentConfirmationScreen.transform;
			this.tradeNameplates.Add(currentNameplate);

			if (pickInfo.playerPicked != null)
			{
				currentNameplate.GetComponentInChildren<TextMeshPro>().text = pickInfo.playerPicked.playerName;

				print(string.Format("{0} is giving up: {1}", timerScript.DrafterNames[(int)this.secondDrafter], pickInfo.playerPicked.playerName));
			}
			else
			{
				currentNameplate.GetComponentInChildren<TextMeshPro>().text = string.Format("Round {0}, Pick {1}", pickInfo.roundNumber + 1, pickInfo.pickNumber + 1);

				print(string.Format("{0} is giving up: Round {1}, Pick {2}", timerScript.DrafterNames[(int)this.secondDrafter], pickInfo.roundNumber + 1, pickInfo.pickNumber + 1));
			}

			currentYPos -= 1.5f;
		}

		this.parentConfirmationScreen.transform.DOMove(this.confirmationScreenShow, timerScript.quickAnimationTime);
	}

	private void ExecuteTrade()
	{
		foreach (var pickInfo in this.firstDrafterTradeCapital)
		{
			pickInfo.drafterID = this.secondDrafter;

			// Pick was already made.
			if (pickInfo.playerPicked != null)
			{
				timerScript.playerProfiles[(int)this.firstDrafter].allPlayerPicks.Remove(pickInfo.playerPicked);

				timerScript.playerProfiles[(int)this.secondDrafter].allPlayerPicks.Add(pickInfo.playerPicked);
			}
		}

		foreach (var pickInfo in this.secondDrafterTradeCapital)
		{
			pickInfo.drafterID = this.firstDrafter;

			// Pick was already made.
			if (pickInfo.playerPicked != null)
			{
				timerScript.playerProfiles[(int)this.secondDrafter].allPlayerPicks.Remove(pickInfo.playerPicked);

				timerScript.playerProfiles[(int)this.firstDrafter].allPlayerPicks.Add(pickInfo.playerPicked);
			}
		}
	}
}
