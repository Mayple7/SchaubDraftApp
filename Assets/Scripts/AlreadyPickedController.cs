using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlreadyPickedController : MonoBehaviour
{
	// Best available order
	enum RecentlyPickedOrder
	{
		RecentlyPicked,
		CurrentRoster,

		// Maybe recently picked by position.
		/*RecentQBs,
		RecentRBs,
		RecentWRs,
		RecentTEs,
		RecentKs,
		RecentDEFs*/
	}

	enum DisplayState
	{
		HideState,
		AnimateIn,
		ShowState,
		AnimateOut
	}

	// Nameplate objects
	public GameObject headerObject;

	public GameObject[] dataBackdrops;
	public Sprite[] teamBackgroundSprites;

	private readonly float showXPosition = 6.7f;
	private readonly float hideXPosition = 11.25f;

	private DraftTimerScript timerScript;
	private bool recentlyPickedRunning = false;
	private bool alreadyPickedPaused = false;

	private int currentAnimatedBackdrop = -1;

	private DisplayState currentDisplayState = DisplayState.HideState;
	private RecentlyPickedOrder currentDisplayOption = RecentlyPickedOrder.RecentlyPicked;

	private float currentTimer = 0;
	public float nextBackdropDelay = 0.25f;
	public float maxShowTime = 5;

	private List<PlayerDatabase.PlayerData> pickHistory = new List<PlayerDatabase.PlayerData>();

	private BreakingNewsScript breakingNews;

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		breakingNews = GameObject.Find("BreakingNews").GetComponent<BreakingNewsScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Only progress if we're running
		if (recentlyPickedRunning)
		{
			switch (currentDisplayState)
			{
				case DisplayState.HideState:
					// Switch the names around for the next display state
					this.SetRecentlyPickedNames();
					currentDisplayState = DisplayState.AnimateIn;
					currentTimer = 0;
					break;
				case DisplayState.AnimateIn:
					this.AnimateInBackdrops();
					break;
				case DisplayState.ShowState:
// 					if (!alreadyPickedPaused)
// 					{
// 						currentTimer += Time.deltaTime;
// 						if (currentTimer >= maxShowTime)
// 						{
// 							currentDisplayState = DisplayState.AnimateOut;
// 							currentTimer = 0;
// 						}
// 					}
					break;
				case DisplayState.AnimateOut:
					this.AnimateOutBackdrop();
					break;
			}
		}
	}

	public void TogglePause()
	{
		alreadyPickedPaused = !alreadyPickedPaused;
	}

	public void CycleRecentlyPickedList()
	{
		if (this.currentDisplayState == DisplayState.ShowState)
		{
			this.currentDisplayState = DisplayState.AnimateOut;
			this.currentTimer = 0;
		}
	}

	public void AddPickToHistory(PlayerDatabase.PlayerData playerData, int currentRound, int currentPick)
	{
		// Skip pick grades for defenses and kickers (they don't work well)
		if (playerData.position != PlayerDatabase.Position.DEF && playerData.position != PlayerDatabase.Position.K)
		{
			// Check pick grade
			int expectedPlayerRank = (currentRound + 3) * (int)DrafterEnum.TotalDrafters + currentPick + 1;

			int pickGrade = expectedPlayerRank - (int)playerData.overallRank;
			// Draft Steal!
			if (pickGrade > 30)
			{
				this.breakingNews.AddNewsToTicker(timerScript.GetCurrentDrafter(), playerData.playerName, true);
				print("Draft Steal!: " + playerData.playerName + " : " + pickGrade);
			}
			// Draft Reach!
			else if (pickGrade < -24)
			{
				this.breakingNews.AddNewsToTicker(timerScript.GetCurrentDrafter(), playerData.playerName, false);
				print("Reach Pick: " + playerData.playerName + " : " + pickGrade);
			}
		}
		
		this.pickHistory.Insert(0, playerData);
		
		// Keep history to max of 5 names.
		if (this.pickHistory.Count > 5)
		{
			this.pickHistory.RemoveAt(this.pickHistory.Count - 1);
		}

		this.currentDisplayState = DisplayState.AnimateOut;
		this.currentTimer = 0;
	}

	private void SetRecentlyPickedNames()
	{
		// Go to the next display option, and loop around if needed
// 		currentDisplayOption += 1;
// 		if (currentDisplayOption > RecentlyPickedOrder.CurrentRoster)
// 		{
// 			currentDisplayOption = RecentlyPickedOrder.RecentlyPicked;
// 		}

		// Best available overall
		List<PlayerDatabase.PlayerData> currentPlayerList = new List<PlayerDatabase.PlayerData>();
		switch (currentDisplayOption)
		{
			case RecentlyPickedOrder.RecentlyPicked:
				headerObject.GetComponentInChildren<TextMeshPro>().text = "Recently Picked";
				currentPlayerList = this.pickHistory;
				break;
			case RecentlyPickedOrder.CurrentRoster:
				headerObject.GetComponentInChildren<TextMeshPro>().text = "Current Roster";
				currentPlayerList = this.GetCurrentRoster();
				break;
		}
			
		// Assign players to correct location.
		for (int i = 0; i < 5; ++i)
		{
			GameObject backdrop;

			if (i >= currentPlayerList.Count || currentPlayerList[i] == null)
			{
				backdrop = dataBackdrops[i];
				backdrop.GetComponentInChildren<TextMeshPro>().text = string.Empty;
				backdrop.GetComponent<SpriteRenderer>().sprite = teamBackgroundSprites[(int)PlayerDatabase.NFLTeam.FA];
				continue;
			}

			backdrop = dataBackdrops[i];
			backdrop.GetComponentInChildren<TextMeshPro>().text = currentPlayerList[i].playerName;
			backdrop.GetComponent<SpriteRenderer>().sprite = teamBackgroundSprites[(int)currentPlayerList[i].nflTeam];
		}
	}

	private void AnimateInBackdrops()
	{
		currentTimer += Time.deltaTime;

		// Backdrops to animate in
		if (currentAnimatedBackdrop < 5)
		{
			switch (currentAnimatedBackdrop)
			{
				// Header, just animate it right away
				case -1:
					headerObject.transform.DOMoveX(showXPosition, timerScript.quickAnimationTime);
					++currentAnimatedBackdrop;
					break;
				default:
					if (currentTimer > nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
					{
						if (dataBackdrops[currentAnimatedBackdrop].GetComponentInChildren<TextMeshPro>().text != string.Empty)
						{
							dataBackdrops[currentAnimatedBackdrop].transform.DOMoveX(showXPosition, timerScript.quickAnimationTime);
						}
						++currentAnimatedBackdrop;
					}
					break;
			}
		}
		// Done starting animation just waiting for time to be done
		else
		{
			if (currentTimer > timerScript.quickAnimationTime + nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
			{
				currentAnimatedBackdrop = -1;
				currentTimer = 0;
				currentDisplayState = DisplayState.ShowState;
			}
		}
	}

	private void AnimateOutBackdrop()
	{
		currentTimer += Time.deltaTime;

		// Backdrops to animate in
		if (currentAnimatedBackdrop < 5)
		{
			switch (currentAnimatedBackdrop)
			{
				// Header, just animate it right away
				case -1:
					headerObject.transform.DOMoveX(hideXPosition, timerScript.quickAnimationTime);
					++currentAnimatedBackdrop;
					break;
				default:
					if (currentTimer > nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
					{
						dataBackdrops[currentAnimatedBackdrop].transform.DOMoveX(hideXPosition, timerScript.quickAnimationTime);
						++currentAnimatedBackdrop;
					}
					break;
			}
		}
		// Done starting animation just waiting for time to be done
		else
		{
			if (currentTimer > timerScript.quickAnimationTime + nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
			{
				currentAnimatedBackdrop = -1;
				currentTimer = 0;
				currentDisplayState = DisplayState.HideState;
			}
		}
	}

	public void SetAlreadyPickedControllerRunning(bool running)
	{
		this.recentlyPickedRunning = running;

		// Set to not running, make sure to hide everything
		if (!recentlyPickedRunning)
		{
			headerObject.transform.DOKill();
			headerObject.transform.DOMoveX(hideXPosition, timerScript.quickAnimationTime);

			foreach (GameObject backdrop in dataBackdrops)
			{
				backdrop.transform.DOKill();
				backdrop.transform.DOMoveX(hideXPosition, timerScript.quickAnimationTime);
			}
		}
	}

	private List<PlayerDatabase.PlayerData> GetCurrentRoster()
	{
		// Get the current drafter.
		DrafterEnum currentDrafter = timerScript.GetCurrentDrafter();

		List<PlayerDatabase.PlayerData> rosterList = new List<PlayerDatabase.PlayerData>();

		var allPlayerList = timerScript.playerProfiles[(int)currentDrafter].allPlayerPicks;

		// Check if there are players in the all players list.
		if (allPlayerList.Count > 0)
		{
			// Loop through the player list and add them all to the roster list.
			for (int i = allPlayerList.Count - 1; i >= Mathf.Max(0, allPlayerList.Count - 5); --i)
			{
				rosterList.Add(allPlayerList[i]);
			}
		}
		else
		{
			// Grab all contract players.
			rosterList.AddRange(timerScript.playerProfiles[(int)currentDrafter].oneYearContracts);
			rosterList.AddRange(timerScript.playerProfiles[(int)currentDrafter].twoYearContracts);
			rosterList.Add(timerScript.playerProfiles[(int)currentDrafter].threeYearContract);

			while (rosterList.Count > 5)
			{
				rosterList.RemoveAt(rosterList.Count - 1);
			}
		}

		return rosterList;
	}
}
