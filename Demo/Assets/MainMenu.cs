using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // need to call method on player data object to track start session time (if data consent given)
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void DataCollection()
    {
        // need to create a new dataConsent instance and then see if data consent is given
        SceneManager.LoadSceneAsync("Data Collection");
    }  
}