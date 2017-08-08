using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum DrafterEnum
{
  Dan,
  Doug,
  Sheebs,
  Patrick,

  Ben,
  Trevor,
  Kopman,
  Hans,

  Parks,
  Colin,
  Drew,
  Jake,
  TotalDrafters
}

public enum DraftState
{
	DraftPaused,
	AnimateToNextDrafter,
	CountdownPickTimer,
	CountdownBonusTimer,
	ThePickIsIn,
	DraftFinished
}

public class PlayerProfile
{
  public Sprite playerNameplate;
  public float bonusTimeRemaining;

	public float totalTimeUsed = 0;
}

public class PickInfo
{
  public int roundNumber;
  public int pickNumber;

  public DrafterEnum drafterID;
  public string playerPicked;
}

public class DraftTimerScript : MonoBehaviour
{
  public string[] DrafterNames =
  {
    "Dan",
    "Doug",
    "Sheebs",
    "Patrick",

    "Ben",
    "Trevor",
    "Kopman",
    "Hans",

    "Parks",
    "Colin",
    "Drew",
    "Jake"
  };

  public DrafterEnum[] DraftOrder =
  {
    DrafterEnum.Patrick,
    DrafterEnum.Jake,
    DrafterEnum.Trevor,
    DrafterEnum.Kopman,
    DrafterEnum.Colin,
    DrafterEnum.Doug,
    DrafterEnum.Sheebs,
    DrafterEnum.Parks,
    DrafterEnum.Ben,
    DrafterEnum.Hans,
    DrafterEnum.Drew,
    DrafterEnum.Dan
  };

	// Ring Objects
	public List<GameObject> RingObjects;
	public Color ringGreen;
	public Color ringYellow;
	public Color ringRed;
	private float ringRValue = 1;
	private float ringGValue = 1;
	private float ringBValue = 1;
	private float ringAValue = 0.125f;

	public List<Sprite> nameplateSprites;
  public PlayerProfile[] playerProfiles;
  public int totalRounds = 15;

  // Pick time variables
  public float maxBonusTime = 5 * 60; // 5 min bonus time
  private float maxPickTime = 5.0f;      // 1 min pick timer
  private float currentPickTime = 0;

	public GameObject TimerStateOverlay;
	public Sprite OnTheClockSprite;
	public Sprite BonusTimeSprite;
	public Sprite RandomPickSprite;

  // Draft data
  private List<PickInfo>[] pickInfo;  // Array of List of PickInfos
  private int currentRound = 0;
  private int currentPick = 0;

	private int tickerRound = 0;
	private int tickerPick = 0;

	public GameObject RoundLabel;
	public GameObject PickLabel;
	public GameObject ReleaseKeeperButton;

	private DraftState currentDraftState = DraftState.DraftPaused;
	private DraftState previousDraftState = DraftState.AnimateToNextDrafter;

	public GameObject draftOrderTickerTemplate;
	private DraftOrderTicker draftOrderTicker;

	public GameObject pickHistoryTicker;

	private Text pickTimerText;

	// Use this for initialization
	void Start ()
  {
    // Create each of the player profiles
    playerProfiles = new PlayerProfile[(int)DrafterEnum.TotalDrafters];
    for(int i = 0; i < (int)DrafterEnum.TotalDrafters; ++i)
    {
      playerProfiles[i] = new PlayerProfile();
      playerProfiles[i].bonusTimeRemaining = maxBonusTime;
      playerProfiles[i].playerNameplate = nameplateSprites[i];
    }

    // Create the pick info structures
    int draftSnake = 1;
    int currentDrafter = 0;
    pickInfo = new List<PickInfo>[totalRounds];
    for(int i = 0; i < totalRounds; ++i)
    {
      pickInfo[i] = new List<PickInfo>();
      for(int j = 0; j < (int)DrafterEnum.TotalDrafters; ++j)
      {
        pickInfo[i].Add(new PickInfo());
        pickInfo[i][j].roundNumber = i;
        pickInfo[i][j].pickNumber = j;
        pickInfo[i][j].drafterID = DraftOrder[currentDrafter];

        currentDrafter += draftSnake;
      }

      draftSnake *= -1;
      currentDrafter += draftSnake;
    }

    currentPickTime = maxPickTime;

		// Start the draft order ticker script
		var tempObj = Instantiate(draftOrderTickerTemplate);
		draftOrderTicker = tempObj.GetComponent<DraftOrderTicker>();

		for (int i = 0; i < draftOrderTicker.GetMaxNameplates(); ++i)
		{
			draftOrderTicker.AddNameplateToTicker(pickInfo[tickerRound][tickerPick].drafterID);

			++tickerPick;
			if (tickerPick >= pickInfo[tickerRound].Count)
			{
				tickerPick = 0;

				++tickerRound;

				if (tickerRound >= totalRounds)
				{
					break;
				}
			}
		}

		pickTimerText = GameObject.Find("PickTimer").GetComponent<Text>();
		pickTimerText.text = FormatTimeText(currentPickTime);

		int time = 15;
		int rotationValue = 1;
		foreach(var ring in RingObjects)
		{
			ring.transform.DORotate(new Vector3(0, 0, 180 * rotationValue), time).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

			rotationValue *= -1;
			time += 2;
		}
	}

