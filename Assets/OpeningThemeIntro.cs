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

	private string[] draftOrderStrings = 
	{
		"Draft Order:",
		"Parks : 3-9-1\n970 Pts",
		"Colin : 4-9\n1147 Pts",
		"Ben : 4-8-1\n969 Pts",
		"Patrick : 5-7-1\n1183 Pts",

		"Hans : 6-7\n1077 Pts",
		"Dan : 6-6-1\n1018 Pts",
		"Kopman : 7-6\n1043 Pts",
		"Doug : 8-5\n1177 Pts",

		"Trevor : 8-5\n1305 Pts",
		"Drew : 9-4\n1190 Pts",
		"Sheebs : 8-5\n1194 Pts",
		"Jake : 7-4-2\n1191 Pts"
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
	public GameObject draftOrderText;
	private Vector3 draftOrderTextStartPosition;
	private float draftOrderTextEndYPos = 7.75f;
	private int draftOrderIndex = -1;

	private float divisionImageXFirst = -5.0f;
	private float divisionImageXSecond = 5.0f;
	private float divisionImageXLast = 12.75f;

	// Use this for initialization
	void Start ()
	{
		this.mainLogoStartPosition = this.mainLogo.transform.position;
		this.draftOrderTextStartPosition = this.draftOrderText.transform.position;
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

				this.draftOrderText.GetComponent<TextMeshPro>().text = this.draftOrderStrings[this.draftOrderIndex];

				// Restart draft order.
				this.draftOrderText.transform.position = this.draftOrderTextStartPosition;
				this.draftOrderText.transform.DOMoveY(this.draftOrderTextEndYPos, 5).SetEase(Ease.Linear).OnComplete(() => this.StartNextAnimation(OpeningState.StartNextDraftOrder));
				break;
			case OpeningState.GoToMainDraft:
				this.mainLogo.transform.DOMoveY(0, 2.0f);
				break;
		}
	}
}
