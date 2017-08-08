using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InputPickScript : MonoBehaviour
{
	public float AnimationTime = 0.5f;

	private Vector3 DisplayPosition = new Vector3(-1.9f, -3, 0);
	private Vector3 HiddenPosition = new Vector3(-14, -3, 0);

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Show()
	{
		this.transform.DOMove(DisplayPosition, AnimationTime);
	}

	public void Hide()
	{
		this.transform.DOMove(HiddenPosition, AnimationTime);
	}
}
