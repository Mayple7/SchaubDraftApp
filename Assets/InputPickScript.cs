using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InputPickScript : MonoBehaviour
{
	// Input field positions
	private Vector3 DisplayPosition = new Vector3(-1.9f, -3, 0);
	private Vector3 HiddenPosition = new Vector3(-14, -3, 0);

	// Reference to the main script
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, timerScript.quickAnimationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, timerScript.quickAnimationTime);
	}
}
