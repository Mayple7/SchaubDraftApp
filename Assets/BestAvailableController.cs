using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BestAvailableController : MonoBehaviour
{
	// Best available order
	enum BestAvailableOption
	{
		TopAvailable,
		TopQBs,
		TopRBs,
		TopWRs,
		TopTEs,
		TopKs,
		TopDEFs
	}

	enum DisplayState
	{
		HideState,
		AnimateIn,
		ShowState,
		AnimateOut
	}

	// Nameplate objects
	public GameObject headerObject;

	public GameObject[] dataBackdrops;
	public Sprite[] teamBackgroundSprites;

	private float showXPosition = -6.7f;
	private float hideXPosition = -11.25f;

	private DraftTimerScript timerScript;
	private bool bestAvailableRunning = false;

	private int currentAnimatedBackdrop = -1;

	private DisplayState currentDisplayState = DisplayState.HideState;
	private BestAvailableOption currentDisplayOption = BestAvailableOption.TopAvailable;

	private float currentTimer = 0;
	public float nextBackdropDelay = 0.25f;
	public float maxShowTime = 5;

	// Use this for initialization
	void Start ()
	{
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Only progress if we're running
		if(bestAvailableRunning)
		{
			switch(currentDisplayState)
			{
				case DisplayState.HideState:
					// Switch the names around for the next display state
					SetBestAvailableNames();
					currentDisplayState = DisplayState.AnimateIn;
					currentTimer = 0;
					break;
				case DisplayState.AnimateIn:
					AnimateInBackdrops();
					break;
				case DisplayState.ShowState:
					currentTimer += Time.deltaTime;
					if(currentTimer >= maxShowTime)
					{
						currentDisplayState = DisplayState.AnimateOut;
						currentTimer = 0;
					}
					break;
				case DisplayState.AnimateOut:
					AnimateOutBackdrop();
					break;
			}
		}
	}

	private void SetBestAvailableNames()
	{
		// Go to the next display option, and loop around if needed
		currentDisplayOption += 1;
		if(currentDisplayOption > BestAvailableOption.TopDEFs)
		{
			currentDisplayOption = BestAvailableOption.TopAvailable;
		}

		// Best available overall
		PlayerDatabase.PlayerData[] bestPlayers = new PlayerDatabase.PlayerData[5];
		switch(currentDisplayOption)
		{
			case BestAvailableOption.TopAvailable:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available";
				bestPlayers = timerScript.playerDatabase.GetBestAvailablePlayers();
				break;
			case BestAvailableOption.TopQBs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available QBs";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.QB);
				break;
			case BestAvailableOption.TopRBs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available RBs";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.RB);
				break;
			case BestAvailableOption.TopWRs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available WRs";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.WR);
				break;
			case BestAvailableOption.TopTEs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available TEs";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.TE);
				break;
			case BestAvailableOption.TopKs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available Ks";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.K);
				break;
			case BestAvailableOption.TopDEFs:
				headerObject.GetComponentInChildren<TextMesh>().text = "Best Available DEFs";
				bestPlayers = timerScript.playerDatabase.GetBestPositionPlayers(PlayerDatabase.Position.DEF);
				break;
		}
		
		// Assign best players to the correct location
		for(int i = 0; i < 5; ++i)
		{
			GameObject backdrop;

			if (bestPlayers[i] == null)
			{
				backdrop = dataBackdrops[i];
				backdrop.GetComponentInChildren<TextMesh>().text = string.Empty;
				backdrop.GetComponent<SpriteRenderer>().sprite = teamBackgroundSprites[(int)PlayerDatabase.NFLTeam.FA];
				continue;
			}

			backdrop = dataBackdrops[i];
			backdrop.GetComponentInChildren<TextMesh>().text = bestPlayers[i].playerName;
			backdrop.GetComponent<SpriteRenderer>().sprite = teamBackgroundSprites[(int)bestPlayers[i].nflTeam];
		}
	}

	private void AnimateInBackdrops()
	{
		currentTimer += Time.deltaTime;

		// Backdrops to animate in
		if(currentAnimatedBackdrop < 5)
		{
			switch (currentAnimatedBackdrop)
			{
				// Header, just animate it right away
				case -1:
					headerObject.transform.DOMoveX(showXPosition, timerScript.animationTime);
					++currentAnimatedBackdrop;
					break;
				default:
					if (currentTimer > nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
					{
						if(dataBackdrops[currentAnimatedBackdrop].GetComponentInChildren<TextMesh>().text != string.Empty)
						{
							dataBackdrops[currentAnimatedBackdrop].transform.DOMoveX(showXPosition, timerScript.animationTime);
						}
						++currentAnimatedBackdrop;
					}
					break;
			}
		}
		// Done starting animation just waiting for time to be done
		else
		{
			if(currentTimer > timerScript.animationTime + nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
			{
				currentAnimatedBackdrop = -1;
				currentTimer = 0;
				currentDisplayState = DisplayState.ShowState;
			}
		}
	}

	private void AnimateOutBackdrop()
	{
		currentTimer += Time.deltaTime;

		// Backdrops to animate in
		if (currentAnimatedBackdrop < 5)
		{
			switch (currentAnimatedBackdrop)
			{
				// Header, just animate it right away
				case -1:
					headerObject.transform.DOMoveX(hideXPosition, timerScript.animationTime);
					++currentAnimatedBackdrop;
					break;
				default:
					if (currentTimer > nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
					{
						dataBackdrops[currentAnimatedBackdrop].transform.DOMoveX(hideXPosition, timerScript.animationTime);
						++currentAnimatedBackdrop;
					}
					break;
			}
		}
		// Done starting animation just waiting for time to be done
		else
		{
			if (currentTimer > timerScript.animationTime + nextBackdropDelay + currentAnimatedBackdrop * nextBackdropDelay)
			{
				currentAnimatedBackdrop = -1;
				currentTimer = 0;
				currentDisplayState = DisplayState.HideState;
			}
		}
	}

	public void SetBestAvailableControllerRunning(bool running)
	{
		bestAvailableRunning = running;

		// Set to not running, make sure to hide everything
		if(!bestAvailableRunning)
		{
			headerObject.transform.DOKill();
			headerObject.transform.DOMoveX(hideXPosition, timerScript.animationTime);

			foreach(GameObject backdrop in dataBackdrops)
			{
				backdrop.transform.DOKill();
				backdrop.transform.DOMoveX(hideXPosition, timerScript.animationTime);
			}
		}
	}
}
