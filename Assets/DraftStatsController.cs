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

	// The stats strings to display 
	private string topStatsString;
	private string botStatsString;

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
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
        PlayerProfile longestDrafter = null;
        PlayerProfile quickestDrafter = null;
        PlayerProfile mostBonusTimeUsed = null;
        PlayerProfile leastBonusTimeUsed = null;
        PlayerProfile mostPlayersOnSameByeWeek = null;
        PlayerProfile leastPlayersOnSameByeWeek = null;
        PlayerProfile mostDiverseTeam = null;
        PlayerProfile leastDiverseTeam = null;
        PlayerProfile mostPicksFromOneTeam = null;
        List<PlayerProfile> profiles = new List<PlayerProfile>();

        foreach (PlayerProfile profile in timerScript.playerProfiles)
        {
            profile.averageTimeUsed = profile.totalTimeUsed / timerScript.totalRounds;
            if(longestDrafter == null || profile.totalTimeUsed > longestDrafter.totalTimeUsed)
            {
                longestDrafter = profile;
            }
            if (quickestDrafter == null || profile.totalTimeUsed < quickestDrafter.totalTimeUsed)
            {
                quickestDrafter = profile;
            }
            if (mostBonusTimeUsed == null || profile.bonusTimeRemaining > mostBonusTimeUsed.bonusTimeRemaining)
            {
                mostBonusTimeUsed = profile;
            }
            if (leastBonusTimeUsed == null || profile.bonusTimeRemaining < leastBonusTimeUsed.bonusTimeRemaining)
            {
                leastBonusTimeUsed = profile;
            }
            PlayerProfile byeWeekProfile = calculateMaxPlayersOnSingleByeWeek(profile);
            profile.leastPlayersOnSingleByeWeek = byeWeekProfile.leastPlayersOnSingleByeWeek;
            profile.mostPlayersOnSingleByeWeek = byeWeekProfile.mostPlayersOnSingleByeWeek;
            if (mostPlayersOnSameByeWeek == null || profile.mostPlayersOnSingleByeWeek > mostPlayersOnSameByeWeek.mostPlayersOnSingleByeWeek)
            {
                mostPlayersOnSameByeWeek = profile;
            }
            if (leastPlayersOnSameByeWeek == null || profile.leastPlayersOnSingleByeWeek < leastPlayersOnSameByeWeek.leastPlayersOnSingleByeWeek)
            {
                leastPlayersOnSameByeWeek = profile;
            }
            PlayerProfile diversityProfile = calculateTeamDiversity(profile);
            profile.diversity = diversityProfile.diversity;
            profile.mostPlayersOnOneTeam = diversityProfile.mostPlayersOnOneTeam;
            if (mostDiverseTeam == null || profile.diversity < mostDiverseTeam.diversity)
            {
                mostDiverseTeam = profile;
            }
            if (leastDiverseTeam == null || profile.diversity > leastDiverseTeam.diversity)
            {
                leastDiverseTeam = profile;
            }
            if (mostPicksFromOneTeam == null || profile.mostPlayersOnOneTeam > mostPicksFromOneTeam.mostPlayersOnOneTeam)
            {
                mostPicksFromOneTeam = profile;
            }
            PlayerProfile draftGradeProfile = calculateDraftGrade(profile);
            profile.draftGradeNumber = draftGradeProfile.draftGradeNumber;
            profiles.Add(profile);
        }
        PickInfo bestPick = null;
        PickInfo worstPick = null;
        long bestValue = 0;
        long worstValue = 0;
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
                }else if (value < worstValue)
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
        stats.Add(StatsMessage.BestPick, "The best pick was " + bestPick.playerPicked.playerName + " by " + bestPick.drafterID.ToString());
        stats.Add(StatsMessage.WorstPick, "The worst pick was " + worstPick.playerPicked.playerName + " by " + worstPick.drafterID.ToString());
        stats.Add(StatsMessage.LeastBonusTimeUsed, leastBonusTimeUsed.playerName + " used the least bonus time with " + leastBonusTimeUsed.bonusTimeRemaining +" remaining.");
        stats.Add(StatsMessage.MostBonusTimeUsed, mostBonusTimeUsed.playerName + " used the most bonus time with " + mostBonusTimeUsed.bonusTimeRemaining + " remaining.");
        stats.Add(StatsMessage.LongestDrafter, longestDrafter.playerName + " took the longest to draft at " + longestDrafter.totalTimeUsed + " seconds used.");
        stats.Add(StatsMessage.QuickestDrafter, quickestDrafter.playerName + " took the shortest to draft at " + quickestDrafter.totalTimeUsed + " seconds used.");
        stats.Add(StatsMessage.MostDiverseTeam, String.Format("{0} has the most diverse team", mostDiverseTeam.playerName));
        stats.Add(StatsMessage.LeastDiverseTeam, String.Format("{0} has the least diverse team", leastDiverseTeam.playerName));
        stats.Add(StatsMessage.MostPicksFromOneTeam, String.Format("{0} has the most picks from one team with {1} players on the same team", mostPicksFromOneTeam.playerName, mostPicksFromOneTeam.mostPlayersOnOneTeam));
        stats.Add(StatsMessage.MostPlayersOnSameByeWeek, String.Format("{0} has the most players on a single bye week: {1}", mostPlayersOnSameByeWeek.playerName, mostPlayersOnSameByeWeek.mostPlayersOnSingleByeWeek));
        stats.Add(StatsMessage.LeastPlayersOnSameByeWeek, String.Format("{0} has the least players on a single bye week: {1}", leastPlayersOnSameByeWeek.playerName, leastPlayersOnSameByeWeek.mostPlayersOnSingleByeWeek));
        stats.Add(StatsMessage.DraftGrades, formatDraftGradesOutput(profiles));
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
        
    private PlayerProfile calculateMaxPlayersOnSingleByeWeek(PlayerProfile profile)
    {
        var byWeeks = new Dictionary<uint, int>();
     foreach(PlayerDatabase.PlayerData data in profile.allPlayerPicks)
        {
            //loop over all bye weeks for each player drafted and increment a counter for each week they have a player on by week
            if (byWeeks.ContainsKey(data.byeWeek))                
            {
                int temp;
                byWeeks.TryGetValue(data.byeWeek, out temp);
                byWeeks.Add(data.byeWeek, ++temp);
            }
            else
            {
                byWeeks.Add(data.byeWeek, 1);
            }
        }
        int maxOnBye = 0;
        //set it higher than possible
        int minOnBye = 100;
        foreach(int playersOnBye in byWeeks.Values){
            if(maxOnBye < playersOnBye)
            {
                maxOnBye = playersOnBye;
            }
            if(playersOnBye < minOnBye)
            {
                minOnBye = playersOnBye;
            }
        }
        profile.mostPlayersOnSingleByeWeek = maxOnBye;
        profile.leastPlayersOnSingleByeWeek = minOnBye;
        return profile;
    }

    private PlayerProfile calculateTeamDiversity(PlayerProfile profile)
    {
        var teamComp = new Dictionary<PlayerDatabase.NFLTeam, int>();
        foreach (PlayerDatabase.PlayerData data in profile.allPlayerPicks)
        {
            if (teamComp.ContainsKey(data.nflTeam))
            {
                int temp;
                teamComp.TryGetValue(data.nflTeam, out temp);
                teamComp.Add(data.nflTeam, ++temp);
            }
            else
            {
                teamComp.Add(data.nflTeam, 1);
            }
        }
        int mostPicksOnSameTeam = 0;
         foreach (int numOnSameTeam in teamComp.Values)
         {
            if(numOnSameTeam > mostPicksOnSameTeam)
            {
                mostPicksOnSameTeam = numOnSameTeam;
            }
         }
        profile.mostPlayersOnOneTeam = mostPicksOnSameTeam;
        //the total rounds (total number of players) / how many unique teams the player has (ex: 12 players on 6 teams = diversity of 2)
        float diversity =  timerScript.totalRounds / teamComp.Keys.Count;
        profile.diversity = diversity;
        return profile;
    }
    private PlayerProfile calculateDraftGrade(PlayerProfile profile)
    {
        //lower = better
        uint overallValue = 0;
        foreach (PlayerDatabase.PlayerData data in profile.allPlayerPicks)
        {
            overallValue += data.overallRank;
        }
        profile.draftGradeNumber = overallValue;
        return profile;
    }
    private List<PlayerProfile> calculateDraftGrades(List<PlayerProfile> profiles)
    {
        profiles.Sort((x, y) => x.draftGradeNumber.CompareTo(y.draftGradeNumber));
        string[] grades = { "A+", "A", "A-", "B+", "B", "B-", "C+","C","C-","D+","D","F" };
        for(int i = 0; i < profiles.Count; i++)
        {
            profiles[i].draftGrade = grades[0];
        }
        return profiles;   
    }
}
