using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawn : MonoBehaviour
{
    public int minEnemies = 0;
    public int maxEnemies = 1;
    public Tilemap spawnableTerrain;
    public Tilemap unspawnableTerrain;
    public GameObject[] enemyPrefabs;

    private void Start()
    {
        if(enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            throw new System.ArgumentException("You must specify the enemy prefabs.");
        }
        int spawnCount = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < spawnCount; i++)
        {
            var randomPos = GetSpawnLocation();
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var obj = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            obj.name = $"{enemyPrefab.name}{i+1}";
        }
    }

    private Vector3 GetSpawnLocation()
    {
        BoundsInt bounds = spawnableTerrain.cellBounds;
        //Debug.Log($"SPAWNABLE xMin: {bounds.xMin}, xMax: {bounds.xMax} yMin: {bounds.yMin} yMax: {bounds.yMax}");

        //BoundsInt dontSpawn = unspawnableTerrain.cellBounds;
        //Debug.Log($"UNSPAWNABLE xMin: {bounds.xMin}, xMax: {bounds.xMax} yMin: {bounds.yMin} yMax: {bounds.yMax}");

        // Generate a random position within the bounds
        Vector3Int randomCell = new Vector3Int(
            Random.Range(bounds.xMin, bounds.xMax),
            Random.Range(bounds.yMin, bounds.yMax),
            0
        );

        //https://docs.unity3d.com/ScriptReference/Tilemaps.Tilemap.html

        // Get the center of the random cell
        Vector3 pos = spawnableTerrain.GetCellCenterWorld(randomCell);

        //Don't choose a position in the unspawnable terrain
        if (unspawnableTerrain.GetTile(unspawnableTerrain.WorldToCell(pos))) return GetSpawnLocation();
        return pos;
    }
}