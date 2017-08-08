using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class DraftOrderTicker : MonoBehaviour
{
	public List<GameObject> draftOrderNameplates;
	public GameObject nameplateTemplate;

	private DraftTimerScript timerScript;

	private int maxNameplates = 6;

	// Use this for initialization
	void Start()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void AddNameplateToTicker(DrafterEnum drafterID)
	{
		int numNameplates = draftOrderNameplates.Count;
		draftOrderNameplates.Add(Instantiate(nameplateTemplate, new Vector3(-7.0f + numNameplates * 3.5f, 3.9f, 0), Quaternion.identity));

		UpdateNameplate nameplateScript = draftOrderNameplates.LastOrDefault().GetComponent<UpdateNameplate>();
		if (nameplateScript)
		{
			nameplateScript.InitializeVariables(drafterID);
		}
	}

	public GameObject GrabNextNameplate()
	{
		// Snag the front nameplate
		GameObject nextNameplate = draftOrderNameplates.FirstOrDefault();

		// Remove the front nameplate from the ticker
		if(nextNameplate != null)
		{
			draftOrderNameplates.RemoveAt(0);
		}

		// Tween all the remaining ticker nameplates
		foreach(GameObject nameplate in draftOrderNameplates)
		{
			nameplate.transform.DOMoveX(nameplate.transform.position.x - 3.5f, timerScript.animationTime).SetEase(Ease.InOutQuad);
		}

		return nextNameplate;
	}

	public int GetMaxNameplates()
	{
		return maxNameplates;
	}
}
