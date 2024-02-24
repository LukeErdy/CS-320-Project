using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class GameSession : MonoBehaviour
{
    private bool data_consent = false;
    //private PlayerData pd;

    public void PlayGame()
    {
        //TODO: need to track start session time (if data consent given)
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void DataCollection()
    {
        // Get data collection consent or player opt out
        SceneManager.LoadSceneAsync("Data Collection");
    }

    public void ConsentGiven()
    {
        data_consent = true;
        // init unity analytics
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ConsentNotGiven()
    {
        data_consent = false;
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public bool GetConsentStatus()
    {
        return data_consent;
    }

    private void ExitGame()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //TODO: need to track end session time (if data consent was given)
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    private void Update()
    {
        ExitGame();
    }
}