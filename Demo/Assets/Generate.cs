//Comment this line to disable level gen and revert to original level
#define DO_GENERATE

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Generate : MonoBehaviour
{
    const int MIN_X = -10;
    const int MAX_X = 60;
    const int MIN_Y = -22;
    const int MAX_Y = 19;

    const int MAX_AMP = 10;
    const int MIN_AMP = 3;
    const int MAX_PER = 10;
    const int MIN_PER = 5;

    //TODO replace this method 2 from https://stackoverflow.com/a/56604959
    public Tilemap tilemap;
    //[0] = null; [1] = grass; [2] = dirt; [3] = sky; [4] = testtile
    public Tile[] tiles = new Tile[5];

    // Start is called before the first frame update
    void Start()
    {
        #if DO_GENERATE
        //TODO: Set player position above ground level

        //Create 2D array of bytes that will be converted into the tilemap
        //Position (0,0) corresponds to the bottom left corner in-game
        const int xDim = MAX_X-MIN_X;
        const int yDim = MAX_Y-MIN_Y;
        byte[,] map = new byte[xDim, yDim];
        tiles[0] = null;

        
        //Generates a grassy, hilly terrain with dirt beneath it
        //TODO: Make sure hills don't go out the top or bottom of map
        int startY = 20, width, amplitude;
        bool startBool = false;
        var rand = new System.Random();

        for(int i = 0; i < xDim-1; i += width+1){
            width = rand.Next(MIN_PER, MAX_PER);
            amplitude = rand.Next(MIN_AMP, MAX_AMP);

            var nextHill = new Hill(width, amplitude, startBool);
            for(int j = 0; j <= width; j++){
                if(i+j >= xDim-1) break;
                int colH = startY+nextHill.getY(j);
                map[i+j, colH] = 1;
                for(int k = colH-1; k >= 0; k--) map[i+j,k] = 2;
            }

            startY += nextHill.getY(width);
            startBool = !startBool;
        }

        //Convert 2D array into tilemap
        //TODO: Convert values to enum
        for(int i = 0; i < xDim; i++){
            for(int j = 0; j < yDim; j++){
                tilemap.SetTile(new Vector3Int(i+MIN_X, j+MIN_Y, 0), tiles[map[i,j]]);
            }
        }
        #endif
    }

    // Update is called once per frame
    void Update(){}
}


class Hill{
    float w;
    float a;
    bool inv;

    public Hill(int width, int amplitude, bool invert){
        w = width;
        a = amplitude;
        inv = invert;
    }

    public int getY(int x){
        int output = (int)((a/2) * (Math.Sin((Math.PI/w)*(x - (w/2))) + 1));
        if(inv) return (int)(-output);
        else return output;
    }
}