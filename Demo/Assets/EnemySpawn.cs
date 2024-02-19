using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    public int minEnemies;
    public int maxEnemies;
    public Generate generatedTerrain;
    public GameObject[] enemyPrefabs;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => generatedTerrain.IsInitialized);
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            throw new System.ArgumentException("You must specify the enemy prefabs.");
        }
        int spawnCount = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            var randomPos = GetSpawnLocation();
            //Debug.Log(randomPos);
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var obj = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            obj.name = $"{enemyPrefab.name}{i+1}";
        }
        //Debug.Log($"Spawned {spawnCount} enemies");
    }

    private Vector3 GetSpawnLocation()
    {
        var grassTiles = generatedTerrain.grassTiles;
        var tilemap = generatedTerrain.tilemap;

        //Select a random grass tile
        var x = Random.Range(0, grassTiles.Length);
        var y = grassTiles[x];
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int cell = new Vector3Int(x, y, 0);

        //Get the center of the random cell
        Vector3 pos = tilemap.GetCellCenterWorld(cell);
        return pos;
    }
}