using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DraftStatsController : MonoBehaviour
{
	enum ShowStatsState
	{
		AnimateInFast,	// At the start of this state set the draft message
		AnimateInSlow,
		AnimateOut
	}
	private ShowStatsState currentStatsState = ShowStatsState.AnimateInFast;

	public enum StatsMessage
	{
		// Grade all the teams against each other
		DraftGrades,

		// Picks based off expected draft location
		BestPick,
		WorstPick,

		// NFL teams stats
		MostDiverseTeam,
		LeastDiverseTeam,
		MostPicksFromOneTeam,

		// Draft time stats
		LongestDrafter,
		QuickestDrafter,
		MostBonusTimeUsed,
		LeastBonusTimeUsed,

		// Bye week stats
		MostPlayersOnSameByeWeek,
		LeastPlayersOnSameByeWeek,

		TotalStatsSubjects		// Used to determine when to loop to the start again
	}

	public DraftTimerScript timerScript;
	private bool showStatsOnScreen = false;

	private List<StatsStrings> stringsList;
	private int currentStringsIndex = 0;

	public float topHideXValue;
	public float botHideXValue;

	public GameObject topStringObject;
	public GameObject botStringObject;

	private float currentTimer = 5;
	private float maxTime = 5;

	public float slowAnimationTime = 8;
	public float fastAnimationTime = 0.25f;

	// The stats strings to display 
	public class StatsStrings
	{
		public StatsStrings(string topString, string botString)
		{
			topStatsString = topString;
			botStatsString = botString;
		}
		public string topStatsString;
		public string botStatsString;
	}

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();

		stringsList = new List<StatsStrings>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(showStatsOnScreen)
		{
			switch(currentStatsState)
			{
				case ShowStatsState.AnimateInFast:
					// Wait for animate out to complete
					currentTimer += Time.deltaTime;

					if (currentTimer >= maxTime)
					{
						// Set the object texts
						topStringObject.GetComponent<TextMesh>().text = stringsList[currentStringsIndex].topStatsString;
						botStringObject.GetComponent<TextMesh>().text = stringsList[currentStringsIndex].botStatsString;
						++currentStringsIndex;

						// Reset the list index
						if (currentStringsIndex >= stringsList.Count)
						{
							currentStringsIndex = 0;
						}

						// Animate the objects
						if(topStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							topStringObject.transform.DOMoveX(topStringObject.GetComponent<Renderer>().bounds.extents.x * 2 + topHideXValue, fastAnimationTime).SetEase(Ease.Linear);
						}
						if(botStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							botStringObject.transform.DOMoveX(-botStringObject.GetComponent<Renderer>().bounds.extents.x * 2 + botHideXValue, fastAnimationTime).SetEase(Ease.Linear);
						}

						// Setup the timers and go to next state
						currentStatsState = ShowStatsState.AnimateInSlow;
						currentTimer = 0;
						maxTime = fastAnimationTime;
					}
					break;
				case ShowStatsState.AnimateInSlow:
					// Wait for animate in fast to complete
					currentTimer += Time.deltaTime;

					if(currentTimer >= maxTime)
					{
						// Animate the objects
						if (topStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							topStringObject.transform.DOMoveX(botHideXValue - 1, slowAnimationTime).SetEase(Ease.Linear);
						}
						if (botStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							botStringObject.transform.DOMoveX(topHideXValue + 1, slowAnimationTime).SetEase(Ease.Linear);
						}
						
						// Setup the timers and go to next state
						currentStatsState = ShowStatsState.AnimateOut;
						currentTimer = 0;
						maxTime = slowAnimationTime;
					}
					break;
				case ShowStatsState.AnimateOut:
					// Wait for animate in slow to complete
					currentTimer += Time.deltaTime;

					if (currentTimer >= maxTime)
					{
						// Animate the objects
						if (topStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							topStringObject.transform.DOMoveX(topHideXValue, fastAnimationTime).SetEase(Ease.Linear);
						}
						if (botStringObject.GetComponent<TextMesh>().text.Length > 0)
						{
							botStringObject.transform.DOMoveX(botHideXValue, fastAnimationTime).SetEase(Ease.Linear);
						}
						
						// Setup the timers and go to next state
						currentStatsState = ShowStatsState.AnimateInFast;
						currentTimer = 0;
						maxTime = fastAnimationTime + 1;
					}
					break;
			}
		}
	}

	public void StartDisplayingDraftStats()
	{
		CreateStatsStrings();
		foreach (StatsStrings strings in stringsList)
		{
			print(strings.topStatsString + " == " + strings.botStatsString);
		}

		SavedDraftData.SerializeDraftStats();
		SceneManager.LoadScene("Ending");

		showStatsOnScreen = true;
	}

	private void CreateStatsStrings()
	{
		// Time based stats - probably won't be ties
		PlayerProfile longestDrafter = null;
		PlayerProfile quickestDrafter = null;
		PlayerProfile mostBonusTimeUsed = null;
		PlayerProfile leastBonusTimeUsed = null;

		// Other stats that may have ties
		List<PlayerProfile> mostPlayersOnSameByeWeek = new List<PlayerProfile>();
		List<PlayerProfile> mostDiverseTeam = new List<PlayerProfile>();
		List<PlayerProfile> leastDiverseTeam = new List<PlayerProfile>();
		List<PlayerProfile> mostPicksFromOneTeam = new List<PlayerProfile>();

		List<PlayerProfile> profiles = new List<PlayerProfile>();

		foreach (PlayerProfile profile in timerScript.playerProfiles)
		{
			// Calculate the average time used per pick
			profile.averageTimeUsed = profile.totalTimeUsed / timerScript.totalRounds;

			// Update the longest drafter
			if(longestDrafter == null || profile.totalTimeUsed > longestDrafter.totalTimeUsed)
			{
				longestDrafter = profile;
			}

			// Update the quickest drafter
			if (quickestDrafter == null || profile.totalTimeUsed < quickestDrafter.totalTimeUsed)
			{
				quickestDrafter = profile;
			}

			// Update most bonus time used
			if (mostBonusTimeUsed == null || profile.bonusTimeRemaining < mostBonusTimeUsed.bonusTimeRemaining)
			{
				mostBonusTimeUsed = profile;
			}

			// Update least bonus time used
			if (leastBonusTimeUsed == null || profile.bonusTimeRemaining > leastBonusTimeUsed.bonusTimeRemaining)
			{
				leastBonusTimeUsed = profile;
			}

			// Update max players on a bye week
			CalculateMaxPlayersOnByeWeek(profile);
			if (mostPlayersOnSameByeWeek.Count == 0 || profile.mostPlayersOnSingleByeWeek > mostPlayersOnSameByeWeek[0].mostPlayersOnSingleByeWeek)
			{
				mostPlayersOnSameByeWeek.Clear();
				mostPlayersOnSameByeWeek.Add(profile);
			}
			else if(mostPlayersOnSameByeWeek.Count == 0 || profile.mostPlayersOnSingleByeWeek == mostPlayersOnSameByeWeek[0].mostPlayersOnSingleByeWeek)
			{
				mostPlayersOnSameByeWeek.Add(profile);
			}
			
			// Update team diversity
			CalculateTeamDiversity(profile);
			if (mostDiverseTeam.Count == 0 || profile.numTeamsDrafted > mostDiverseTeam[0].numTeamsDrafted)
			{
				mostDiverseTeam.Clear();
				mostDiverseTeam.Add(profile);
			}
			else if(mostDiverseTeam.Count == 0 || profile.numTeamsDrafted == mostDiverseTeam[0].numTeamsDrafted)
			{
				mostDiverseTeam.Add(profile);
			}

			if (leastDiverseTeam.Count == 0 || profile.numTeamsDrafted < leastDiverseTeam[0].numTeamsDrafted)
			{
				leastDiverseTeam.Clear();
				leastDiverseTeam.Add(profile);
			}
			else if(leastDiverseTeam.Count == 0 || profile.numTeamsDrafted == leastDiverseTeam[0].numTeamsDrafted)
			{
				leastDiverseTeam.Add(profile);
			}

			if (mostPicksFromOneTeam.Count == 0 || profile.mostPlayersOnOneTeam > mostPicksFromOneTeam[0].mostPlayersOnOneTeam)
			{
				mostPicksFromOneTeam.Clear();
				mostPicksFromOneTeam.Add(profile);
			}
			else if(mostPicksFromOneTeam.Count == 0 || profile.mostPlayersOnOneTeam == mostPicksFromOneTeam[0].mostPlayersOnOneTeam)
			{
				mostPicksFromOneTeam.Add(profile);
			}

			CalculateDraftGrade(profile);
			profiles.Add(profile);
		}

		PickInfo bestPick = null;
		PickInfo worstPick = null;
		long bestValue = -9000;
		long worstValue = 9000;
		int pickNumber = 1;
		foreach (List<PickInfo> pickList in timerScript.pickInfo)
		{
			foreach (PickInfo pick in pickList)
			{                
				// Skip if pick is null
				if(pick.playerPicked == null)
				{
					continue;
				}

				//larger negative = worse value, larger postive = greater value. 0 = at cost value;
				long value = pickNumber - pick.playerPicked.overallRank;
				if (value > bestValue)
				{
					bestPick = pick;
					bestValue = value;
				}

				if (value < worstValue)
				{
					worstPick = pick;
					worstValue = value;
				}
				++pickNumber;
			}
		}
		

		// Define final Stats
		// Draft quality stats
		stringsList.Add(new StatsStrings("Biggest Steal of Draft:", bestPick.playerPicked.playerName + " by " + timerScript.DrafterNames[(int)bestPick.drafterID]));
		SavedDraftData.statsDatas.Add(new StatsData(bestPick.drafterID, "Biggest Steal of Draft:", bestPick.playerPicked.playerName + " by " + timerScript.DrafterNames[(int)bestPick.drafterID]));
		stringsList.Add(new StatsStrings("Furthest Reach of Draft:", worstPick.playerPicked.playerName + " by " + timerScript.DrafterNames[(int)worstPick.drafterID]));
		SavedDraftData.statsDatas.Add(new StatsData(worstPick.drafterID, "Furthest Reach of Draft:", worstPick.playerPicked.playerName + " by " + timerScript.DrafterNames[(int)worstPick.drafterID]));

		// Time stats
		stringsList.Add(new StatsStrings("Least Bonus Time Used:", leastBonusTimeUsed.playerName + " with " + timerScript.FormatTimeText(leastBonusTimeUsed.bonusTimeRemaining) + " min Remaining."));
		SavedDraftData.statsDatas.Add(new StatsData(leastBonusTimeUsed.playerEnum, "Least Bonus Time Used:", leastBonusTimeUsed.playerName + " with " + timerScript.FormatTimeText(leastBonusTimeUsed.bonusTimeRemaining) + " min Remaining."));

		stringsList.Add(new StatsStrings("Most Bonus Time Used:", mostBonusTimeUsed.playerName + " with " + timerScript.FormatTimeText(mostBonusTimeUsed.bonusTimeRemaining) + " min Remaining."));
		SavedDraftData.statsDatas.Add(new StatsData(mostBonusTimeUsed.playerEnum, "Most Bonus Time Used:", mostBonusTimeUsed.playerName + " with " + timerScript.FormatTimeText(mostBonusTimeUsed.bonusTimeRemaining) + " min Remaining."));

		stringsList.Add(new StatsStrings("Quickest Drafter:", quickestDrafter.playerName + " in " + timerScript.FormatTimeText(quickestDrafter.totalTimeUsed) + " mins."));
		SavedDraftData.statsDatas.Add(new StatsData(quickestDrafter.playerEnum, "Quickest Drafter:", quickestDrafter.playerName + " in " + timerScript.FormatTimeText(quickestDrafter.totalTimeUsed) + " mins."));

		stringsList.Add(new StatsStrings("Longest Drafter:", longestDrafter.playerName + " in " + timerScript.FormatTimeText(longestDrafter.totalTimeUsed) + " mins."));
		SavedDraftData.statsDatas.Add(new StatsData(longestDrafter.playerEnum, "Longest Drafter:", longestDrafter.playerName + " in " + timerScript.FormatTimeText(longestDrafter.totalTimeUsed) + " mins."));

		// Draft diversity stats
		//AddDiverseStatsToStringList(mostDiverseTeam, leastDiverseTeam);
		//AddSingleTeamAndByeWeek(mostPicksFromOneTeam, mostPlayersOnSameByeWeek);

		CalculateDraftGrades(profiles);
	}
	private string formatDraftGradesOutput(List<PlayerProfile> profiles)
	{
			string returnVal = "Draft Grades: ";
			foreach (PlayerProfile profile in profiles)
			{
					returnVal += String.Format("{0} : {1}, ", profile.playerName, profile.draftGrade);
			}
			return returnVal;
	}

	private void CalculateMaxPlayersOnByeWeek(PlayerProfile profile)
	{
		// Create an array for bye weeks (max 16 weeks in a season)
		int[] byeWeeks = new int[16];

		foreach(PlayerDatabase.PlayerData data in profile.allPlayerPicks)
		{
			// Loop over all bye weeks for each player drafted and increment a counter for each week they have a player on by week
			++byeWeeks[data.byeWeek];
		}

		// Reset the most players on a bye week
		profile.mostPlayersOnSingleByeWeek = 0;
		// Loop through each bye week and select the max players
		foreach(int playersOnBye in byeWeeks)
		{
			if(profile.mostPlayersOnSingleByeWeek < playersOnBye)
			{
				profile.mostPlayersOnSingleByeWeek = playersOnBye;
			}
		}
	}

	private void CalculateTeamDiversity(PlayerProfile profile)
	{
		int[] teamComp = new int[(int)PlayerDatabase.NFLTeam.TotalNFLTeams];
		profile.mostPlayersOnOneTeam = 0;
		foreach (PlayerDatabase.PlayerData data in profile.allPlayerPicks)
		{
			// New team, increment the num teams counter
			if(teamComp[(int)data.nflTeam] == 0)
			{
				++profile.numTeamsDrafted;
			}

			// Incremeant the number of players from specific team
			++teamComp[(int)data.nflTeam];

			if(teamComp[(int)data.nflTeam] > profile.mostPlayersOnOneTeam)
			{
				profile.mostPlayersOnOneTeam = teamComp[(int)data.nflTeam];
			}
		}
	}

	private void CalculateDraftGrade(PlayerProfile profile)
	{
		// Lower is better
		profile.draftGradeNumber = 0;
		foreach (PlayerDatabase.PlayerData data in profile.allPlayerPicks)
		{
			profile.draftGradeNumber += data.overallRank;
		}
	}

	private void CalculateDraftGrades(List<PlayerProfile> profiles)
	{
		string[] grades = { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "F" };
		int gradesIndex = 0;

		profiles.Sort((x, y) => x.draftGradeNumber.CompareTo(y.draftGradeNumber));
		for(int i = 0; i < profiles.Count; i++)
		{
			// First draft grade or draft game is tied with previous person
			if (i == 0 || profiles[i].draftGradeNumber == profiles[i - 1].draftGradeNumber)
			{
				// Use draft grade, don't advance draft grade index
				profiles[i].draftGrade = grades[gradesIndex];
			}
			else
			{
				++gradesIndex;
				profiles[i].draftGrade = grades[gradesIndex];
			}
		}

		// Add draft grades to the stats string list
		stringsList.Add(new StatsStrings("Draft Grades:", string.Empty));
		
		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[0]), FormatDraftGradeString(profiles[1])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[11].playerEnum, "Draft Grade: " + profiles[11].draftGrade, profiles[11].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[10].playerEnum, "Draft Grade: " + profiles[10].draftGrade, profiles[10].playerName));

		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[2]), FormatDraftGradeString(profiles[3])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[9].playerEnum, "Draft Grade: " + profiles[9].draftGrade, profiles[9].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[8].playerEnum, "Draft Grade: " + profiles[8].draftGrade, profiles[8].playerName));

		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[4]), FormatDraftGradeString(profiles[5])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[7].playerEnum, "Draft Grade: " + profiles[7].draftGrade, profiles[7].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[6].playerEnum, "Draft Grade: " + profiles[6].draftGrade, profiles[6].playerName));

		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[6]), FormatDraftGradeString(profiles[7])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[5].playerEnum, "Draft Grade: " + profiles[5].draftGrade, profiles[5].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[4].playerEnum, "Draft Grade: " + profiles[4].draftGrade, profiles[4].playerName));

		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[8]), FormatDraftGradeString(profiles[9])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[3].playerEnum, "Draft Grade: " + profiles[3].draftGrade, profiles[3].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[2].playerEnum, "Draft Grade: " + profiles[2].draftGrade, profiles[2].playerName));

		stringsList.Add(new StatsStrings(FormatDraftGradeString(profiles[10]), FormatDraftGradeString(profiles[11])));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[1].playerEnum, "Draft Grade: " + profiles[1].draftGrade, profiles[1].playerName));
		SavedDraftData.statsDatas.Add(new StatsData(profiles[0].playerEnum, "Draft Grade: " + profiles[0].draftGrade, profiles[0].playerName));
	}

	private string FormatDraftGradeString(PlayerProfile profile)
	{
		return profile.playerName + ": " + profile.draftGrade;
	}

	private void AddDiverseStatsToStringList(List<PlayerProfile> mostDiverseTeam, List<PlayerProfile> leastDiverseTeam)
	{
		// Add diverse teams
		string diverseString = mostDiverseTeam[0].playerName;
		foreach(PlayerProfile profile in mostDiverseTeam)
		{
			// First profile in list
			if(profile == mostDiverseTeam[0])
			{
				diverseString = profile.playerName;
			}
			// Last drafter in list
			else if(profile == mostDiverseTeam[mostDiverseTeam.Count - 1])
			{
				diverseString += ", and " + profile.playerName;
			}
			else
			{
				diverseString += ", " + profile.playerName;
			}

			SavedDraftData.statsDatas.Add(new StatsData(profile.playerEnum, "Most Diverse Team With " + profile.numTeamsDrafted + " Teams On Roster:", profile.playerName));
		}

		stringsList.Add(new StatsStrings("Most Diverse Team With " + mostDiverseTeam[0].numTeamsDrafted + " Teams On Roster:", diverseString));

		diverseString = string.Empty;
		foreach (PlayerProfile profile in leastDiverseTeam)
		{
			// First profile in list
			if (profile == leastDiverseTeam[0])
			{
				diverseString = profile.playerName;
			}
			// Last drafter in list
			else if (profile == leastDiverseTeam[leastDiverseTeam.Count - 1])
			{
				diverseString += ", and " + profile.playerName;
			}
			else
			{
				diverseString += ", " + profile.playerName;
			}

			SavedDraftData.statsDatas.Add(new StatsData(profile.playerEnum, "Least Diverse Team With " + profile.numTeamsDrafted + " Teams On Roster:", profile.playerName));
		}

		stringsList.Add(new StatsStrings("Least Diverse Team With " + leastDiverseTeam[0].numTeamsDrafted + " Teams On Roster:", diverseString));
	}

	private void AddSingleTeamAndByeWeek(List<PlayerProfile> mostPlayersOnOneTeam, List<PlayerProfile> mostPlayersOnOneByeWeek)
	{
		// Add diverse teams
		string oneTeamString = string.Empty;
		foreach (PlayerProfile profile in mostPlayersOnOneTeam)
		{
			// First profile in list
			if (profile == mostPlayersOnOneTeam[0])
			{
				oneTeamString = profile.playerName;
			}
			// Last drafter in list
			else if (profile == mostPlayersOnOneTeam[mostPlayersOnOneTeam.Count - 1])
			{
				oneTeamString += ", and " + profile.playerName;
			}
			else
			{
				oneTeamString += ", " + profile.playerName;
			}

			SavedDraftData.statsDatas.Add(new StatsData(profile.playerEnum, "Most Players From One Team With " + profile.numTeamsDrafted + " Players:", profile.playerName));
		}

		stringsList.Add(new StatsStrings("Most Players From One Team With " + mostPlayersOnOneTeam[0].numTeamsDrafted + " Players:", oneTeamString));

		oneTeamString = string.Empty;
		foreach (PlayerProfile profile in mostPlayersOnOneByeWeek)
		{
			// First profile in list
			if (profile == mostPlayersOnOneByeWeek[0])
			{
				oneTeamString = profile.playerName;
			}
			// Last drafter in list
			else if (profile == mostPlayersOnOneByeWeek[mostPlayersOnOneByeWeek.Count - 1])
			{
				oneTeamString += ", and " + profile.playerName;
			}
			else
			{
				oneTeamString += ", " + profile.playerName;
			}

			SavedDraftData.statsDatas.Add(new StatsData(profile.playerEnum, "Most Players on One Bye Week With " + profile.mostPlayersOnSingleByeWeek + " Players:", profile.playerName));
		}

		stringsList.Add(new StatsStrings("Most Players on One Bye Week With " + mostPlayersOnOneByeWeek[0].mostPlayersOnSingleByeWeek + " Players:", oneTeamString));
	}
}
