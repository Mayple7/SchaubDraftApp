using UnityEngine;

public class BestAvailableCycleButton : MonoBehaviour
{
	private BestAvailableController bestAvailableController;

	// Use this for initialization
	void Start ()
	{
		this.bestAvailableController = GameObject.Find("BestAvailableController").GetComponent<BestAvailableController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		// Notify the best available controller to go to the next list.
		this.bestAvailableController.CycleBestAvailableList();
	}
}
