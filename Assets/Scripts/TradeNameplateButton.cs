using UnityEngine;

public class TradeNameplateButton : MonoBehaviour
{
	public DrafterEnum Drafter;

	private Vector3 StartPosition;
	public Vector3 HiddenPosition = new Vector3(-15, 0, 0);

	private TradeButton tradeButtonScript;

	// Use this for initialization
	void Start ()
	{
		this.StartPosition = this.transform.position;
		this.tradeButtonScript = GameObject.Find("TradeButton").GetComponent<TradeButton>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void ResetToStartPosition()
	{
		this.transform.position = this.StartPosition;
		print(Drafter + ": " + this.transform.position);
	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		this.transform.position = HiddenPosition;

		this.tradeButtonScript.SetDrafter(this.Drafter);
	}
}
