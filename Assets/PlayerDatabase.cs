using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class PlayerDatabase
{
	public enum Position
	{
		QB,
		RB,
		WR,
		TE,
		K,
		DEF,
		NoPosition
	}

	public enum NFLTeam
	{
		// NFC
		MIN,
		GB,
		DET,
		CHI,

		SF,
		SEA,
		ARI,
		LAR, // Rams

		DAL,
		PHI,
		WAS,
		NYG,

		NO,
		ATL,
		CAR,
		TB,

		// AFC
		DEN,
		OAK,
		KC,
		LAC, // Chargers

		CLE,
		CIN,
		PIT,
		BAL,

		IND,
		JAC,
		TEN,
		HOU,

		NE,
		BUF,
		MIA,
		NYJ,

		FA, // Free agent

		TotalNFLTeams
	}
	
	public class PlayerData
	{
		public string playerName;
		public uint overallRank;
		public NFLTeam nflTeam;
		public Position position;
		public uint byeWeek;
	}

	// Database
	List<PlayerData> qbList;
	List<PlayerData> rbList;
	List<PlayerData> wrList;
	List<PlayerData> teList;
	List<PlayerData> kList;
	List<PlayerData> defList;

	List<PlayerData> searchResults;

	public PlayerDatabase()
	{
		qbList = new List<PlayerData>();
		rbList = new List<PlayerData>();
		wrList = new List<PlayerData>();
		teList = new List<PlayerData>();
		kList = new List<PlayerData>();
		defList = new List<PlayerData>();

		searchResults = new List<PlayerData>();
	}

	public void ImportPlayerData()
	{
		// Grab the file with the player data
		Debug.Log("Importing Contract Players...");
		string filename = "/" + (DateTime.Now.Year) + "DraftOverallRankings.csv";
		string filepath = Application.persistentDataPath + filename;

		// No Importing of contracts if the file does not exist
		if (!File.Exists(filepath))
		{
			Debug.Log(filepath + " does not exist!");
			return;
		}

		StreamReader reader = new StreamReader(filepath);
		while (!reader.EndOfStream)
		{
			// Rank : Player Name : Team : Position : Bye Week
			string playerLine = reader.ReadLine();
			string[] playerFields = playerLine.Split(',');
			playerFields[3] = new String(playerFields[3].Where(c => (c < '0' || c > '9')).ToArray());

			PlayerData newData = new PlayerData();
			newData.overallRank = Convert.ToUInt32(playerFields[0]);
			newData.playerName = playerFields[1];
			newData.nflTeam = GetNFLTeam(playerFields[2]);
			newData.byeWeek = Convert.ToUInt32(playerFields[4]);

			switch (playerFields[3])
			{
				case "QB":
					newData.position = Position.QB;
					qbList.Add(newData);
					break;
				case "RB":
					newData.position = Position.RB;
					rbList.Add(newData);
					break;
				case "WR":
					newData.position = Position.WR;
					wrList.Add(newData);
					break;
				case "TE":
					newData.position = Position.TE;
					teList.Add(newData);
					break;
				case "K":
					newData.position = Position.K;
					kList.Add(newData);
					break;
				case "DST":
					newData.position = Position.DEF;
					defList.Add(newData);
					break;
			}
		}

		Debug.Log(wrList.Count);
	}

	public PlayerData PickPlayerFromDatabase(string playerName)
	{
		// Get the number of matches
		int numMatches = NumMatchingPlayers(playerName);

		// Single match found, remove from database and return the player
		if (numMatches == 1)
		{
			PlayerData foundPlayer = searchResults.First();

			// Remove the player from the database
			switch (foundPlayer.position)
			{
				case Position.QB:
					qbList.Remove(foundPlayer);
					break;
				case Position.RB:
					rbList.Remove(foundPlayer);
					break;
				case Position.WR:
					wrList.Remove(foundPlayer);
					break;
				case Position.TE:
					teList.Remove(foundPlayer);
					break;
				case Position.K:
					kList.Remove(foundPlayer);
					break;
				case Position.DEF:
					defList.Remove(foundPlayer);
					break;
			}

			// Return the player
			return foundPlayer;
		}
		else
		{
			PlayerData newPlayer = new PlayerData();
			newPlayer.playerName = playerName;
			newPlayer.overallRank = 500;
			newPlayer.nflTeam = NFLTeam.FA;
			newPlayer.position = Position.NoPosition;
			newPlayer.byeWeek = 0;
			return newPlayer;
		}
	}

	// Gets the nfl team enum from the team string
	public NFLTeam GetNFLTeam(string teamString)
	{
		foreach(NFLTeam team in Enum.GetValues(typeof(NFLTeam)))
		{
			if(team.ToString() == teamString)
			{
				return team;
			}
		}

		return NFLTeam.TotalNFLTeams;
	}

	public int NumMatchingPlayers(string playerToFind)
	{
		searchResults.Clear();

		// Player is an empty string
		if(playerToFind.Length == 0)
		{
			return 0;
		}

		// Look through the QBs
		foreach (PlayerData player in qbList)
		{
			if(player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the RBs
		foreach (PlayerData player in rbList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the WRs
		foreach (PlayerData player in wrList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the TEs
		foreach (PlayerData player in teList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the Ks
		foreach (PlayerData player in kList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		// Look through the DEFs
		foreach (PlayerData player in defList)
		{
			if (player.playerName.IndexOf(playerToFind, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				searchResults.Add(player);
			}
		}

		return searchResults.Count;
	}

	public string GetSearchResult()
	{
		if(searchResults.Count > 1 || searchResults.Count == 0)
		{
			return string.Empty;
		}
		else
		{
			return searchResults.First().playerName;
		}
	}

	private uint arbitraryMaxRank = 9001;
	public PlayerData[] GetBestAvailablePlayers()
	{
		PlayerData[] bestAvailable = new PlayerData[5];

		int qbIndex = 0;
		int rbIndex = 0;
		int wrIndex = 0;
		int teIndex = 0;
		int kIndex = 0;
		int defIndex = 0;

		uint qbRank = arbitraryMaxRank;
		uint rbRank = arbitraryMaxRank;
		uint wrRank = arbitraryMaxRank;
		uint teRank = arbitraryMaxRank;
		uint kRank = arbitraryMaxRank;
		uint defRank = arbitraryMaxRank;

		if (qbList.Count > 0)
		{
			qbRank = qbList[0].overallRank;
		}

		if (rbList.Count > 0)
		{
			rbRank = rbList[0].overallRank;
		}

		if (wrList.Count > 0)
		{
			wrRank = wrList[0].overallRank;
		}

		if (teList.Count > 0)
		{
			teRank = teList[0].overallRank;
		}

		if (kList.Count > 0)
		{
			kRank = kList[0].overallRank;
		}

		if (defList.Count > 0)
		{
			defRank = defList[0].overallRank;
		}


		// Loop to add to best available players
		for (int i = 0; i < 5; ++i)
		{
			uint minRank = Math.Min(qbRank, Math.Min(rbRank, Math.Min(wrRank, Math.Min(teRank, Math.Min(kRank, defRank)))));

			if(minRank == arbitraryMaxRank)
			{
				bestAvailable[i] = null;
				continue;
			}

			// QB is lowest
			if (qbList[qbIndex].overallRank == minRank)
			{
				bestAvailable[i] = qbList[qbIndex];
				++qbIndex;
				if(qbIndex >= qbList.Count)
				{
					qbRank = arbitraryMaxRank;
				}
				else
				{
					qbRank = qbList[qbIndex].overallRank;
				}
			}
			// RB is lowest
			else if (rbList[rbIndex].overallRank == minRank)
			{
				bestAvailable[i] = rbList[rbIndex];
				++rbIndex;
				if (rbIndex >= rbList.Count)
				{
					rbRank = arbitraryMaxRank;
				}
				else
				{
					rbRank = rbList[rbIndex].overallRank;
				}
			}
			// WR is lowest
			else if (wrList[wrIndex].overallRank == minRank)
			{
				bestAvailable[i] = wrList[wrIndex];
				++wrIndex;
				if (wrIndex >= wrList.Count)
				{
					wrRank = arbitraryMaxRank;
				}
				else
				{
					wrRank = wrList[wrIndex].overallRank;
				}
			}
			// TE is lowest
			else if (teList[teIndex].overallRank == minRank)
			{
				bestAvailable[i] = teList[teIndex];
				++teIndex;
				if (teIndex >= teList.Count)
				{
					teRank = arbitraryMaxRank;
				}
				else
				{
					teRank = teList[teIndex].overallRank;
				}
			}
			// K is lowest
			else if (kList[kIndex].overallRank == minRank)
			{
				bestAvailable[i] = kList[kIndex];
				++kIndex;
				if (kIndex >= kList.Count)
				{
					kRank = arbitraryMaxRank;
				}
				else
				{
					kRank = kList[kIndex].overallRank;
				}
			}
			// DEF is lowest
			else
			{
				bestAvailable[i] = defList[defIndex];
				++defIndex;
				if (defIndex >= qbList.Count)
				{
					defRank = arbitraryMaxRank;
				}
				else
				{
					defRank = defList[defIndex].overallRank;
				}
			}
		}

		return bestAvailable;
	}

	public PlayerData[] GetBestPositionPlayers(Position playerPosition)
	{
		PlayerData[] bestAvailable = new PlayerData[5];

		for(int i = 0; i < 5; ++i)
		{
			switch (playerPosition)
			{
				case Position.QB:
					if(i < qbList.Count)
					{
						bestAvailable[i] = qbList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
				case Position.RB:
					if (i < rbList.Count)
					{ 
						bestAvailable[i] = rbList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
				case Position.WR:
					if (i < wrList.Count)
					{
						bestAvailable[i] = wrList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
				case Position.TE:
					if (i < teList.Count)
					{
						bestAvailable[i] = teList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
				case Position.K:
					if (i < kList.Count)
					{
						bestAvailable[i] = kList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
				case Position.DEF:
					if (i < defList.Count)
					{
						bestAvailable[i] = defList[i];
					}
					else
					{
						bestAvailable[i] = null;
					}
					break;
			}
		}

		return bestAvailable;
	}
}