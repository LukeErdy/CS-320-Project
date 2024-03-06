using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class AmysTest
{
    private Player GetPlayer()
    {
        GameObject obj = GameObject.Find("Player");
        if (obj == null) return null;
        else return obj.GetComponent<Player>();
    }

    //Acceptance Test
    [Test]
    public void TestPlayer()
    {
        Player p = GetPlayer();
        Assert.IsNotNull(p);
    }

    //Testing Enemy.WithinSightRadius(); along with TestEnemySpriteInsideRadius, achieves 100% coverage
    [UnityTest]
    public IEnumerator TestEnemySpriteOutsideRadius()
    {
        //Spawn enemy outside of sight lines of the player
        Player p = GetPlayer();
        GameObject obj = Resources.Load<GameObject>("Enemy");
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX+100, p.posY+100, 0), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Make sure they are not in the attacking sprite mode
        Enemy e = objInst.GetComponent<Enemy>();
        Assert.AreNotEqual(e.attackingSprites, e.currentSprites);
    }

    //Testing Enemy.WithinSightRadius(); along with TestEnemySpriteOutsideRadius, achieves 100% coverage
    [UnityTest]
    public IEnumerator TestEnemySpriteInsideRadius()
    {
        //Spawn enemy inside the sight lines of the player
        Player p = GetPlayer();
        GameObject obj = Resources.Load<GameObject>("Enemy");
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX, p.posY, 0), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Make sure they are in the attacking sprite mode
        Enemy e = objInst.GetComponent<Enemy>();
        Assert.AreEqual(e.attackingSprites, e.currentSprites);  
    }

    //Testing Enemy.Update(); along with TestEnemyMovementInsideRadius, achieves 100% coverage
    [UnityTest]
    public IEnumerator TestEnemyMovementOutsideRadius()
    {
        //Spawn enemy outside of sight lines of the player
        Player p = GetPlayer();
        GameObject obj = Resources.Load<GameObject>("Enemy");
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX + 100, p.posY + 100, 0), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Make sure they are not moving
        Enemy e = objInst.GetComponent<Enemy>();
        Assert.AreEqual(e.rb.velocity, Vector3.zero);
    }

    //Testing Enemy.Update(); along with TestEnemyMovementInsideRadius, achieves 100% coverage
    [UnityTest]
    public IEnumerator TestEnemyMovementInsideRadius()
    {
        //Spawn enemy inside of sight lines of the player
        Player p = GetPlayer();
        GameObject obj = Resources.Load<GameObject>("Enemy");
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX + 100, p.posY + 100, 0), Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Make sure they are moving
        Enemy e = objInst.GetComponent<Enemy>();
        Assert.AreEqual(e.rb.velocity, Vector3.zero);
    }

    //Acceptance Test
    [UnityTest]
    public IEnumerator TestEnemyDestroyOnLowThreshold()
    {
        //Create a test enemy
        GameObject obj = Resources.Load<GameObject>("Enemy");
        obj.name = "TestEnemy";
        GameObject objInst = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Put them into low threshold
        Enemy e = objInst.GetComponent<Enemy>();
        e.rb.position = new Vector2(0, e.lowThreshold);

        //Make sure GameObject was destroyed
        Assert.IsNull(GameObject.Find("TestEnemy"));
    }

    //Acceptance test
    [UnityTest]
    public IEnumerator TestEnemyDestroyOn0HP()
    {
        //Create a test enemy
        GameObject obj = Resources.Load<GameObject>("Enemy");
        obj.name = "TestEnemy";
        GameObject objInst = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Kill them
        Enemy e = objInst.GetComponent<Enemy>();
        e.SetMaxHP(1); 
        e.AdjustHealth(-1);

        yield return new WaitForSeconds(1f);

        //Make sure GameObject was destroyed
        Assert.IsNull(GameObject.Find("TestEnemy"));

    }

    //Integration test - Player and Enemy
    [UnityTest]
    public IEnumerator TestPlayerXPOnEnemyDeath()
    {
        Player p = GetPlayer();
        float currentXP = p.currentXP;

        //Create a test enemy
        GameObject obj = Resources.Load<GameObject>("Enemy");
        obj.name = "TestEnemy";
        GameObject objInst = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        //Kill them
        Enemy e = objInst.GetComponent<Enemy>();
        e.SetMaxHP(1);
        e.AdjustHealth(-1);

        yield return new WaitForSeconds(1f);

        //Make sure Player got XP
        Assert.Greater(currentXP, p.currentXP); 
    }

    //Integration test - Player and Enemy
    [UnityTest]
    public IEnumerator TestPlayerMeleeAttack()
    {
        Player p = GetPlayer();
        //Create a test enemy
        GameObject obj = Resources.Load<GameObject>("Enemy");
        obj.name = "TestEnemy";
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX, p.posY, 0), Quaternion.identity);
        Enemy e = objInst.GetComponent<Enemy>();
        //TODO: proc "Q"
        yield return null;
        //Make sure enemy got damaged
        Assert.Less(e.currentHP, e.maxHP);
    }

    //Integration test - Player and Enemy
    [UnityTest]
    public IEnumerator TestEnemyMeleeAttack()
    {
        Player p = GetPlayer();
        //Create a test enemy
        GameObject obj = Resources.Load<GameObject>("Enemy");
        obj.name = "TestEnemy";
        GameObject objInst = GameObject.Instantiate(obj, new Vector3(p.posX, p.posY, 0), Quaternion.identity);

        yield return new WaitForSeconds(1);

        //Make sure Player got damaged
        Assert.Less(p.currentHP, p.maxHP);
    }

    //Integration test - Enemy, EnemySpawn, and Generate
    [UnityTest]
    public IEnumerator TestEnemySpawnOnGrassTile()
    {
        yield return null;
    }
    
    //Acceptance test
    [UnityTest]
    public IEnumerator TestSpawnerNumberEnemies()
    {
        yield return null;
    }

    //Acceptance test
    [UnityTest]
    public IEnumerator TestPlayerRespawn()
    {
        //Put player into low threshold
        Player p = GetPlayer();
        Vector2 oldPos = p.rb.position;
        p.rb.position = new Vector2(0, p.lowThreshold);

        yield return new WaitForSeconds(1f);

        //Make sure they died
        Assert.AreNotEqual(p.rb.position, oldPos);
    }

}
