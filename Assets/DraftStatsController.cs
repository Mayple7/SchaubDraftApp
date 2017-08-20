using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DraftStatsController : MonoBehaviour
{
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

	private List<StatsStrings> stringsList;

	// The stats strings to display 
	public struct StatsStrings
	{
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
		
	}

	public void StartDisplayingDraftStats()
	{
		Dictionary<StatsMessage, string>  stats = statsAccumulation();
		foreach (StatsMessage key in stats.Keys)
		{
			String output;
			stats.TryGetValue(key, out output);
			print(key.ToString() + " " + output);
		}
	}

	public Dictionary<StatsMessage, string> statsAccumulation()
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
			if (mostBonusTimeUsed == null || profile.bonusTimeRemaining > mostBonusTimeUsed.bonusTimeRemaining)
			{
				mostBonusTimeUsed = profile;
			}

			// Update least bonus time used
			if (leastBonusTimeUsed == null || profile.bonusTimeRemaining < leastBonusTimeUsed.bonusTimeRemaining)
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
		profiles = calculateDraftGrades(profiles);

		//define final Stats
		var stats = new Dictionary<StatsMessage, string>();
		stats.Add(StatsMessage.DraftGrades, formatDraftGradesOutput(profiles));
		stats.Add(StatsMessage.BestPick, "The best pick was " + bestPick.playerPicked.playerName + " by " + bestPick.drafterID.ToString());
		stats.Add(StatsMessage.WorstPick, "The worst pick was " + worstPick.playerPicked.playerName + " by " + worstPick.drafterID.ToString());
		stats.Add(StatsMessage.LeastBonusTimeUsed, leastBonusTimeUsed.playerName + " used the least bonus time with " + leastBonusTimeUsed.bonusTimeRemaining +" remaining.");
		stats.Add(StatsMessage.MostBonusTimeUsed, mostBonusTimeUsed.playerName + " used the most bonus time with " + mostBonusTimeUsed.bonusTimeRemaining + " remaining.");
		stats.Add(StatsMessage.LongestDrafter, longestDrafter.playerName + " took the longest to draft at " + longestDrafter.totalTimeUsed + " seconds used.");
		stats.Add(StatsMessage.QuickestDrafter, quickestDrafter.playerName + " took the shortest to draft at " + quickestDrafter.totalTimeUsed + " seconds used.");
		stats.Add(StatsMessage.MostDiverseTeam, String.Format("{0} has the most diverse team", mostDiverseTeam[0].playerName));
		stats.Add(StatsMessage.LeastDiverseTeam, String.Format("{0} has the least diverse team", leastDiverseTeam[0].playerName));
		stats.Add(StatsMessage.MostPicksFromOneTeam, String.Format("{0} has the most picks from one team with {1} players on the same team", mostPicksFromOneTeam[0].playerName, mostPicksFromOneTeam[0].mostPlayersOnOneTeam));
		stats.Add(StatsMessage.MostPlayersOnSameByeWeek, String.Format("{0} has the most players on a single bye week: {1}", mostPlayersOnSameByeWeek[0].playerName, mostPlayersOnSameByeWeek[0].mostPlayersOnSingleByeWeek));
		return stats;
	}
	private string formatDraftGradesOutput(List<PlayerProfile> profiles)
	{
			String returnVal = "Draft Grades: ";
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

	private List<PlayerProfile> calculateDraftGrades(List<PlayerProfile> profiles)
	{
		profiles.Sort((x, y) => x.draftGradeNumber.CompareTo(y.draftGradeNumber));
		string[] grades = { "A+", "A", "A-", "B+", "B", "B-", "C+","C","C-","D+","D","F" };
		for(int i = 0; i < profiles.Count; i++)
		{
				profiles[i].draftGrade = grades[i];
		}
		return profiles;   
	}
}
