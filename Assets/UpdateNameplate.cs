using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNameplate : MonoBehaviour
{
	private DraftTimerScript timerScript;
	private TextMesh textMesh;

	private DrafterEnum drafterID = DrafterEnum.Dan;

	// Use this for initialization
	void Start ()
	{
		textMesh = this.GetComponentInChildren<TextMesh>();
	}

	public void InitializeVariables(DrafterEnum drafter)
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		drafterID = drafter;

		if (this.GetComponentInParent<SpriteRenderer>())
		{
			this.GetComponent<SpriteRenderer>().sprite = timerScript.nameplateSprites[(int)drafterID];// DraftTimer.playerProfiles[(int)drafterID].playerNameplate;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Get the amount of bonus time remaining for the drafter
		float bonusTime = timerScript.playerProfiles[(int)drafterID].bonusTimeRemaining;

		string minutes = Mathf.Floor(bonusTime / 60).ToString();
		string seconds = Mathf.FloorToInt(bonusTime % 60).ToString("00");

		textMesh.text = minutes + ":" + seconds;
	}
}
