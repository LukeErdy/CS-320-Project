using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System;

public class GameSession : MonoBehaviour
{
    String sessionStart;
    // enemies killed
    // playerDeaths
    // levels completed?

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("DataConsent",0) == 1)
        {
            sessionStart = System.DateTime.Now.ToString();
            // start data collection
            UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            AnalyticsService.Instance.RecordEvent("sessionStarted");

            // to send the event immediately
            AnalyticsService.Instance.Flush();
        }
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void DataCollection()
    {
        // Get data collection consent or player opt out
        SceneManager.LoadSceneAsync("Data Collection");
    }

    public void ConsentGiven()
    {
        PlayerPrefs.SetInt("DataConsent", 1);
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ConsentNotGiven()
    {
        PlayerPrefs.SetInt("DataConsent", 0);
        // stop data collection
        AnalyticsService.Instance.StopDataCollection();
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void ExitGame()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (PlayerPrefs.GetInt("DataConsent", 0) == 1)
            {
                CustomEvent myEvent = new CustomEvent("sessionEnded")
            {
                {"sessionTimestamp",sessionStart}
                // also want to track enemies killed and playerDeaths in session
            };
                AnalyticsService.Instance.RecordEvent(myEvent);
            }
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    private void MovementType()
    {
        if (Input.GetKeyDown("left"))
        {
            // backward movement
            trackMovement("backwards");
        }
        if (Input.GetKeyDown("right"))
        {
            // forward movement
            trackMovement("forwards");
        }
        if (Input.GetButtonDown("Jump"))
        {
            // jump movement
            trackMovement("jump");
        }
        if (Input.GetKeyDown("q"))
        {
            // attack movement
            trackMovement("attack");
        }
        if (Input.GetButtonDown("Jump") && Input.GetKeyDown("right"))
        {
            // jump forward
            trackMovement("jump forwards");
        }
        if (Input.GetButtonDown("Jump") && Input.GetKeyDown("left"))
        {
            // jump backwards
            trackMovement("jump backwards");
        }
    }

    private void trackMovement(String movementType)
    {
        CustomEvent myEvent = new CustomEvent("playerMovement")
            {
                {"movementType",movementType},
                {"sessionTimestamp",sessionStart}
            };
        AnalyticsService.Instance.RecordEvent(myEvent);

    }

    private void Update()
    {
        ExitGame();
        MovementType();
    }
}