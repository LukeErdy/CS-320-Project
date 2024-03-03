using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System;

public class GameSession : MonoBehaviour
{
    static String sessionStart;
    static String sessionEnd;
    static bool collectingData;
    static bool inBattle;
    static int playerDeaths;
    static int enemiesKilled;
    // levels completed?

    public static GameSession Instance;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
    }

    public void PlayGame()
    {
        playerDeaths = 0;
        SceneManager.LoadSceneAsync("SampleScene");
        if (PlayerPrefs.GetInt("DataConsent",0) == 1)
        {
            sessionStart = System.DateTime.Now.ToString();
            // start data collection
            UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            AnalyticsService.Instance.RecordEvent("sessionStarted");
        }
        Debug.Log(playerDeaths);
    }

    public void DataCollection()
    {
        // Get data collection consent or player opt out
        SceneManager.LoadSceneAsync("Data Collection");
    }

    public void ConsentGiven()
    {
        PlayerPrefs.SetInt("DataConsent", 1);
        collectingData = true;
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ConsentNotGiven()
    {
        PlayerPrefs.SetInt("DataConsent", 0);
        // stop data collection
        if (collectingData){
            collectingData = false;
            UnityServices.InitializeAsync();
            AnalyticsService.Instance.StopDataCollection();
        }
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void ExitGame()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (PlayerPrefs.GetInt("DataConsent", 0) == 1)
            {
                sessionEnd = System.DateTime.Now.ToString();
                CustomEvent myEvent = new CustomEvent("sessionEnded")
                {
                    {"sessionTimestamp",sessionStart},
                    {"playerDeaths",playerDeaths},
                    {"enemiesKilled",enemiesKilled}
                };
                Debug.Log(playerDeaths);
                AnalyticsService.Instance.RecordEvent(myEvent);
            }
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    private void MovementType()
    {
        if (inBattle){
            if (PlayerPrefs.GetInt("DataConsent",0) == 1){

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
        } 
    }

    public void IncreasePlayerDeath(){
        playerDeaths += 1;
    }

    public void IncreaseEnemiesKilled(){
        enemiesKilled += 1;
    }

    private void trackMovement(String movementType)
    {
        if(PlayerPrefs.GetInt("DataConsent",0) == 1 && inBattle)
        {
            CustomEvent myEvent = new CustomEvent("playerMovement")
            {
                {"movementType",movementType},
                {"sessionTimestamp",sessionStart}
            };
            AnalyticsService.Instance.RecordEvent(myEvent);
        }
    }

    private void Update()
    {
        ExitGame();
        MovementType();
    }
}