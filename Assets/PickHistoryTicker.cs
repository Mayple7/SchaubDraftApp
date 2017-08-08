using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PickHistoryTicker : MonoBehaviour
{
	// Pick history objects
	public List<GameObject> pickHistoryList;
	public GameObject pickHistoryTemplate;

	// Reference to the main script
	private DraftTimerScript timerScript;

	// Max number of pick history to show (any more and UI overlaps)
	private int maxPickHistory = 4;

	// Use this for initialization
	void Start()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	// Adds the given pick player to the history list
	public void AddPickToHistory(string pickedPlayer)
	{
		// Delete extra hidden history (if needed)
		if(pickHistoryList.Count > maxPickHistory)
		{
			Destroy(pickHistoryList[0]);
			pickHistoryList.RemoveAt(0);
		}

		// Create new pick history object, and add it to the list
		int numNameplates = pickHistoryList.Count;
		GameObject newItem = Instantiate(pickHistoryTemplate, new Vector3(12, 3, 10), Quaternion.identity);
		pickHistoryList.Add(newItem);
		newItem.GetComponentInChildren<TextMesh>().text = pickedPlayer;

		// Move all pick histories down a notch
		for(int i = 0; i < pickHistoryList.Count; ++i)
		{
			// Hide this oldest history nameplate (if we reach the max)
			if(i == 0 && pickHistoryList.Count > maxPickHistory)
			{
				pickHistoryList[i].transform.DOMoveX(12, timerScript.animationTime);
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
			// Show the new pick history object
			else if(i == pickHistoryList.Count - 1)
			{
				pickHistoryList[i].transform.DOMoveX(6, timerScript.animationTime);
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
			// Moves the other pick histories down
			else
			{
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
		}
	}

	// Hides all the pick history objects
	public void HideHistoryTicker()
	{
		// Move all pick histories
		for (int i = 0; i < pickHistoryList.Count; ++i)
		{
			// Kill any pending tweens
			DOTween.Kill(pickHistoryList[i]);

			// Hide this history nameplate
			pickHistoryList[i].transform.DOMoveX(12, timerScript.animationTime);
		}
	}
}
