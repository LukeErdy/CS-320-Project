using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class DataConsent : MonoBehaviour
{ 
   public void Consent()
    {
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        SceneManager.LoadSceneAsync("Main Menu");
    }
   public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    
}
