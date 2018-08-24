using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakingNewsScript : MonoBehaviour
{
	// Array of good breaking news strings.
	private List<string> goodBreakingNews = new List<string>
	{
		"BREAKING NEWS: Surprising experts everywhere, {0} snagged {1} much later than expected.",
		"BREAKING NEWS: Looks like {0} found a gem in {1} who was surprisingly still on the board.",
		"BREAKING NEWS: Experts stunned as {1} was still on the board. {0} must be ecstatic with the steal.",
		"BREAKING NEWS: {0} wasn't about to pass up the chance to draft {1} who should return excellent value.",
		"BREAKING NEWS: {1} may be one of the steals of the draft as {0} had the easy decision picking him up this late in the draft.",
		"BREAKING NEWS: Sources say {0} was able to hide {1} from everyone allowing them to pick him up well past his expected draft position.",
		"BREAKING NEWS: {0} is looking very confident after grabbing the great player {1}. Many consider this to be the steal of the draft.",
	};
	int goodNewsIndex = 0;

	// Array of bad breaking news strings.
	private List<string> badBreakingNews = new List<string>
	{
		"BREAKING NEWS: Sources say that {0} fell in love with {1}'s talent, but word around the league is that pick may have been a reach.",
		"BREAKING NEWS: With a surprise pick, {0} went up and got their guy. However, {1} was projected to be taken in later rounds.",
		"BREAKING NEWS: Owners around the league are shocked at {0} who drafted {1} well above their expected position.",
		"BREAKING NEWS: Questions arise as {1} was taken by {0} much earlier than expected.",
		"BREAKING NEWS: Are you trying to lose {0}? {1} could have been drafter in later rounds.",
		"BREAKING NEWS: Owners breathe a sigh of relief as {0} takes {1} off the board, leaving several other promising prospects available.",
		"BREAKING NEWS: Did {0} fall for a trap pick? Experts say that {1} won't produce as much value as other available players.",
	};
	int badNewsIndex = 0;

	public float newsScrollSpeed = 5;  // Arbitrary for now
	private float breakingNewsAnimateTime = 2.0f;

	private enum BreakingNewsTickerState
	{
		AnimateIn,
		AnimateTicker,
		AnimateOut,
		WaitForNextNews,
		SetupNextNews
	}

	private BreakingNewsTickerState showingNews = BreakingNewsTickerState.WaitForNextNews;
	private float currentTimer = 0;
	private float fullAnimationTime = 2;

	private RectTransform rectTransform;
	private Text textComponent;
	private float canvasScaleFactor;
	private Vector3 hiddenPosition;

	public Camera cam;

	public GameObject parentNewsTicker;
	private bool animatingNews = false;

	private Queue<string> breakingNewsQueue = new Queue<string>();
	private DraftTimerScript timerScript;

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		rectTransform = GetComponent<RectTransform>();
		textComponent = GetComponent<Text>();
		canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
		hiddenPosition = rectTransform.position;
		hiddenPosition.y = -20;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// News is not animating.
		if (!animatingNews)
		{
			this.SetNextNewsState(BreakingNewsTickerState.SetupNextNews);
		}
	}

	public void AddNewsToTicker(DrafterEnum drafter, string playerName, bool isGoodNews)
	{
		if (isGoodNews)
		{
			this.breakingNewsQueue.Enqueue(string.Format(this.goodBreakingNews[this.goodNewsIndex], timerScript.DrafterNames[(int)drafter], playerName));

			if (++this.goodNewsIndex >= this.goodBreakingNews.Count)
			{
				this.goodNewsIndex = 0;
			}
		}
		else
		{
			this.breakingNewsQueue.Enqueue(string.Format(this.badBreakingNews[this.badNewsIndex], timerScript.DrafterNames[(int)drafter], playerName));

			if (++this.badNewsIndex >= this.badBreakingNews.Count)
			{
				this.badNewsIndex = 0;
			}
		}
	}

	private void SetNextNewsState(BreakingNewsTickerState newState)
	{
		switch (newState)
		{
			case BreakingNewsTickerState.AnimateIn:
				this.AnimateInBreakingNews();
				break;
			case BreakingNewsTickerState.AnimateTicker:
				this.BeginAnimateNewsTicker();
				break;
			case BreakingNewsTickerState.AnimateOut:
				this.AnimateOutBreakingNews();
				break;
			case BreakingNewsTickerState.WaitForNextNews:
				this.animatingNews = false;
				break;
			case BreakingNewsTickerState.SetupNextNews:
				if (!this.animatingNews)
				{
					this.StartNextBreakingNews();
				}
				break;
		}
	}

	private void AnimateInBreakingNews()
	{
		this.parentNewsTicker.transform.DOMoveY(0, this.breakingNewsAnimateTime).OnComplete(() => this.SetNextNewsState(BreakingNewsTickerState.AnimateTicker));

		this.parentNewsTicker.GetComponent<AudioSource>().Play();
	}

	private void BeginAnimateNewsTicker()
	{
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
		fullAnimationTime = fullDistance / (newsScrollSpeed * 1000);

		// Start the tween to move to the left side
		rectTransform.DOMoveX(-endPosition.x, fullAnimationTime).SetEase(Ease.Linear).OnComplete(() => this.SetNextNewsState(BreakingNewsTickerState.AnimateOut));
	}

	private void AnimateOutBreakingNews()
	{
		this.parentNewsTicker.transform.DOMoveY(-0.6f, this.breakingNewsAnimateTime).OnComplete(() => this.SetNextNewsState(BreakingNewsTickerState.WaitForNextNews));
	}

	private void StartNextBreakingNews()
	{
		if (this.breakingNewsQueue.Count > 0)
		{
			string nextNews = this.breakingNewsQueue.Dequeue();

			textComponent.text = nextNews;

			rectTransform.position = hiddenPosition;

			this.animatingNews = true;

			this.SetNextNewsState(BreakingNewsTickerState.AnimateIn);
		}
	}
}
