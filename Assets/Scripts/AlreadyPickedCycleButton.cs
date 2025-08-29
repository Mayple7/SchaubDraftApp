using UnityEngine;

public class AlreadyPickedCycleButton : MonoBehaviour
{
	private AlreadyPickedController alreadyPickedController;

	// Use this for initialization
	void Start()
	{
		this.alreadyPickedController = GameObject.Find("AlreadyPickedController").GetComponent<AlreadyPickedController>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	// Button activated
	private void OnMouseUpAsButton()
	{
		// Notify the best available controller to go to the next list.
		this.alreadyPickedController.CycleRecentlyPickedList();
	}
}
