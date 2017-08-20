using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNameplate : MonoBehaviour
{
	// Reference to the main script
	private DraftTimerScript timerScript;

	// This object's text mesh component (we reference this a lot)
	private TextMesh textMesh;

	// This nameplate's drafterID
	private DrafterEnum drafterID = DrafterEnum.Dan;

	// PositionInfos
	public GameObject[] positionTags;

	// Use this for initialization
	void Start ()
	{
		textMesh = this.GetComponentInChildren<TextMesh>();
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

	public void ShowPositionNeeds()
	{

	}
}
