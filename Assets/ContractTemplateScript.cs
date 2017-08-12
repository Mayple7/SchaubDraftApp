using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ContractTemplateScript : MonoBehaviour
{
	// Child objects
	public GameObject contractLengthObject;
	public GameObject contractPlayerObject;
	public GameObject contractReleaseButton;

	// Animation time
	private float animationTime;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Initialize(float initialYPosition, int contractLength, string contractPlayer)
	{
		animationTime = GameObject.Find("ContractPlayerFlow").GetComponent<ContractPlayerScript>().animationTime;

		// Set the initial position
		transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);

		// Set the initial contract length
		contractLengthObject.GetComponentInChildren<TextMesh>().text = contractLength + "Yr";

		// Set the initial contract player
		contractPlayerObject.GetComponentInChildren<InputField>().text = contractPlayer;

		// Start the animation for everything
		contractLengthObject.transform.DOMoveX(-6.0f, animationTime);
		contractPlayerObject.transform.DOMoveX(0, animationTime);
	}

	public void AnimateInReleaseButton()
	{
		contractReleaseButton.transform.DOMoveX(6.0f, animationTime);
	}

	public void AnimateEverythingOut()
	{
		contractLengthObject.transform.DOMoveX(-10, animationTime);
		contractPlayerObject.transform.DOMoveX(15, animationTime);
		contractReleaseButton.transform.DOMoveX(21, animationTime);

		Destroy(gameObject, animationTime);
	}
}