	public void BeginDraft()
	{
		GameObject.Find("BeginDraftButton").transform.DOMoveY(-10, animationTime);

		RoundLabel.transform.DOMoveX(-5.6f, animationTime);
		PickLabel.transform.DOMoveX(-5.6f, animationTime);
		pickTimerText.transform.DOMoveY(-1.4f, animationTime);
		TimerStateOverlay.transform.DOMoveY(-0.75f, animationTime);
		makePickButton.transform.DOMoveY(-3, animationTime);

		GoToDraftState(DraftState.AnimateToNextDrafter);
	}
	
	// Update is called once per frame
	void Update ()
  {
		switch(currentDraftState)
		{
			case DraftState.DraftPaused:
				break;
			case DraftState.AnimateToNextDrafter:
				AnimateToNextDrafter();
				break;
			case DraftState.CountdownPickTimer:
				CountdownPickTimer();
				break;
			case DraftState.CountdownBonusTimer:
				CountdownBonusTimer();
				break;
			case DraftState.ThePickIsIn:
				break;
			case DraftState.DraftFinished:
				DraftFinishedAnimation();
				break;
		}

		// Adjust the ring colors
		Color currentColor = new Color(ringRValue, ringGValue, ringBValue, ringAValue);
		foreach (var ring in RingObjects)
		{
			ring.GetComponent<Image>().color = currentColor;
		}
	}

	// Drafter nameplates
	private GameObject newDrafterNameplate = null;
	private GameObject oldDrafterNameplate = null;
	private bool animationInitialized = false;

	private float endingNameplateX = -11.0f;
	public float animationTime = 2.0f;

	private float currentAnimationTime = 0;

	private float currentAlphaValue = 0;

