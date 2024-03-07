using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DataCollectionTests
{
    [Test] // Acceptance test
    public void TestSessionStart()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();

        // given consent and start of game session timestamp is tracked
        gs.ConsentGiven();
        gs.PlayGame();
        Assert.IsNotNull(gs.GetTimestamp("start"));

        // consent not given and start of game session timestamp is not tracked
        gs.ConsentNotGiven();
        gs.PlayGame();
        Assert.IsNull(gs.GetTimestamp("start"));
    }

    [Test] // Acceptance test
    public void TestSessionEnd()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();

        // given consent and start of game session timestamp is tracked
        gs.ConsentGiven();
        gs.PlayGame();
        gs.ExitGame();
        Assert.IsNotNull(gs.GetTimestamp("end"));

        // consent not given and start of game session timestamp is not tracked
        gs.Reset();
        gs.ConsentNotGiven();
        gs.ExitGame();
        Assert.IsNull(gs.GetTimestamp("end"));
    }

    [Test] //Player and GameSession units being tested using Big-Bang
    public void TestPlayerDeathsCount()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        int beginning = gs.GetCount("deaths");
        Player player = obj.AddComponent<Player>();
        player.AdjustXP(-30);
        player.CheckHealth();
        Assert.AreEqual(beginning+1, gs.GetCount("deaths"));
    }

    [Test] //Enemy and GameSession units being tested using Big-Bang
    public void TestEnemyKilledCount()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        int beginning = gs.GetCount("killed");
        Enemy enemy = obj.AddComponent<Enemy>();
        enemy.Die();
        Assert.AreEqual(beginning+1, gs.GetCount("killed"));
    }

    [Test] // Acceptance test
    public void TestEnterBattle()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        Assert.AreEqual(gs.GetStatus("inBattle"),false);
        gs.BattleStatus(true);
        Assert.AreEqual(gs.GetStatus("inBattle"),true);
    }

    [Test] // Acceptance test
    public void TestExitBattle()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        Assert.AreEqual(gs.GetStatus("inBattle"),false);
        gs.BattleStatus(true);
        Assert.AreEqual(gs.GetStatus("inBattle"),true);
        gs.BattleStatus(false);
        Assert.AreEqual(gs.GetStatus("inBattle"),false);
    }

    [Test] // Acceptance test
    public void TestDataConsent()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();

        // given consent and ensure player preferences updated
        gs.ConsentGiven();
        Assert.AreEqual(true, gs.GetStatus("collectingData"));

        //consent not given ensure player preferences updated
        gs.ConsentNotGiven();
        Assert.AreEqual(false, gs.GetStatus("collectingData"));
    }

    [Test] // Acceptance test
    public void TestDataPersistence()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        gs.ConsentGiven();
        gs.IncreaseEnemiesKilled();
        gs.IncreasePlayerDeath();

        GameSession newgs = new GameSession(); 
        Assert.AreEqual(gs.GetCount("deaths"),newgs.GetCount("deaths"));
        Assert.AreEqual(gs.GetCount("killed"),newgs.GetCount("killed"));
        Assert.AreEqual(gs.GetStatus("collectingData"),newgs.GetStatus("collectingData"));

    }

    [Test] // Acceptance test
    public void TestResetFields()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        gs.ConsentGiven();
        gs.IncreaseEnemiesKilled();
        gs.IncreasePlayerDeath();
        Assert.AreEqual(gs.GetCount("deaths"),1);
        Assert.AreEqual(gs.GetCount("killed"),1);

        gs.Reset();
        Assert.AreEqual(gs.GetCount("deaths"),0);
        Assert.AreEqual(gs.GetCount("killed"),0);
    }

    //  Along with TestGetCountKills() and TestGetCountInvalidInput() 100% coverage 
    /* 
     public int GetCount(String field){
        switch (field){
            case "deaths": return playerDeaths;
            case "killed": return enemiesKilled;
            default: return -1;
        }
    }
    */
    [Test]
    public void TestGetCountDeaths()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();

        Assert.AreEqual(gs.GetCount("deaths"),0);
        gs.IncreasePlayerDeath();
        Assert.AreEqual(gs.GetCount("deaths"),1);
    }

    [Test]
    public void TestGetCountKills()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();

        Assert.AreEqual(gs.GetCount("killed"),0);
        gs.IncreaseEnemiesKilled();
        Assert.AreEqual(gs.GetCount("killed"),1);
    }

    [Test]
    public void TestGetCountInvalidInput()
    {
        GameObject obj = new GameObject();
        GameSession gs = obj.AddComponent<GameSession>();
        gs.Reset();
        Assert.AreEqual(gs.GetCount("sessions"),-1);
    }
}