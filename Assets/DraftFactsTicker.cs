using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DraftFactsTicker : MonoBehaviour
{
	// Array for facts to show on the facts ticker.
	private List<string> factsList = new List<string>
	{
		"The Schaub Keeper League is entering its 12th season!",
		"Tom Brady is the only player who has been kept by the same team in every season (Kopman).",
		"In a legendary league post in 2006, Sheebs pointed out that Patrick looks a lot like Robbie Gould.",
		"Ever since the league went to multiple divisions in 2010, every champion has come from the second division.",
		"Ties are an embarrasment to everyone involved. The most ties in one season go to Sheebs and Trevor with 2 ties each.",
		"The 2010 season saw the inaguration of the coveted Schaub Jersey that is awarded to the winner of the league each year.",
		"The 2012 season was the first season with flex spots in rosters. This was a heavily debated topic in previous years.",
		"Everyone changing team names every year made it a pain to compile stats... Grrrr...",
		"One of the greatest ties in league history came after a week-long trash talking tirade between Dan and Sheebs.",
		"The buy-out rule for keepers was introduced in the 2016 season and was used 6 times.",

	};
	private List<string> usedFactsList = new List<string>();
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
				SetNewFactText();

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

	private void SetNewFactText()
	{
		// Pick a random fact
		int factIndex = Random.Range(0, factsList.Count - 1);
		textComponent.text = factsList[factIndex];

		// Remove the fact and add it to the used list
		usedFactsList.Add(factsList[factIndex]);
		factsList.RemoveAt(factIndex);

		// Reset the facts list
		if (factsList.Count == 0)
		{
			factsList.AddRange(usedFactsList);
			usedFactsList.Clear();
		}
	}
}
