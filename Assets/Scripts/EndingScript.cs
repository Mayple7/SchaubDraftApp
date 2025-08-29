using DG.Tweening;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class EndingScript : MonoBehaviour
{
	// Show stats in this order
	enum StatsMessageOrder
	{
		// Picks based off expected draft location
		BestPick,
		WorstPick,

		// NFL teams stats
		MostDiverseTeam,
		LeastDiverseTeam,
		MostPicksFromOneTeam,

		// Draft time stats
		LongestDrafter,
		QuickestDrafter,
		MostBonusTimeUsed,
		LeastBonusTimeUsed,

		// Bye week stats
		MostPlayersOnSameByeWeek,
		LeastPlayersOnSameByeWeek,

		// Grade all the teams against each other
		DraftGrades,

		TotalStatsMessages
	}

	public List<Sprite> helmetSprites;

	public GameObject explosionPrefab;

	private Vector3 helmetStartPosition = new Vector3(-12.5f, 0, 0);
	private Vector3 topTextStartPosition = new Vector3(14.5f, 1.5f, 0);
	private Vector3 botTextStartPosition = new Vector3(14.5f, -1.5f, 0);
	private float helmetWaitPosition = 5;
	private float helmetSmashPosition = 0;
	private Vector3 helmetRotateAmount = new Vector3(0, 0, 15.0f);

	public float helmetEnterTime;
	public float helmetShowTime;
	public float helmetSmashTime;
	public float helmetWaitTime;
	public float helmetRotateTime;

	private int statsIndex = -1;
	public GameObject helmetObject;
	public GameObject topTextObject;
	public GameObject botTextObject;

	public float bestDrafterShowTime = 10.0f;

	// Use this for initialization
	void Start ()
	{
		Invoke("ShowNextStats", 2.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private void ShowNextStats()
	{
		++statsIndex;
		// Last stat is the best drafter. SUPER BOWL SUPER BOWL SUPER BOWL!
		if (this.statsIndex >= SavedDraftData.statsDatas.Count)
		{
			statsIndex = 0;
		}

		if (this.statsIndex == 0 && !this.GetComponent<AudioSource>().isPlaying)
		{
			this.GetComponent<AudioSource>().Stop();
			this.GetComponent<AudioSource>().Play();
		}

		StatsData statsData = SavedDraftData.statsDatas[statsIndex];

		// Set the sprites and text correctly
		this.helmetObject.GetComponent<SpriteRenderer>().sprite = this.helmetSprites[(int)statsData.drafter];

		this.topTextObject.GetComponent<TextMeshPro>().text = statsData.topText;
		this.botTextObject.GetComponent<TextMeshPro>().text = statsData.botText;

		// Animate to dueling formation
		this.helmetObject.transform.DOMoveX(-this.helmetWaitPosition, this.helmetEnterTime);
		this.topTextObject.transform.DOMoveX(this.helmetWaitPosition, this.helmetEnterTime);
		this.botTextObject.transform.DOMoveX(this.helmetWaitPosition, this.helmetEnterTime);

		float tempShowTime = this.helmetShowTime;
		if (this.statsIndex == SavedDraftData.statsDatas.Count - 1)
		{
			tempShowTime = bestDrafterShowTime;
		}

		// Rotate for impact
		this.helmetObject.transform.DORotate(-this.helmetRotateAmount, this.helmetRotateTime).SetDelay(this.helmetEnterTime + tempShowTime).SetEase(Ease.Linear);

		// Smash into each other
		// Explosion
		// Reset positions
		this.helmetObject.transform.DOMoveX(this.helmetSmashPosition, this.helmetSmashTime).SetDelay(this.helmetEnterTime + tempShowTime + this.helmetRotateTime).OnStart(() => Invoke("SpawnExplosion", 0.15f));
		this.topTextObject.transform.DOMoveX(this.helmetSmashPosition, this.helmetSmashTime).SetDelay(this.helmetEnterTime + tempShowTime + this.helmetRotateTime);
		this.botTextObject.transform.DOMoveX(this.helmetSmashPosition, this.helmetSmashTime).SetDelay(this.helmetEnterTime + tempShowTime + this.helmetRotateTime).OnComplete(this.ResetPositions);
	}

	private void SpawnExplosion()
	{
		GameObject explosion = GameObject.Instantiate(this.explosionPrefab, Vector3.zero, Quaternion.identity);

		GameObject.Destroy(explosion, 4.0f);
	}

	private void ResetPositions()
	{
		// Wait a sec until repeat
		this.helmetObject.transform.position = this.helmetStartPosition;
		this.topTextObject.transform.position = this.topTextStartPosition;
		this.botTextObject.transform.position = this.botTextStartPosition;

		this.helmetObject.transform.rotation = Quaternion.identity;

		this.helmetObject.transform.DOMove(this.helmetStartPosition, this.helmetWaitTime).OnComplete(this.ShowNextStats);
	}
}
