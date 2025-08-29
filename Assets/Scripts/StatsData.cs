using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Specific data blob
public struct StatsData
{
	public StatsData(DrafterEnum drafterStats, string firstText, string secondText)
	{
		drafter = drafterStats;
		topText = firstText;
		botText = secondText;
	}

	public DrafterEnum drafter;

	public string topText;
	public string botText;
}

// Class holding all our stats datas and helper functions for serializing.
public static class SavedDraftData
{
	public static List<StatsData> statsDatas = new List<StatsData>();

	public static void SerializeDraftStats()
	{
		StreamWriter statsWriter;

		string filename = "/AdvancedDraftStats" + DateTime.Now.Year + ".txt";
		string filepath = Application.persistentDataPath + filename;

		// Remove the old draft data
		if (File.Exists(filepath))
		{
			File.Delete(filepath);
		}

		statsWriter = new StreamWriter(filepath, true);
		statsWriter.AutoFlush = true;

		foreach (var draftString in statsDatas)
		{
			statsWriter.WriteLine(draftString.topText);
			statsWriter.WriteLine(draftString.botText);
		}

		statsWriter.Close();
	}
}
