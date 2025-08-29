using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningThemeIntro : MonoBehaviour
{
	enum OpeningState
	{
		StartLogo,
		StartFirstDivision,
		StartSecondDivision,
		StartThirdDivision,
		StartNextDraftOrder,
		GoToMainDraft
	}

	public Sprite[] helmetSpriteOrder;
	private Vector3 helmetStartPosition = new Vector3(12.5f, 0, 0);
	private Vector3 helmetWaitPosition = new Vector3(5, 0, 0);
	private Vector3 helmetSmashPosition = new Vector3(0, 0, 0);
	private Vector3 helmetRotateAmount = new Vector3(0, 0, 15.0f);

	public float helmetEnterTime;
	public float helmetShowTime;
	public float helmetSmashTime;
	public float helmetWaitTime;
	public float helmetRotateTime;

	public GameObject leftSide;
	public GameObject rightSide;
	public GameObject leftHelmetObject;
	public GameObject rightHelmetObject;
	public GameObject explosionPrefab;

	private string[] draftOrderStrings = 
	{
		"Trevor : 3-11\n1st Pick",
		"Dan : 4-10\n2nd Pick",
		"Kopman : 5-9\n3rd Pick",
		"Jake : 5-9\n4th Pick",

		"Drew : 6-8\n5th Pick",
		"Hans : 7-7\n6th Pick",
		"Patrick : 10-4\n7th Pick",
		"Colin : 8-6\n8th Pick",

		"Sheebs : 8-6\n9th Pick",
		"Ben : 11-3\n10th Pick",
		"Mike : 8-6\n11th Pick",
		"Yabbs : 9-5\n12th Pick"
	};

	public float animationTime = 5.0f;
	private float animateLogoOutTime = 18.5f;
	private float songDelay = 18.7f;

	// This order for things moving.
	public GameObject mainLogo;
	private Vector3 mainLogoStartPosition;
	public GameObject division1Image;
	public GameObject division2Image;
	public GameObject division3Image;
	private int draftOrderIndex = -1;

	private float divisionImageXFirst = -5.0f;
	private float divisionImageXSecond = 5.0f;
	private float divisionImageXLast = 12.75f;

	// Use this for initialization
	void Start ()
	{
		this.mainLogoStartPosition = this.mainLogo.transform.position;
		this.StartNextAnimation(OpeningState.StartLogo);
		this.StartNextAnimation(OpeningState.StartFirstDivision);
	}

	void Update()
	{
		if (!this.GetComponent<AudioSource>().isPlaying)
		{
			SceneManager.LoadScene("DraftRoom");
		}
	}

	private void StartNextAnimation(OpeningState newState)
	{
		switch (newState)
		{
			case OpeningState.StartLogo:
				this.mainLogo.transform.DOMoveY(0, animationTime).SetEase(Ease.Linear);
				this.mainLogo.transform.DOMoveY(-this.mainLogo.transform.position.y, 0.5f).SetDelay(this.animateLogoOutTime);
				break;
			case OpeningState.StartFirstDivision:
				// Move after the delay.
				this.division1Image.transform.DOMoveX(this.divisionImageXFirst, 0.25f).SetEase(Ease.Linear).SetDelay(songDelay);
				this.division1Image.transform.DOMoveX(this.divisionImageXSecond, 1.75f).SetEase(Ease.Linear).SetDelay(songDelay + 0.25f).OnComplete(() => this.StartNextAnimation(OpeningState.StartSecondDivision));
				this.division1Image.transform.DOMoveX(this.divisionImageXLast, 0.25f).SetEase(Ease.Linear).SetDelay(songDelay + 2.0f);
				break;
			case OpeningState.StartSecondDivision:
				this.mainLogo.transform.position = this.mainLogoStartPosition;
				// Moving in from the right side.
				this.division2Image.transform.DOMoveX(-this.divisionImageXFirst, 0.25f).SetEase(Ease.Linear);
				this.division2Image.transform.DOMoveX(-this.divisionImageXSecond, 1.5f).SetEase(Ease.Linear).SetDelay(0.25f).OnComplete(() => this.StartNextAnimation(OpeningState.StartThirdDivision));
				this.division2Image.transform.DOMoveX(-this.divisionImageXLast, 0.25f).SetEase(Ease.Linear).SetDelay(1.75f);
				break;
			case OpeningState.StartThirdDivision:
				this.division3Image.transform.DOMoveX(this.divisionImageXFirst, 0.25f).SetEase(Ease.Linear);
				this.division3Image.transform.DOMoveX(this.divisionImageXSecond, 1.5f).SetEase(Ease.Linear).SetDelay(0.25f).OnComplete(() => this.StartNextAnimation(OpeningState.StartNextDraftOrder));
				this.division3Image.transform.DOMoveX(this.divisionImageXLast, 0.25f).SetEase(Ease.Linear).SetDelay(1.75f);
				break;
			case OpeningState.StartNextDraftOrder:
				++draftOrderIndex;
				if (this.draftOrderIndex >= draftOrderStrings.Length)
				{
					this.StartNextAnimation(OpeningState.GoToMainDraft);
					return;
				}

				// Set the sprites and text correctly
				this.leftSide.GetComponentInChildren<SpriteRenderer>().sprite = this.helmetSpriteOrder[draftOrderIndex];
				this.leftSide.GetComponentInChildren<TextMeshPro>().text = this.draftOrderStrings[draftOrderIndex];

				++draftOrderIndex;
				this.rightSide.GetComponentInChildren<SpriteRenderer>().sprite = this.helmetSpriteOrder[draftOrderIndex];
				this.rightSide.GetComponentInChildren<TextMeshPro>().text = this.draftOrderStrings[draftOrderIndex];

				// Animate to dueling formation
				this.leftSide.transform.DOMove(-this.helmetWaitPosition, this.helmetEnterTime);
				this.rightSide.transform.DOMove(this.helmetWaitPosition, this.helmetEnterTime);

				// Rotate for impact
				this.leftHelmetObject.transform.DORotate(-this.helmetRotateAmount, this.helmetRotateTime).SetDelay(this.helmetEnterTime + this.helmetShowTime).SetEase(Ease.Linear);
				this.rightHelmetObject.transform.DORotate(this.helmetRotateAmount, this.helmetRotateTime).SetDelay(this.helmetEnterTime + this.helmetShowTime).SetEase(Ease.Linear);

				// Smash into each other
				// Explosion
				// Reset positions
				this.leftSide.transform.DOMove(this.helmetSmashPosition, this.helmetSmashTime).SetDelay(this.helmetEnterTime + this.helmetShowTime + this.helmetRotateTime).OnStart(() => Invoke("SpawnExplosion", 0.15f));
				this.rightSide.transform.DOMove(this.helmetSmashPosition, this.helmetSmashTime).SetDelay(this.helmetEnterTime + this.helmetShowTime + this.helmetRotateTime).OnComplete(this.ResetHelmetPositions);
				break;
			case OpeningState.GoToMainDraft:
				this.mainLogo.transform.DOMoveY(0, 6.0f);
				break;
		}
	}

	private void SpawnExplosion()
	{
		GameObject explosion = GameObject.Instantiate(this.explosionPrefab, Vector3.zero, Quaternion.identity);

		GameObject.Destroy(explosion, 4.0f);
	}

	private void ResetHelmetPositions()
	{
		// Wait a sec until repeat
		this.leftSide.transform.position = -this.helmetStartPosition;
		this.rightSide.transform.position = this.helmetStartPosition;

		this.leftHelmetObject.transform.rotation = Quaternion.identity;
		this.rightHelmetObject.transform.rotation = Quaternion.identity;

		this.leftSide.transform.DOMove(-this.helmetStartPosition, this.helmetWaitTime).OnComplete(() => this.StartNextAnimation(OpeningState.StartNextDraftOrder));
	}
}
