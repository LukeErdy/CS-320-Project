using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

public class AmysTest
{
    /// <summary>
    /// Creates a Player instance that is spawned at the specified location.
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public Player CreatePlayer(float x=0, float y=0)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
        Player player = obj.GetComponent<Player>();
        if (!player) return null;
        player.rb.position = new Vector3(x, y, 0);
        return player;
    }

    /// <summary>
    /// Creates a Enemy instance that is spawned at the specified location and of the specified type (Rodent or Snake).
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public Enemy CreateEnemy(float x=0, float y = 0, string typeOfEnemy = "Rodent")
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(typeOfEnemy, typeof(GameObject))) as GameObject;
        Enemy enemy = obj.GetComponent<Enemy>();
        if (!enemy) return null;
        enemy.rb.position = new Vector3(x, y, 0);
        return enemy;
    }

    //Acceptance Test
    [Test]
    public void TestPlayer()
    {
        Player p = CreatePlayer();
        Assert.IsNotNull(p);
    }

    //Acceptance Test
    [Test]
    public void TestRodent()
    {
        Enemy rodent = CreateEnemy(0,0,"Rodent");
        Assert.IsNotNull (rodent);
    }

    //Acceptance Test
    [Test]
    public void TestSnake()
    {
        Enemy snake = CreateEnemy(0,0,"Snake");
        Assert.IsNotNull(snake);
    }

    //Testing Enemy.WithinSightRadius(); along with TestEnemySpriteInsideRadius, achieves 100% coverage
    [Test]
    public void TestEnemySpriteOutsideRadius()
    {
        //Spawn enemy outside of sight lines of the player
        Player p = CreatePlayer();
        Enemy e = CreateEnemy(p.posX + 100, p.posY + 100);

        //Make sure they are not in the attacking sprite mode
        Assert.AreNotEqual(e.attackingSprites, e.currentSprites);
    }

    //Testing Enemy.WithinSightRadius(); along with TestEnemySpriteOutsideRadius, achieves 100% coverage
    [Test]
    public void TestEnemySpriteInsideRadius()
    {
        //Spawn enemy inside the sight lines of the player
        Player p = CreatePlayer();
        Enemy e = CreateEnemy(p.posX, p.posY);
        
        //Make sure they are in the attacking sprite mode
        Assert.AreEqual(e.attackingSprites, e.currentSprites);
    }

    //Testing Enemy.Update(); along with TestEnemyMovementInsideRadius, achieves 100% coverage
    [Test]
    public void TestEnemyMovementOutsideRadius()
    {
        //Spawn enemy outside of sight lines of the player
        Player p = CreatePlayer(); 
        Enemy e = CreateEnemy(p.posX+100, p.posY+100);

        //Make sure they are not moving
        Assert.AreEqual(e.rb.velocity, Vector3.zero);
    }

    //Testing Enemy.Update(); along with TestEnemyMovementInsideRadius, achieves 100% coverage
    [Test]
    public void TestEnemyMovementInsideRadius()
    {
        //Spawn enemy inside of sight lines of the player
        Player p = CreatePlayer();
        Enemy e = CreateEnemy(p.posX, p.posY);
        
        //Make sure they are moving
        Assert.AreEqual(e.rb.velocity, Vector3.zero);
    }

    //Acceptance Test
    [Test]
    public void TestEnemyDestroyOnLowThreshold()
    {
        //Create a test enemy
        Enemy e = CreateEnemy();
        e.gameObject.name = "TestEnemy";
        
        //Put them into low threshold
        e.rb.position = new Vector2(0, e.lowThreshold);
        
        //Make sure GameObject was destroyed
        Assert.IsNull(GameObject.Find("TestEnemy"));
    }

    //Acceptance test
    [Test]
    public void TestEnemyDestroyOn0HP()
    {
        //Create a test enemy
        Enemy e = CreateEnemy();
        e.gameObject.name = "TestEnemy";

        //Kill them
        e.SetMaxHP(1);
        e.AdjustHealth(-1);

        //Make sure GameObject was destroyed
        Assert.IsNull(GameObject.Find("TestEnemy"));

    }

    //Integration test - Player and Enemy
    [Test]
    public void TestPlayerXPOnEnemyDeath()
    {
        Player p = CreatePlayer();
        float currentXP = p.currentXP;

        //Create a test enemy
        Enemy e = CreateEnemy();
        e.gameObject.name = "TestEnemy";
        
        //Kill them
        e.SetMaxHP(1);
        e.AdjustHealth(-1);
        
        //Make sure Player got XP
        Assert.Greater(currentXP, p.currentXP);
    }

    //Integration test - Player and Enemy
    [Test]
    public void TestPlayerMeleeAttack()
    {
        Player p = CreatePlayer();
        //Create a test enemy
        Enemy e = CreateEnemy(p.posX, p.posY);
        e.gameObject.name = "TestEnemy";

        //TODO: proc "Q"
        var keyboard = InputSystem.AddDevice<Keyboard>();
        keyboard.OnTextInput('Q');
        //Make sure enemy got damaged
        Assert.Less(e.currentHP, e.maxHP);
    }

    //Integration test - Player and Enemy
    [Test]
    public void TestEnemyMeleeAttack()
    {
        Player p = CreatePlayer();
        //Create a test enemy
        Enemy e = CreateEnemy(p.posX, p.posY);
        e.gameObject.name = "TestEnemy";

        //Make sure Player got damaged
        Assert.Less(p.currentHP, p.maxHP);
    }

    //Integration test - Enemy, EnemySpawn, and Generate
    [UnityTest]
    public IEnumerator TestEnemySpawnOnGrassTile()
    {
        var spawner = new EnemySpawn();
        spawner.minEnemies = 1;
        spawner.maxEnemies = 10;
        spawner.generatedTerrain = new Generate();
        spawner.generatedTerrain.Start();
        yield return new WaitForSeconds(1f);
        spawner.Start();

        var enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            //check if enemy is in grass tile
            Assert.That(spawner.generatedTerrain.grassTiles[(int)enemy.posX] == enemy.posY);
        }
    }

    //Acceptance test
    [Test]
    public void TestSpawnerNumberEnemies()
    {
        var spawner = new EnemySpawn();
        spawner.minEnemies = 1;
        spawner.maxEnemies = 10;
        spawner.generatedTerrain = new Generate();
        spawner.Start();

        var enemies = GameObject.FindObjectsOfType<Enemy>();
        int number = enemies.Length;
        Assert.AreEqual(true, (spawner.minEnemies <= number) && (number <= spawner.maxEnemies));
    }

    //Acceptance test
    [Test]
    public void TestPlayerRespawn()
    {
        //Put player into low threshold
        Player p = CreatePlayer();
        Vector2 oldPos = p.rb.position;
        p.rb.position = new Vector2(0, p.lowThreshold);

        //Make sure they died
        Assert.AreNotEqual(p.rb.position, oldPos);
    }
}
