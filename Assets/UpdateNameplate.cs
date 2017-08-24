using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpdateNameplate : MonoBehaviour
{
	enum PositionImportance
	{
		RB,
		WR,
		QB,
		Flex1,	// RB,WR
		TE,
		Flex2,	// RB,WR,TE
		DEF,
		RBBackup,
		WRBackup,
		QBBackup,
		FlexBackup, // RB,WR,TE
		K,
		TotalRosterSlots
	}

	// Reference to the main script
	private DraftTimerScript timerScript;

	// This object's text mesh component (we reference this a lot)
	private TextMesh textMesh;

	// This nameplate's drafterID
	private DrafterEnum drafterID = DrafterEnum.Dan;

	// PositionInfos
	public GameObject[] positionTags;

	public float showYPosition = 0;
	public float hideYPosition = 0;

	public Sprite MissingStarterSlot;
	public Sprite MissingBackupSlot;

	private int[] rosterSlots;

	// Use this for initialization
	void Start ()
	{
		textMesh = this.GetComponentInChildren<TextMesh>();
		rosterSlots = new int[(int)PositionImportance.TotalRosterSlots];
		showYPosition -= hideYPosition;
	}

	// Initialize this nameplate for the given drafter
	public void InitializeVariables(DrafterEnum drafter)
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		drafterID = drafter;
		this.GetComponent<SpriteRenderer>().sprite = timerScript.nameplateSprites[(int)drafterID];
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Get the amount of bonus time remaining for the drafter
		float bonusTime = timerScript.playerProfiles[(int)drafterID].bonusTimeRemaining;

		// Format time into human readable time
		string minutes = Mathf.Floor(bonusTime / 60).ToString();
		string seconds = Mathf.FloorToInt(bonusTime % 60).ToString("00");

		// Set the text mesh 
		textMesh.text = minutes + ":" + seconds;
	}

	// Show what positions are needed
	public void ShowPositionNeeds()
	{
		int positionTagIndex = 0;

		// Puts contract data in (it has not been updated to the all player list
		if(!timerScript.playerProfiles[(int)drafterID].contractDataWritten)
		{
			// Add 3-year contract
			AddPositionToRoster(timerScript.playerProfiles[(int)drafterID].threeYearContract.position);

			// Add 2-year contracts
			foreach (PlayerDatabase.PlayerData contract in timerScript.playerProfiles[(int)drafterID].twoYearContracts)
			{
				AddPositionToRoster(contract.position);
			}

			// Add 1-year contracts
			foreach (PlayerDatabase.PlayerData contract in timerScript.playerProfiles[(int)drafterID].oneYearContracts)
			{
				AddPositionToRoster(contract.position);
			}
		}
		else
		{
			// Add all other players
			foreach (PlayerDatabase.PlayerData contract in timerScript.playerProfiles[(int)drafterID].allPlayerPicks)
			{
				AddPositionToRoster(contract.position);
			}
		}

		// Update the position tags and move them
		for(int i = 0; i < positionTags.Length; ++i)
		{
			float delayAmount = 0.1f * i;

			// No RBs on roster
			if(positionTagIndex <= (int)PositionImportance.RB && rosterSlots[(int)PositionImportance.RB] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "RB";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.RB + 1;
				continue;
			}

			// 0 or 1 WRs on roster
			if(positionTagIndex <= (int)PositionImportance.WR && rosterSlots[(int)PositionImportance.WR] <= 1)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "WR";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.WR + 1;
				continue;
			}

			// No QBs on roster
			if(positionTagIndex <= (int)PositionImportance.QB && rosterSlots[(int)PositionImportance.QB] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "QB";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.QB + 1;
				continue;
			}

			// No Flex1 starters on roster
			if (positionTagIndex <= (int)PositionImportance.Flex1 && rosterSlots[(int)PositionImportance.Flex1] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "Flex";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.Flex1 + 1;
				continue;
			}

			// No TEs on roster
			if (positionTagIndex <= (int)PositionImportance.TE && rosterSlots[(int)PositionImportance.TE] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "TE";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.TE + 1;
				continue;
			}

			// No Flex2 starters on roster
			if (positionTagIndex <= (int)PositionImportance.Flex2 && rosterSlots[(int)PositionImportance.Flex2] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "Flex";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.Flex2 + 1;
				continue;
			}

			// No DEF starters on roster
			if (positionTagIndex <= (int)PositionImportance.DEF && rosterSlots[(int)PositionImportance.DEF] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "DEF";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.DEF + 1;
				continue;
			}

			// No RB backups on roster
			if (positionTagIndex <= (int)PositionImportance.RBBackup && rosterSlots[(int)PositionImportance.RBBackup] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingBackupSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "RB";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.RBBackup + 1;
				continue;
			}

			// No WR backups on roster
			if (positionTagIndex <= (int)PositionImportance.WRBackup && rosterSlots[(int)PositionImportance.WRBackup] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingBackupSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "WR";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.WRBackup + 1;
				continue;
			}

			// No QB backups on roster
			if (positionTagIndex <= (int)PositionImportance.QBBackup && rosterSlots[(int)PositionImportance.QBBackup] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingBackupSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "QB";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.QBBackup + 1;
				continue;
			}

			// No Flex backups on roster
			if (positionTagIndex <= (int)PositionImportance.FlexBackup && rosterSlots[(int)PositionImportance.FlexBackup] <= 1)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingBackupSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "Flex";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.FlexBackup + 1;
				continue;
			}
			// No K on roster
			if (positionTagIndex <= (int)PositionImportance.K && rosterSlots[(int)PositionImportance.K] == 0)
			{
				positionTags[i].GetComponent<SpriteRenderer>().sprite = MissingStarterSlot;
				positionTags[i].GetComponentInChildren<TextMesh>().text = "K";
				positionTags[i].transform.DOMoveY(showYPosition, timerScript.quickAnimationTime).SetDelay(delayAmount);
				positionTagIndex = (int)PositionImportance.K + 1;
				continue;
			}
		}

		// Set the autopick button to use this player
		SetAutoPickPlayer();
	}

	private void SetAutoPickPlayer()
	{
		string autopickPlayer = string.Empty;

		// No RBs on roster
		if (rosterSlots[(int)PositionImportance.RB] == 0)
		{
			// Get the top RB
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB)[0].playerName;
		}
		// 0 or 1 WRs on roster
		else if (rosterSlots[(int)PositionImportance.WR] <= 1)
		{
			// Get the top WR
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR)[0].playerName;
		}
		// No QBs on roster
		else if (rosterSlots[(int)PositionImportance.QB] == 0)
		{
			// Get the top QB
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.QB)[0].playerName;
		}
		// No Flex1 starters on roster
		else if (rosterSlots[(int)PositionImportance.Flex1] == 0)
		{
			// Get the top flex
			PlayerDatabase.PlayerData WR = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR)[0];
			PlayerDatabase.PlayerData RB = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB)[0];

			// WR is better ranked
			if(WR.overallRank < RB.overallRank)
			{
				autopickPlayer = WR.playerName;
			}
			else
			{
				autopickPlayer = RB.playerName;
			}
		}
		// No TEs on roster
		else if (rosterSlots[(int)PositionImportance.TE] == 0)
		{
			// Get the top TE
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.TE)[0].playerName;
		}
		// No Flex2 starters on roster
		else if (rosterSlots[(int)PositionImportance.Flex2] == 0)
		{
			// Get the top flex
			PlayerDatabase.PlayerData WR = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR)[0];
			PlayerDatabase.PlayerData RB = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB)[0];
			PlayerDatabase.PlayerData TE = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.TE)[0];

			// WR is better ranked
			if (WR.overallRank < RB.overallRank && WR.overallRank < TE.overallRank)
			{
				autopickPlayer = WR.playerName;
			}
			// RB is better ranked
			else if(RB.overallRank < WR.overallRank && RB.overallRank < TE.overallRank)
			{
				autopickPlayer = RB.playerName;
			}
			// TE is better ranked
			else
			{
				autopickPlayer = TE.playerName;
			}
		}
		// No DEF starters on roster
		else if (rosterSlots[(int)PositionImportance.DEF] == 0)
		{
			// Get the top DEF
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.DEF)[0].playerName;
		}
		// No RB backups on roster
		else if (rosterSlots[(int)PositionImportance.RBBackup] == 0)
		{
			// Get the top RB
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB)[0].playerName;
		}
		// No WR backups on roster
		else if (rosterSlots[(int)PositionImportance.WRBackup] == 0)
		{
			// Get the top WR
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR)[0].playerName;
		}
		// No QB backups on roster
		else if (rosterSlots[(int)PositionImportance.QBBackup] == 0)
		{
			// Get the top QB
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.QB)[0].playerName;
		}
		// No Flex backups on roster
		else if (rosterSlots[(int)PositionImportance.FlexBackup] <= 1)
		{
			// Get the top flex
			PlayerDatabase.PlayerData WR = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR)[0];
			PlayerDatabase.PlayerData RB = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB)[0];
			PlayerDatabase.PlayerData TE = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.TE)[0];

			// WR is better ranked
			if (WR.overallRank < RB.overallRank && WR.overallRank < TE.overallRank)
			{
				autopickPlayer = WR.playerName;
			}
			// RB is better ranked
			else if (RB.overallRank < WR.overallRank && RB.overallRank < TE.overallRank)
			{
				autopickPlayer = RB.playerName;
			}
			// TE is better ranked
			else
			{
				autopickPlayer = TE.playerName;
			}
		}
		// No K on roster
		else if (rosterSlots[(int)PositionImportance.K] == 0)
		{
			// Get the top K
			autopickPlayer = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.K)[0].playerName;
		}
		else
		{
			// Get the top player
			autopickPlayer = timerScript.playerDatabase.GetBestAvailablePlayers()[0].playerName;
		}

		GameObject.Find("AutoPickButton").GetComponent<AutoPickButton>().SetAutoPickPlayer(autopickPlayer);
	}

	private void AddPositionToRoster(PlayerDatabase.Position playerPosition)
	{
		switch (playerPosition)
		{
			// QB player
			case PlayerDatabase.Position.QB:
				if(rosterSlots[(int)PositionImportance.QB] == 0)
				{
					++rosterSlots[(int)PositionImportance.QB];
				}
				else
				{
					++rosterSlots[(int)PositionImportance.QBBackup];
				}
				break;

			case PlayerDatabase.Position.RB:
				if(rosterSlots[(int)PositionImportance.RB] == 0)
				{
					++rosterSlots[(int)PositionImportance.RB];
				}
				else if(rosterSlots[(int)PositionImportance.Flex1] == 0)
				{
					++rosterSlots[(int)PositionImportance.Flex1];
				}
				else if(rosterSlots[(int)PositionImportance.Flex2] == 0)
				{
					++rosterSlots[(int)PositionImportance.Flex2];
				}
				else if(rosterSlots[(int)PositionImportance.RBBackup] == 0)
				{
					++rosterSlots[(int)PositionImportance.RBBackup];
				}
				else
				{
					++rosterSlots[(int)PositionImportance.FlexBackup];
				}
				break;
			case PlayerDatabase.Position.WR:
				if(rosterSlots[(int)PositionImportance.WR] <= 1)
				{
					++rosterSlots[(int)PositionImportance.WR];
				}
				else if(rosterSlots[(int)PositionImportance.Flex1] == 0)
				{
					++rosterSlots[(int)PositionImportance.Flex1];
				}
				else if(rosterSlots[(int)PositionImportance.Flex2] == 0)
				{
					++rosterSlots[(int)PositionImportance.Flex2];
				}
				else if(rosterSlots[(int)PositionImportance.WRBackup] == 0)
				{
					++rosterSlots[(int)PositionImportance.WRBackup];
				}
				else
				{
					++rosterSlots[(int)PositionImportance.FlexBackup];
				}
				break;
			case PlayerDatabase.Position.TE:
				if(rosterSlots[(int)PositionImportance.TE] == 0)
				{
					++rosterSlots[(int)PositionImportance.TE];
				}
				else if(rosterSlots[(int)PositionImportance.Flex2] == 0)
				{
					++rosterSlots[(int)PositionImportance.Flex2];
				}
				else
				{
					++rosterSlots[(int)PositionImportance.FlexBackup];
				}
				break;
			case PlayerDatabase.Position.K:
				++rosterSlots[(int)PositionImportance.K];
				break;
			case PlayerDatabase.Position.DEF:
				++rosterSlots[(int)PositionImportance.DEF];
				break;
		}
	}
}
