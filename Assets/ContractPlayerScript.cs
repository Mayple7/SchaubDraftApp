using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ContractPlayerScript : MonoBehaviour
{
	enum ContractPhaseState
	{
		StoppedState,
		AnimateToNextDrafter,
		SignContractsPhase,
		AnimateToBuyOutPhase,
		BuyOutPhase,
		AnimateOutEverything
	};

	private ContractPhaseState currentContractPhase = ContractPhaseState.StoppedState;

	// Template for the contract object
	public GameObject contractTemplateObject;

	// Only one three player contract and is always there
	private GameObject threePlayerContract;

	// All other contracts are placed here
	private List<GameObject> otherContractedPlayers = new List<GameObject>();
	private List<GameObject> signedPlayers = new List<GameObject>();

	// Child objects
	private float threeYearPickYValue = 1.5f;
	private float contractYValueDifference = 1.25f;
	private float currentYValue;

	// Keeps track of total contracts
	private int totalContracts = 1;

	// Reference to the main script
	private DraftTimerScript timerScript;
	private DrafterEnum currentContractDrafter;
	private int contractDrafterIndex = 0;

	private float currentTimer = 0;
	public float animationTime = 1;

	// Use this for initialization
	void Start ()
	{
		currentYValue = threeYearPickYValue;
		timerScript = GameObject.Find("DraftTimer").GetComponent<DraftTimerScript>();
		contractDrafterIndex = (int)DrafterEnum.TotalDrafters - 1;
		currentContractDrafter = timerScript.DraftOrder[contractDrafterIndex];

		currentLabelText = "Signing Open Contracts: " + timerScript.DrafterNames[(int)currentContractDrafter];
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(currentContractPhase)
		{
			case ContractPhaseState.StoppedState:
			case ContractPhaseState.SignContractsPhase:
				break;
			case ContractPhaseState.AnimateToNextDrafter:
				currentTimer += Time.deltaTime;
				if(currentTimer >= animationTime)
				{
					currentTimer = 0;
					currentContractPhase = ContractPhaseState.SignContractsPhase;
					timerScript.UpdateLabels();
				}
				break;
			case ContractPhaseState.AnimateToBuyOutPhase:
				currentTimer += Time.deltaTime;
				if (currentTimer >= animationTime)
				{
					currentTimer = 0;
					currentContractPhase = ContractPhaseState.BuyOutPhase;
					timerScript.UpdateLabels();
				}
				break;
			case ContractPhaseState.AnimateOutEverything:
				currentTimer += Time.deltaTime;
				if (currentTimer >= animationTime)
				{
					--contractDrafterIndex;
					if(contractDrafterIndex >= 0)
					{
						currentTimer = 0;
						currentContractPhase = ContractPhaseState.AnimateToNextDrafter;
						currentContractDrafter = timerScript.DraftOrder[contractDrafterIndex];
						InitializeContracts();
					}
					else
					{
						GameObject.Find("NextContractButton").GetComponent<NextContractButton>().Hide();
						timerScript.StartMainDraft();
						Destroy(gameObject);
					}
				}
				break;
		}
	}

	public void GoToNextState()
	{
		switch (currentContractPhase)
		{
			case ContractPhaseState.StoppedState:
				currentContractPhase = ContractPhaseState.AnimateToNextDrafter;
				GameObject.Find("NextContractButton").GetComponent<NextContractButton>().Show();
				InitializeContracts();
				timerScript.SwitchRingColor(timerScript.ringBlue);
				break;
			case ContractPhaseState.SignContractsPhase:
				// Ensure a 3-year contract was signed
				if(threePlayerContract.GetComponentInChildren<InputField>().text.Length > 0)
				{
					currentContractPhase = ContractPhaseState.AnimateToBuyOutPhase;
					AnimateInBuyOutButtons();
					timerScript.SwitchRingColor(timerScript.ringOther);
				}
				break;
			case ContractPhaseState.BuyOutPhase:
				currentContractPhase = ContractPhaseState.AnimateOutEverything;
				SaveAndClearCurrentContracts();
				timerScript.SwitchRingColor(timerScript.ringBlue);
				currentYValue = threeYearPickYValue;
				break;
		}
	}

	public void InitializeContracts()
	{
		// No one starts with a 3-year contract so create one here
		threePlayerContract = Instantiate(contractTemplateObject, transform);
		threePlayerContract.GetComponent<ContractTemplateScript>().Initialize(threeYearPickYValue, 3, string.Empty);
		currentYValue -= contractYValueDifference;

		// Update the total contracts
		totalContracts += timerScript.playerProfiles[(int)currentContractDrafter].oldThreeYearContracts.Count + timerScript.playerProfiles[(int)currentContractDrafter].oldTwoYearContracts.Count;

		// Drafter has less than 3 contracts with an open 2 year slot - create a blank slot
		if(totalContracts < 3 && timerScript.playerProfiles[(int)currentContractDrafter].oldThreeYearContracts.Count == 0)
		{
			GameObject newContract = Instantiate(contractTemplateObject, transform);
			newContract.GetComponent<ContractTemplateScript>().Initialize(currentYValue, 2, string.Empty);
			otherContractedPlayers.Add(newContract);
			signedPlayers.Add(newContract);
			currentYValue -= contractYValueDifference;
		}
		else
		{
			// Import each of the contracts starting with 2-year and ending with 1-year
			foreach (string contract in timerScript.playerProfiles[(int)currentContractDrafter].oldThreeYearContracts)
			{
				GameObject newContract = Instantiate(contractTemplateObject, transform);
				newContract.GetComponent<ContractTemplateScript>().Initialize(currentYValue, 2, contract);
				newContract.GetComponentInChildren<InputField>().interactable = false;
				otherContractedPlayers.Add(newContract);
				currentYValue -= contractYValueDifference;
			}
		}

		if (totalContracts < 3 && timerScript.playerProfiles[(int)currentContractDrafter].oldTwoYearContracts.Count == 0)
		{
			GameObject newContract = Instantiate(contractTemplateObject, transform);
			newContract.GetComponent<ContractTemplateScript>().Initialize(currentYValue, 1, string.Empty);
			otherContractedPlayers.Add(newContract);
			signedPlayers.Add(newContract);
			currentYValue -= contractYValueDifference;
		}
		else
		{
			foreach (string contract in timerScript.playerProfiles[(int)currentContractDrafter].oldTwoYearContracts)
			{
				GameObject newContract = Instantiate(contractTemplateObject, transform);
				newContract.GetComponent<ContractTemplateScript>().Initialize(currentYValue, 1, contract);
				newContract.GetComponentInChildren<InputField>().interactable = false;
				otherContractedPlayers.Add(newContract);
				currentYValue -= contractYValueDifference;
			}
		}
	}

	public List<string> GetContractsOfSpecifiedLength(int contractLength)
	{
		List<string> contractNames = new List<string>();

		// Return the 3 year contract
		if(contractLength == 3)
		{
			contractNames.Add(threePlayerContract.GetComponentInChildren<InputField>().text);
		}
		else
		{
			foreach(GameObject contract in otherContractedPlayers)
			{
				if (contract.GetComponentInChildren<Text>().text.Contains(contractLength.ToString()))
				{
					contractNames.Add(contract.GetComponentInChildren<InputField>().text);
				}
			}
		}

		return contractNames;
	}

	public void AnimateInBuyOutButtons()
	{
		// Count of delayed picks
		int numDelayedPicks = 1;

		threePlayerContract.GetComponentInChildren<InputField>().interactable = false;

		for (int i = 0; i < otherContractedPlayers.Count; ++i)
		{
			GameObject contract = otherContractedPlayers[i];
			// Ensure the contract doesn't exist in the signed players
			if(!signedPlayers.Contains(contract))
			{
				contract.GetComponent<ContractTemplateScript>().AnimateInReleaseButton();
			}
			else
			{
				contract.GetComponentInChildren<InputField>().interactable = false;
			}

			// Remove empty contracts
			if (otherContractedPlayers[i].GetComponentInChildren<InputField>().text.Length == 0)
			{
				BuyOutKeeper(otherContractedPlayers[i]);
				--i;
			}
			else
			{
				++numDelayedPicks;
			}
		}

		timerScript.playerProfiles[(int)currentContractDrafter].totalContractedPlayers = 1 + otherContractedPlayers.Count;
	}

	public void SaveAndClearCurrentContracts()
	{
		// The 3-year contract
		timerScript.playerProfiles[(int)currentContractDrafter].threeYearContract = threePlayerContract.GetComponentInChildren<InputField>().text;
		threePlayerContract.GetComponent<ContractTemplateScript>().AnimateEverythingOut();

		foreach(GameObject contract in otherContractedPlayers)
		{
			contract.GetComponent<ContractTemplateScript>().AnimateEverythingOut();

			if(contract.GetComponentInChildren<InputField>().text.Length == 0)
			{
				continue;
			}

			// 2 year contract
			if(contract.transform.Find("ContractLengthBackground").gameObject.GetComponentInChildren<TextMesh>().text.Contains("2"))
			{
				timerScript.playerProfiles[(int)currentContractDrafter].twoYearContracts.Add(contract.GetComponentInChildren<InputField>().text);
			}
			// 1 year contract
			else
			{
				timerScript.playerProfiles[(int)currentContractDrafter].oneYearContracts.Add(contract.GetComponentInChildren<InputField>().text);
			}
		}

		otherContractedPlayers.Clear();
		signedPlayers.Clear();
	}

	private string currentLabelText = "Wtf how did this get here?";
	public string GetLabelText()
	{
		if (currentContractPhase == ContractPhaseState.SignContractsPhase)
		{
			currentLabelText = "Signing Open Contracts: " + timerScript.DrafterNames[(int)currentContractDrafter];
		}
		else if(currentContractPhase == ContractPhaseState.BuyOutPhase)
		{
			currentLabelText = "Buy Out Contracts: " + timerScript.DrafterNames[(int)currentContractDrafter];
		}

		return currentLabelText;
	}

	public void BuyOutKeeper(GameObject templateObject)
	{
		bool moveUp = false;

		foreach(GameObject contractObject in otherContractedPlayers)
		{
			if(moveUp)
			{
				contractObject.transform.DOMoveY(contractObject.transform.position.y + contractYValueDifference, animationTime);
			}
			else if(templateObject == contractObject)
			{
				templateObject.GetComponentInChildren<ContractTemplateScript>().AnimateEverythingOut();
				moveUp = true;
			}
		}

		// Accumulate a pick to delay
		if(1 + otherContractedPlayers.Count > 3)
		{
			++timerScript.playerProfiles[(int)currentContractDrafter].picksToDelay;
		}

		otherContractedPlayers.Remove(templateObject);
	}
}
