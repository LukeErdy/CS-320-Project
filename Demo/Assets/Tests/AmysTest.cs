using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AmysTest
{
    #region Examples
    // A Test behaves as an ordinary method
    [Test]
    public void AmysTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AmysTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    #endregion

    [UnityTest]
    public IEnumerator TestEnemySpriteOutsideRadius()
    {
       Player p = GameObject.Find("Player").GetComponent<Player>();
        yield return null;
    }

    [Test]
    public void TestEnemySpriteInsideRadius()
    {

    }

    [Test]
    public void TestEnemyMovementOutsideRadius()
    {

    }

    [Test]
    public void TestEnemyMovementInsideRadius()
    {

    }

    [Test]
    public void TestEnemyDestroyOnLowThreshold()
    {

    }

    public void TestEnemyDestroyOn0HP()
    {

    }

    [Test]
    public void TestPlayerXPOnEnemyDeath()
    {

    }

    [Test]
    public void TestPlayerMeleeAttack()
    {

    }

    [Test]
    public void TestEnemyMeleeAttack()
    {

    }

    [Test]
    public void TestEnemySpawnOnGrassTile()
    {

    }

    [Test]
    public void TestSpawnerNumberEnemies()
    {

    }

    [Test]
    public void TestPlayerRespawn()
    {
        
    }

}
