using System.Collections;
using System.Collections.Generic;
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

	}
}
