using UnityEngine;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class DataConsent : MonoBehaviour
{
    async void Start()
    {
		await UnityServices.InitializeAsync();

		AskForConsent();
    }

	void AskForConsent()
	{
		// show the player a UI element that asks for consent.

		// if consent given take down UI element and call on ConsentGiven

		// else take down UI element and do not collect data 
	}

	void ConsentGiven()
	{
		AnalyticsService.Instance.StartDataCollection();
	}
}
