using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.Analytics;
using System;


public class GameSession : MonoBehaviour
{
    //private PlayerData pd;
    private void Start()
    {
        PlayerPrefs.SetInt("DataConsent", 0);
    }

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("DataConsent",0) == 1)
        {
            Debug.Log("CONSENT GIVEN");
            String sessionStart = DateTime.Now.ToString();
            Analytics.CustomEvent("gameStarted",new Dictionary<string, object> { { "Start of Session", sessionStart } });
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
        // set dataConsent to true
        PlayerPrefs.SetInt("DataConsent", 1);
        // init unity analytics
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ConsentNotGiven()
    {
        PlayerPrefs.SetInt("DataConsent", 0);
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void ExitGame()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (PlayerPrefs.GetInt("DataConsent", 0) == 1)
            {
                String sessionEnd = DateTime.Now.ToString();
                Analytics.CustomEvent("gameEnded", new Dictionary<string, object> { { "End of Session", sessionEnd } });
            }
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    private void Update()
    {
        ExitGame();
    }
}