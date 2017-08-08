using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class PickHistoryTicker : MonoBehaviour
{
	public List<GameObject> pickHistoryList;
	public GameObject pickHistoryTemplate;

	private DraftTimerScript timerScript;

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

	public void AddPickToHistory(string pickedPlayer)
	{
		// Delete extra hidden history (if needed)
		if(pickHistoryList.Count > maxPickHistory)
		{
			Destroy(pickHistoryList[0]);
			pickHistoryList.RemoveAt(0);
		}

		// Create New History
		int numNameplates = pickHistoryList.Count;
		GameObject newItem = Instantiate(pickHistoryTemplate, new Vector3(12, 3, 10), Quaternion.identity);
		pickHistoryList.Add(newItem);
		newItem.GetComponentInChildren<TextMesh>().text = pickedPlayer;

		// Move all pick histories
		for(int i = 0; i < pickHistoryList.Count; ++i)
		{
			// Hide this history nameplate
			if(i == 0 && pickHistoryList.Count > maxPickHistory)
			{
				pickHistoryList[i].transform.DOMoveX(12, timerScript.animationTime);
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
			else if(i == pickHistoryList.Count - 1)
			{
				pickHistoryList[i].transform.DOMoveX(6, timerScript.animationTime);
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
			else
			{
				pickHistoryList[i].transform.DOMoveY(pickHistoryList[i].transform.position.y - 1, timerScript.animationTime);
			}
		}
	}

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
