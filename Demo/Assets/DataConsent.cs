using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class DataConsent : MonoBehaviour
{ 
    public bool consent;
   public void Consent()
    {
        consent = true;
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        SceneManager.LoadSceneAsync("Main Menu");
    }
   public void MainMenu()
    {
        consent = false;
        SceneManager.LoadSceneAsync("Main Menu");
    }
    
}
