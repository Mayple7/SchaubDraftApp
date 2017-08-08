using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class DraftOrderTicker : MonoBehaviour
{
	// Nameplate list and objects
	public List<GameObject> draftOrderNameplates;
	public GameObject nameplateTemplate;

	// Reference to main script
	private DraftTimerScript timerScript;

	// Max nameplates to show (5 along the top, 1 in the pick slot)
	public int maxNameplates = 6;

	// Use this for initialization
	void Start()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	// Add a given drafter to the order ticker
	public void AddNameplateToTicker(DrafterEnum drafterID)
	{
		// Create and initialize the nameplate
		int numNameplates = draftOrderNameplates.Count;
		draftOrderNameplates.Add(Instantiate(nameplateTemplate, new Vector3(-7.0f + numNameplates * 3.5f, 3.9f, 0), Quaternion.identity));
		draftOrderNameplates.LastOrDefault().GetComponent<UpdateNameplate>().InitializeVariables(drafterID);
	}

	// Returns the nameplate at the front of the draft order
	public GameObject GrabNextNameplate()
	{
		// Snag the front nameplate
		GameObject nextNameplate = draftOrderNameplates.FirstOrDefault();

		// Remove the front nameplate from the ticker list
		if(nextNameplate != null)
		{
			draftOrderNameplates.RemoveAt(0);
		}

		// Tween all the remaining ticker nameplates into place
		foreach(GameObject nameplate in draftOrderNameplates)
		{
			nameplate.transform.DOMoveX(nameplate.transform.position.x - 3.5f, timerScript.animationTime).SetEase(Ease.InOutQuad);
		}

		// Return the front nameplate
		return nextNameplate;
	}
}
