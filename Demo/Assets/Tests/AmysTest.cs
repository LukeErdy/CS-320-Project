using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class AmysTest
{
    #region Examples
    /*
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
    }*/
    #endregion

    [Test]
    public void TestPlayer()
    {
        Player p = GameObject.Find("Player").GetComponent<Player>();
        Assert.IsNotNull(p);
    }


    [UnityTest]
    public IEnumerator TestEnemySpriteOutsideRadius()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestEnemySpriteInsideRadius()
    {
        Player p = GameObject.Find("Player").GetComponent<Player>();
        GameObject enemy = Resources.Load<GameObject>("Enemy");
        GameObject enemyInstance = GameObject.Instantiate(enemy, new Vector3(p.posX, p.posY, 0), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(enemyInstance.GetComponent<Enemy>().attackingSprites, enemyInstance.GetComponent<Enemy>().currentSprites);

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
        Player p = GameObject.Find("Player").GetComponent<Player>();
    }

    [Test]
    public void TestPlayerMeleeAttack()
    {

        Player p = GameObject.Find("Player").GetComponent<Player>();
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

        Player p = GameObject.Find("Player").GetComponent<Player>();
    }

}
