using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exitgame : MonoBehaviour
{
   public void ExitGame()
    {
        // need to call on player data object to collect end session time (if data consent was given)
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
