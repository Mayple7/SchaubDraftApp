using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DraftFactsTicker : MonoBehaviour
{
	// Array for facts to show on the facts ticker.
	private string[] factsArray =
	{
		"Small Fact.",
		"A bit of a longer fact about something that most people won't look at.",
	};
	int currentFactIndex = 0;

	// Time between facts
	public float timeBetweenFacts = 5;
	public float factsScrollSpeed = 5;  // Arbitrary for now

	private enum FactTickerState
	{
		AnimateTicker,
		WaitToShowNext,
		ChangeText
	}
	private FactTickerState showingFact = FactTickerState.WaitToShowNext;
	private float currentTimer = 0;
	private float fullAnimationTime = 2;

	private RectTransform rectTransform;
	private Text textComponent;
	private float canvasScaleFactor;
	private Vector3 hiddenPosition;

	public Camera cam;

	// Use this for initialization
	void Start ()
	{
		rectTransform = GetComponent<RectTransform>();
		textComponent = GetComponent<Text>();
		canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
		hiddenPosition = rectTransform.position;
		hiddenPosition.y = -20;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (showingFact == FactTickerState.AnimateTicker)
		{
			currentTimer += Time.deltaTime;

			if (currentTimer >= timeBetweenFacts)
			{
				showingFact = FactTickerState.WaitToShowNext;

				// Reset the timer and showing fact state
				currentTimer = 0;
			}
		}
		else if(showingFact == FactTickerState.WaitToShowNext)
		{
			currentTimer += Time.deltaTime;

			if (currentTimer >= fullAnimationTime)
			{
				currentTimer = 0;
				showingFact = FactTickerState.ChangeText;

				// Set the fact to display
				textComponent.text = factsArray[currentFactIndex++];

				if (currentFactIndex >= factsArray.Length)
				{
					currentFactIndex = 0;
				}

				rectTransform.position = hiddenPosition;
			}
		}
		else
		{
			showingFact = FactTickerState.AnimateTicker;

			// Set the position to the right side of the screen
			float startXPosition = cam.pixelWidth + (rectTransform.rect.width * canvasScaleFactor) / 2;
			float fullDistance = startXPosition * 2;

			// Set the starting position
			Vector3 startPosition = rectTransform.position;
			startPosition.x = startXPosition;
			Vector3 endPosition;

			RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, startPosition, cam, out endPosition);
			endPosition.y = -4.740741f;
			rectTransform.position = endPosition;

			// Time = distance / rate (converting from ms to s)
			fullAnimationTime = fullDistance / (factsScrollSpeed * 1000);

			// Start the tween to move to the left side
			rectTransform.DOMoveX(-endPosition.x, fullAnimationTime).SetEase(Ease.Linear);
		}
	}
}
