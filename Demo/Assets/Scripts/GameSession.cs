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
        Reset();
        SceneManager.LoadSceneAsync("SampleScene");
        if (PlayerPrefs.GetInt("DataConsent",0) == 1)
        {
            sessionStart = System.DateTime.Now.ToString();
            AnalyticsService.Instance.RecordEvent("sessionStarted");
        }
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
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
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

    public void ExitGame()
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
            AnalyticsService.Instance.RecordEvent(myEvent);
        }
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void MovementType()
    {
        if (inBattle){
            if (PlayerPrefs.GetInt("DataConsent",0) == 1){

                if (Input.GetKeyDown("left"))
                {
                    // backward movement
                    TrackMovement("backwards");
                }
                if (Input.GetKeyDown("right"))
                {
                    // forward movement
                    TrackMovement("forwards");
                }
                if (Input.GetButtonDown("Jump"))
                {
                    // jump movement
                    TrackMovement("jump");
                }
                if (Input.GetKeyDown("q"))
                {
                    // attack movement
                    TrackMovement("attack");
                }
                if (Input.GetButtonDown("Jump") && Input.GetKeyDown("right"))
                {
                    // jump forward
                    TrackMovement("jump forwards");
                }
                if (Input.GetButtonDown("Jump") && Input.GetKeyDown("left"))
                {
                    // jump backwards
                    TrackMovement("jump backwards");
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

    public void BattleStatus(bool state){
        inBattle = state;
    }

    public bool GetStatus(String field){
        switch (field){
            case "collectingData": return (PlayerPrefs.GetInt("DataConsent",0) == 1);
            case "inBattle": return inBattle; 
            default: return false;
        }
    }

    public String GetTimestamp(String field){
        switch (field){
            case "start": return sessionStart;
            case "end": return sessionEnd;
            default: return null;
        }
    }

    public int GetCount(String field){
        switch (field){
            case "deaths": return playerDeaths;
            case "killed": return enemiesKilled;
            default: return -1;
        }
    }

    public void Reset(){
        sessionStart = null;
        sessionEnd = null;
        inBattle = false;
        playerDeaths = 0;
        enemiesKilled = 0;
    }

    public void TrackMovement(String movementType)
    {
        if(collectingData && inBattle)
        {
            switch (movementType){
                case "backwards": break;
                case "forwards": break;
                case "jump backwards": break;
                case "jump forwards": break;
                case "jump": break;
                case "attack": break;
                default: return;
            }
            try {
                CustomEvent myEvent = new CustomEvent("playerMovement")
                {
                    {"movementType",movementType},
                    {"sessionTimestamp",sessionStart}
                };
                AnalyticsService.Instance.RecordEvent(myEvent);
            } catch {
            }
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)){
            ExitGame();
        }
        
        MovementType();
    }
}