	private void AnimateToNextDrafter()
	{
		// Wait during animation
		if (animationInitialized)
		{
			currentAnimationTime += Time.deltaTime;

			if(newDrafterNameplate != null)
			{
				newDrafterNameplate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, currentAlphaValue);
			}

			// Animation is done
			if (currentAnimationTime >= animationTime)
			{
				currentAnimationTime = 0;
				GoToDraftState(DraftState.CountdownPickTimer);
				animationInitialized = false;
				pickTimerText.color = WhiteOpaque;

				currentPickTime = maxPickTime;

				TimerStateOverlay.GetComponent<SpriteRenderer>().sprite = OnTheClockSprite;
			}
		}
		else
		{
			// Kill the old nameplate
			if(oldDrafterNameplate)
			{
				GameObject.Destroy(oldDrafterNameplate);
			}

			// Add the next nameplate and update the ticker round/pick values
			if (tickerRound < totalRounds)
			{
				draftOrderTicker.AddNameplateToTicker(pickInfo[tickerRound][tickerPick].drafterID);

				++tickerPick;
				if (tickerPick >= pickInfo[tickerRound].Count)
				{
					tickerPick = 0;

					++tickerRound;
				}
			}

			// Grab the new nameplate
			oldDrafterNameplate = newDrafterNameplate;
			newDrafterNameplate = draftOrderTicker.GrabNextNameplate();

			// Animate the old and new nameplates
			if (oldDrafterNameplate)
			{
				oldDrafterNameplate.transform.DOMoveX(endingNameplateX, animationTime).SetEase(Ease.InQuad);
			}

			if (newDrafterNameplate)
			{
				newDrafterNameplate.transform.DOMoveX(0, animationTime).SetEase(Ease.InOutQuad);
				newDrafterNameplate.transform.DOMoveY(0, animationTime).SetEase(Ease.OutQuad);

				currentAlphaValue = newDrafterNameplate.GetComponent<SpriteRenderer>().color.a;
				DOTween.To(()=>currentAlphaValue, x=>currentAlphaValue = x, 1.0f, animationTime);
			}

			// Ensure the ring colors are green
			DOTween.To(() => ringRValue, x => ringRValue = x, ringGreen.r, animationTime);
			DOTween.To(() => ringGValue, x => ringGValue = x, ringGreen.g, animationTime);
			DOTween.To(() => ringBValue, x => ringBValue = x, ringGreen.b, animationTime);
			DOTween.To(() => ringAValue, x => ringAValue = x, ringGreen.a, animationTime);

			// Animation has been initialized
			animationInitialized = true;
		}
	}

	public Color WhiteOpaque = new Color(1, 1, 1, 1);
	public Color WhiteFaded = new Color(1, 1, 1, 0.25f);

	private void CountdownPickTimer()
	{
		currentPickTime -= Time.deltaTime;

		// Current pick time is over
		if (currentPickTime <= 0)
		{
			// Ensure the ring colors are yellow
			DOTween.To(() => ringRValue, x => ringRValue = x, ringYellow.r, animationTime);
			DOTween.To(() => ringGValue, x => ringGValue = x, ringYellow.g, animationTime);
			DOTween.To(() => ringBValue, x => ringBValue = x, ringYellow.b, animationTime);
			DOTween.To(() => ringAValue, x => ringAValue = x, ringYellow.a, animationTime);

			// Adjust background ring colors
			Color currentColor = new Color(ringRValue, ringGValue, ringBValue, 0.125f);
			foreach (var ring in RingObjects)
			{
				ring.GetComponent<Image>().color = currentColor;
			}

			TimerStateOverlay.GetComponent<SpriteRenderer>().sprite = BonusTimeSprite;
			currentPickTime = 0;

			// Set bonus time alpha to maximum and normal timer to smaller color
			newDrafterNameplate.GetComponentInChildren<TextMesh>().color = WhiteOpaque;
			pickTimerText.color = WhiteFaded;

			GoToDraftState(DraftState.CountdownBonusTimer);
		}

		pickTimerText.text = FormatTimeText(currentPickTime);
	}

	private void CountdownBonusTimer()
	{
		playerProfiles[(int)pickInfo[currentRound][currentPick].drafterID].bonusTimeRemaining -= Time.deltaTime;

		// Current bonus time is over
		if (playerProfiles[(int)pickInfo[currentRound][currentPick].drafterID].bonusTimeRemaining <= 0)
		{
			// Ensure the ring colors are red
			DOTween.To(() => ringRValue, x => ringRValue = x, ringRed.r, animationTime);
			DOTween.To(() => ringGValue, x => ringGValue = x, ringRed.g, animationTime);
			DOTween.To(() => ringBValue, x => ringBValue = x, ringRed.b, animationTime);
			DOTween.To(() => ringAValue, x => ringAValue = x, ringRed.a, animationTime);

			playerProfiles[(int)pickInfo[currentRound][currentPick].drafterID].bonusTimeRemaining = 0;

			// Set the timer overlay to be random :(
			TimerStateOverlay.GetComponent<SpriteRenderer>().sprite = RandomPickSprite;

			// Go to the pick is in
			PickIsIn();
		}

		pickTimerText.text = FormatTimeText(currentPickTime);
	}

	public string FormatTimeText(float time)
	{
		string minutes = Mathf.Floor(time / 60).ToString();
		string seconds = (time % 60).ToString("00");

		return minutes + ":" + seconds;
	}

	public GameObject makePickButton;
	public GameObject textBoxObject;
	public GameObject confirmPickButton;

	// Pick is in
	public void PickIsIn()
	{
		// Don't activate if timer is not active
		if(currentDraftState != DraftState.CountdownPickTimer && currentDraftState != DraftState.CountdownBonusTimer)
		{
			return;
		}

		// Initialize the pick is in
		if(currentDraftState != DraftState.ThePickIsIn)
		{
			GoToDraftState(DraftState.ThePickIsIn);
		}

		// Animate the pick is in.
		makePickButton.GetComponent<MakePickButton>().Hide();
		textBoxObject.GetComponent<InputPickScript>().Show();
		confirmPickButton.GetComponent<ConfirmPickButton>().Show();

	}

	// Pick has been confirmed with button press
	public void PickConfirmed()
	{
		// Grab the pick text and clear the textbox.
		pickInfo[currentRound][currentPick].playerPicked = textBoxObject.GetComponent<InputField>().text;
		pickHistoryTicker.GetComponent<PickHistoryTicker>().AddPickToHistory(pickInfo[currentRound][currentPick].playerPicked);
		textBoxObject.GetComponent<InputField>().text = "";
		textBoxObject.GetComponent<InputPickScript>().Hide();
		confirmPickButton.GetComponent<ConfirmPickButton>().Hide();

		makePickButton.GetComponent<MakePickButton>().Show();

		// Cycle the pick now
		if (CycleToNextPick())
		{
			return;
		}

		// Start next pick animation
		GoToDraftState(DraftState.AnimateToNextDrafter);
	}

	public void ReleaseKeeper()
	{
		// Don't activate if timer is not active
		if (currentDraftState != DraftState.CountdownPickTimer && currentDraftState != DraftState.CountdownBonusTimer)
		{
			return;
		}

		PickInfo newPick = new PickInfo();
		newPick.drafterID = pickInfo[currentRound][currentPick].drafterID;
		switch (currentRound)
		{
			case 1:
				newPick.roundNumber = 4;
				newPick.pickNumber = pickInfo[newPick.roundNumber].Count;
				pickInfo[newPick.roundNumber].Add(newPick);
				break;
			case 2:
				newPick.roundNumber = 3;
				newPick.pickNumber = 12;
				pickInfo[newPick.roundNumber].Insert(12, newPick);
				for(int i = 12; i < pickInfo[newPick.roundNumber].Count; ++i)
				{
					pickInfo[newPick.roundNumber][i].pickNumber = i;
				}
				break;
			default:
				return;
		}
		
		if(CycleToNextPick())
		{
			return;
		}
		
		// Start next pick animation
		GoToDraftState(DraftState.AnimateToNextDrafter);
	}

	private void GoToDraftState(DraftState newDraftState)
	{
		previousDraftState = currentDraftState;
		currentDraftState = newDraftState;
	}

	public void UpdateLabels()
	{
		if(RoundLabel && PickLabel)
		{
			RoundLabel.GetComponent<Text>().text = "Round: " + (currentRound + 1);
			PickLabel.GetComponent<Text>().text = "Pick: " + (currentPick + 1);
		}
	}

	// Returns true if draft is over
	public bool CycleToNextPick()
	{
		// Add time to the profile
		playerProfiles[(int)pickInfo[currentRound][currentPick].drafterID].totalTimeUsed += (maxPickTime - currentPickTime);

		// Cycle the pick now
		++currentPick;
		currentPickTime = maxPickTime;

		if (currentPick >= pickInfo[currentRound].Count)
		{
			currentPick = 0;
			++currentRound;

			if (currentRound >= totalRounds)
			{
				SerializeDraftData();
				CleanupDraftUI();
				return true;
			}

			// Animate the release keeper button
			if (currentRound == 1)
			{
				ReleaseKeeperButton.transform.DOMoveX(-5.7f, animationTime);
			}
			if (currentRound == 3)
			{
				ReleaseKeeperButton.transform.DOMoveX(-12, animationTime);
			}
		}

		UpdateLabels();

		return false;
	}

	public void CleanupDraftUI()
	{
		GameObject.Find("ExitDraftButton").GetComponent<ExitDraftButton>().Show();
		GameObject.Find("TickerBackground").transform.DOMoveY(10, animationTime);

		// Hide the text
		RoundLabel.transform.DOMoveX(-12, animationTime);
		PickLabel.transform.DOMoveX(-12, animationTime);
		pickTimerText.transform.DOMoveY(-10, animationTime);
		TimerStateOverlay.transform.DOMoveY(-8, animationTime);

		ReleaseKeeperButton.GetComponent<ReleaseKeeperButton>().Hide();

		GoToDraftState(DraftState.DraftFinished);

		// Hide UI stuff
		makePickButton.GetComponent<MakePickButton>().Hide();
		newDrafterNameplate.transform.DOMoveX(endingNameplateX, animationTime).SetEase(Ease.InQuad);

		pickHistoryTicker.GetComponent<PickHistoryTicker>().HideHistoryTicker();
	}

	public void SerializeDraftData()
  {
    print("SERIALIZE DRAFT DATA");
    string filename = "/DraftResults.txt";
    string filepath = Application.persistentDataPath + filename;
    print(filepath);

    StreamWriter writer;
    // Remove the old draft data
    if(File.Exists(filepath))
    {
      File.Delete(filepath);
    }

    writer = new StreamWriter(filepath, true);
    writer.WriteLine("Round:Pick:Drafter:PickedPlayer");
    for (int i = 0; i < totalRounds; ++i)
    {
      for (int j = 0; j < (int)pickInfo[i].Count; ++j)
      {
        PickInfo pick = pickInfo[i][j];
        string lineToWrite = (pick.roundNumber + 1) + ":" + (pick.pickNumber + 1) + ":" + DrafterNames[(int)pick.drafterID] + ":" + pick.playerPicked;
        writer.WriteLine(lineToWrite);
      }
    }

		writer.WriteLine("Total Time Used:Bonus Time Used");

		for (int i = 0; i < (int)DrafterEnum.TotalDrafters; ++i)
		{
			PlayerProfile currentPlayer = playerProfiles[i];
			currentPlayer.totalTimeUsed += (maxBonusTime - currentPlayer.bonusTimeRemaining);

			writer.WriteLine(DrafterNames[i] + ":" + currentPlayer.totalTimeUsed + ":" + (maxBonusTime - currentPlayer.bonusTimeRemaining));
		}

    writer.Close();
  }

	public DrafterEnum[] GetDraftOrder()
	{
		return DraftOrder;
	}

	public GameObject DraftCompleteMessage;
	public GameObject FinishedBarTemplate;

	// Min/Max bar height
	public float minBarHeight;
	public float maxBarHeight;

	// Min/Max bar scale
	public float minBarScale;
	public float maxBarScale;

	// Animation time (speed)
	public float minBarTime;
	public float maxBarTime;

	public class FinishedBar
	{
		public GameObject barObject;
		public float timeAlive;
	}

	// Bar spawn timer
	private float nextBarSpawnTime;
	private float barSpawningTimer;
	public float minBarSpawningTime;
	public float maxBarSpawningTime;

	private List<FinishedBar> finishedBarList;
	bool finishedDraftAnimationStarted = false;
	public void DraftFinishedAnimation()
	{
		if(!finishedDraftAnimationStarted)
		{
			DraftCompleteMessage.transform.DOMoveX(0, animationTime / 2.0f).SetEase(Ease.InQuad);
			finishedDraftAnimationStarted = true;

			finishedBarList = new List<FinishedBar>();
		}
		else
		{
			// Check for bar spawning
			barSpawningTimer += Time.deltaTime;

			// Bar should spawn
			if(barSpawningTimer >= nextBarSpawnTime)
			{
				// Set up the timer for the next bar
				barSpawningTimer = 0;
				nextBarSpawnTime = Random.Range(minBarSpawningTime, maxBarSpawningTime);

				for(int i = 0; i < 4; ++i)
				{
					FinishedBar newBar = new FinishedBar();
					float barScale = Random.Range(minBarScale, maxBarScale);
					float barHeight = Random.Range(minBarHeight, maxBarHeight);
					newBar.barObject = Instantiate(FinishedBarTemplate, new Vector3(-21, barHeight, 1), Quaternion.identity);
					newBar.barObject.transform.localScale = new Vector3(barScale, barScale);
					newBar.barObject.GetComponent<SpriteRenderer>().color = new Color(0, 0.7764f + Random.Range(-0.2f, 0.2f), 0.6117f + Random.Range(-0.2f, 0.2f), 0.5f);
					newBar.timeAlive = Random.Range(minBarTime, maxBarTime);
					finishedBarList.Add(newBar);
					newBar.barObject.transform.DOMoveX(21, newBar.timeAlive);
				}
			}
			
			// Loop through the bar to update timer and check to destroy bars
			for(int i = 0; i < finishedBarList.Count; ++i)
			{
				finishedBarList[i].timeAlive -= Time.deltaTime;

				if (finishedBarList[i].timeAlive <= 0)
				{
					Destroy(finishedBarList[i].barObject);
					finishedBarList.RemoveAt(i);
					--i;
				}
			}
		}
	}
}
