using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Generate : MonoBehaviour
{
    const int MIN_X = -10;
    const int MAX_X = 60;
    const int MIN_Y = -22;
    const int MAX_Y = 19;

    //TODO replace this method 2 from https://stackoverflow.com/a/56604959
    public Tilemap tilemap;
    public Tile grass_1;
    public Tile sky_1;
    public Tile testtile_1;

    // Start is called before the first frame update
    void Start()
    {
        //Create 2D array of bytes that will be converted into the tilemap
        const int xDim = MAX_X-MIN_X;
        const int yDim = MAX_Y-MIN_Y;
        byte[,] map = new byte[xDim, yDim];

        //Draw a box around the map
        for(int i = 0; i < xDim; i++){
            map[i,0] = 1;
            map[i,yDim-1] = 1;
        }
        for(int j = 0; j < yDim; j++){
            map[0,j] = 1;
            map[xDim-1,j] = 1;
        }

        //Convert 2D array into tilemap
        for(int i = 0; i < xDim; i++){
            for(int j = 0; j < yDim; j++){
                if(map[i,j] == 1) tilemap.SetTile(new Vector3Int(i+MIN_X, j+MIN_Y, 0), testtile_1);
            }
        }




        // Debug.Log("hello world");
        
        // Vector3Int loc = new Vector3Int(0,0,0);
        // tilemap.SetTile(loc, grass_1);

        //Test code taken from https://stackoverflow.com/a/56604959
        // for(int i = 0; i < 20; i++){
        //     for(int j = 0; j < 20; j++){
        //         bool odd = ((i+j)%2 == 1);
        //         tilemap.SetTile(new Vector3Int(i,j,0), odd ? grass_1 : sky_1);
        //     }
        // }

        // for(int i = MIN_X; i < MAX_X; i++){
            // for(int j = -22; j < 19; j++){
                // tilemap.SetTile(new Vector3Int(i,-2,0), testtile_1);
                // tilemap.SetTile(new Vector3Int(0,j,0), testtile_1);
            // }
        // } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
