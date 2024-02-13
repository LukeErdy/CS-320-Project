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
    //[0] = null; [1] = grass; [2] = dirt; [3] = stone; [4] = gravel
    public Tile[] tiles = new Tile[6];

    // Start is called before the first frame update
    void Start()
    {
        #if DO_GENERATE
        //TODO: Set player position above ground level

        //Create 2D array of bytes that will be converted into the tilemap
        //Position (0,0) corresponds to the bottom left corner in-game
        const int xDim = MAX_X-MIN_X;
        const int yDim = MAX_Y-MIN_Y;
        Tl[,] map = new Tl[xDim, yDim];
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
                //FIXME: Got an index OOB error once
                map[i+j, colH] = Tl.grass_0;
                for(int k = colH-1; k > colH-5; k--) map[i+j,k] = Tl.dirt_0;
                map[i+j,colH-5] = Tl.gravel_0;
                for(int k = colH-6; k >= 0; k--) map[i+j,k] = Tl.stone_0;
            }

            startY += nextHill.getY(width);
            startBool = !startBool;
        }

        //Convert 2D array into tilemap
        for(int i = 0; i < xDim; i++){
            for(int j = 0; j < yDim; j++){
                tilemap.SetTile(new Vector3Int(i+MIN_X, j+MIN_Y, 0), tiles[(int)map[i,j]]);
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


enum Tl{ empty, grass_0, dirt_0, stone_0, gravel_0 }


//A WFCRule indicates all the possible tiles that can be placed in
//the x,y position relative to the current tile
//TODO: implement weights for tiles
class WFCRule{
    int x, y;
    HashSet<Tl> tiles;
    public WFCRule(int xIn, int yIn, HashSet<Tl> tilesIn){ x=xIn; y=yIn; tiles=tilesIn; }
}


class WFCQueue{
    int[,] indices;
    int size;
    (int x, int y, HashSet<Tl> tiles)[] queue;

    public WFCQueue(int x, int y){
        indices = new int[x,y];
        queue = new (int x, int y, HashSet<Tl> tiles)[x*y];
        size = queue.Length;
        //Initialize the queue with each tile being able to be any of
        //the possible types and the indices pointing to the correct
        //location in the queue
        for(int i = 0; i < x; i++){
            for(int j = 0; j < y; j++){
                indices[i,j] = i*y + j;
                queue[indices[i,j]] = (i, j, new HashSet<Tl>(new Tl[] 
                    {Tl.empty, Tl.grass_0, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}));
            }
        }
    }

    void swap(int ind1, int ind2){
        (int x, int y, HashSet<Tl> tiles) temp = queue[ind1];
        queue[ind1] = queue[ind2];
        queue[ind2] = temp;
        indices[queue[ind1].x, queue[ind1].y] = ind1;
        indices[queue[ind2].x, queue[ind2].y] = ind2;
    }

    public (int x, int y, HashSet<Tl> tiles) pop(){
        (int x, int y, HashSet<Tl> tiles) output = queue[0];
        queue[0] = queue[size];

        int i = 0;
        while(true){
            int lCh = (2*i) + 1;
            int rCh = (2*i) + 2;
            if((lCh >= size || queue[i].tiles.Count <= queue[lCh].tiles.Count) &&
                (rCh >= size || queue[i].tiles.Count <= queue[rCh].tiles.Count)){
                break;
            } else if(queue[i].tiles.Count > queue[lCh].tiles.Count){
                swap(i, lCh);
                i = lCh;
            } else if(queue[i].tiles.Count > queue[rCh].tiles.Count){
                swap(i, rCh);
                i = rCh;
            } else Debug.Log("WFCQueue.pop: Should not reach this control path");
        }
        size--;
        return output;
    }

    public void update(int x, int y, HashSet<Tl> intersect){
        int i = indices[x,y];
        queue[i].tiles.IntersectWith(intersect);

        while(true){
            int par = (i-1)/2;
            if(par < 0 || queue[i].tiles.Count >= queue[par].tiles.Count) break;
            else{
                swap(i, par);
                i = par;
            }
        }
    }

    public void print(){
        for(int i = 0; i < size; i++){
            Debug.Log("x: " + queue[i].x + " y: " + queue[i].y + " Tiles: " + queue[i].tiles.Count);
        }
    }
}


class WaveFuncColl{
    Tl[,] map;
    WFCRule[][] rules;

    public WaveFuncColl(int x, int y){ 
        map = new Tl[x,y];
        //Manually specifying rules for now
        rules = new WFCRule[Enum.GetNames(typeof(Tl)).Length][];
        rules[(int)Tl.grass_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.empty})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0, Tl.gravel_0}))
        };
        rules[(int)Tl.dirt_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0, Tl.gravel_0}))
        };
        rules[(int)Tl.stone_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.stone_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0}))
        };
        rules[(int)Tl.gravel_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}))
        };
    }

    public void run(){
        return;
    }


}