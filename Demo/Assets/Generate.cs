//Comment these defines to disable level gen and revert to original level
//#define OLD_GEN
//#define WFC_GEN

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//TODO: Set player position above ground level


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
        const int xDim = MAX_X-MIN_X;
        const int yDim = MAX_Y-MIN_Y;


        #if WFC_GEN
        int testX = xDim-1, testY = yDim;
        WaveFuncColl test = new WaveFuncColl(testX,testY);

        //Generate a horizontal line of grass
        // for(int i = 0; i < testX; i++) test.set(i, 30, Tl.grass_0);

        //Generate Grassy Hills
        //FIXME: Some empty tiles with high amplitude (~30)
        int startY=yDim-20, width=0, wCount=0, currH, prevH=-1, amplitude;
        bool goDown = true;
        var rand = new System.Random();
        Hill nextHill = null;

        for(int i = 0; i < xDim-1; i++){
            if(wCount == width){
                goDown = !goDown;
                if(prevH != -1) startY += nextHill.getY(width);
                width = rand.Next(MIN_PER, MAX_PER);
                amplitude = rand.Next(MIN_AMP, MAX_AMP);
                nextHill = new Hill(width, amplitude, goDown);
                wCount = 0;
            }
            currH = startY + nextHill.getY(wCount);
            if(currH >= testY) currH = testY-1;
            else if(currH < 0) currH = 0;

            if(prevH != -1){
                if(currH < prevH){
                    for(int j = 0; j > currH-prevH; j--){
                        test.set(i, prevH+j, Tl.empty);
                        test.set(i-1, prevH+j-1, Tl.dirt_0);
                    }
                } else {
                    for(int j = 0; j < currH-prevH; j++){
                        test.set(i, prevH+j, Tl.dirt_0);
                        //FIXME: Goes out of bounds at the top sometimes
                        test.set(i-1, prevH+j+1, Tl.empty);
                    }
                }
            }
            test.set(i, currH, Tl.grass_0);
            prevH = currH;
            wCount++;
        }
        
        test.run();

        //Write tiles to the game's tilemap
        for(int i = 0; i < testX; i++){
            for(int j = 0; j < testY; j++){
                tilemap.SetTile(new Vector3Int(i+MIN_X, j+MIN_Y, 0), tiles[(int)test.map[i,j]]);
            }
        }
        #endif


        #if OLD_GEN
        //Create 2D array of bytes that will be converted into the tilemap
        //Position (0,0) corresponds to the bottom left corner in-game
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

//Uses a sin wave to produce realistic-ish hills
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


enum Tl{ empty, grass_0, dirt_0, stone_0, gravel_0, error_0 }


//A WFCRule indicates all the possible tiles that can be placed in
//the x,y position relative to the current tile
//TODO: implement weights for tiles
class WFCRule{
    public int x, y;
    public HashSet<Tl> tiles;
    public WFCRule(int xIn, int yIn, HashSet<Tl> tilesIn){ x=xIn; y=yIn; tiles=tilesIn; }
}


class WFCQueue{
    int[,] indices;
    int size; //Actual size minus one
    (int x, int y, HashSet<Tl> tiles)[] queue;

    public WFCQueue(int x, int y){
        indices = new int[x,y];
        queue = new (int x, int y, HashSet<Tl> tiles)[x*y];
        size = queue.Length-1;

        //Initialize the queue with each tile being able to be any of
        //the possible types and the indices pointing to the correct
        //location in the queue
        for(int i = 0; i < x; i++){
            for(int j = 0; j < y; j++){
                indices[i,j] = i*y + j;
                queue[indices[i,j]] = (i, j, new HashSet<Tl>(new Tl[] 
                    {Tl.empty, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}));
            }
        }
    }

    //Swaps items in the queue and updates their indices in the table
    void swap(int ind1, int ind2){
        var temp = queue[ind1];
        queue[ind1] = queue[ind2];
        queue[ind2] = temp;
        indices[queue[ind1].x, queue[ind1].y] = ind1;
        indices[queue[ind2].x, queue[ind2].y] = ind2;
    }

    public (int x, int y, HashSet<Tl> tiles) pop(){
        var output = queue[0];
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
        indices[output.x, output.y] = -1;
        size--;
        return output;
    }

    //Updates the possible tile type a specific tile can be by doing
    //a set intersection between the current list of tiles and the
    //input set, then moves that item up the queue if necessary
    public void update(int x, int y, HashSet<Tl> intersect){
        if(x < 0 || y < 0 || x >= indices.GetLength(0) || y >= indices.GetLength(1)) return;
        /*
         *The second condition in this if statement shouldn't be
         *necessary, so I'm probably missing a bounds check somewhere
         *else in the code, but checking here seems to be adequate to
         *generate the level correctly. If more bugs crop up with
         *level generation, this is a likely candidate.
         */
        int i = indices[x,y];
        if(i == -1 || i > size) return;

        queue[i].tiles.IntersectWith(intersect);

        while(true){
            int par = (i-1)/2;
            //This will move the tile up the queue even if it's equal
            //to the parent to ensure WaveFunColl.set() works
            //TODO: I don't think "par < 0" is necessary
            if(i == 0 || par < 0 || queue[i].tiles.Count > queue[par].tiles.Count) break;
            else{
                swap(i, par);
                i = par;
            }
        }
    }

    public int getSize(){ return size+1; }

    //Debugging
    public void print(int depth=-1){
        //TODO: implement depth
        Debug.Log("Size = " + (size+1));
        for(int i = 0; i < size+1; i++){
            Debug.Log("x: " + queue[i].x + " y: " + queue[i].y + " Tiles: " + queue[i].tiles.Count);
        }
    }
}


class WaveFuncColl{
    public Tl[,] map; //TODO: Make private and move setting the actual game map tiles to this class
    public WFCQueue queue; //TODO: Make private
    WFCRule[][] rules;

    public WaveFuncColl(int x, int y){ 
        map = new Tl[x,y];
        queue = new WFCQueue(x,y);

        //Manually specifying rules for now
        rules = new WFCRule[Enum.GetNames(typeof(Tl)).Length][];
        rules[(int)Tl.empty] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.empty})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.empty})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.empty}))
        };
        rules[(int)Tl.grass_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.empty})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
        };
        rules[(int)Tl.dirt_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.gravel_0}))
        };
        rules[(int)Tl.stone_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.stone_0, Tl.gravel_0}))
        };
        rules[(int)Tl.gravel_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0, Tl.gravel_0}))
        };
        rules[(int)Tl.error_0] = new WFCRule[] {};
    }

    //Runs through the rules for the tile type at the given location
    //and updates the possible types for all the tiles affected by 
    //those rules
    void update(int x, int y){
        Tl tile = map[x,y];
        for(int i = 0; i < rules[(int)tile].Length; i++){
            int newX = x + rules[(int)tile][i].x;
            int newY = y + rules[(int)tile][i].y;
            queue.update(newX, newY, rules[(int)tile][i].tiles);
        }
    }

    //Sets a tile to a specific type
    public void set(int x, int y, Tl tile){
        queue.update(x, y, new HashSet<Tl>(new Tl[] {}));
        var setTile = queue.pop();
        map[x,y] = tile;
        update(x, y);
    }

    //Generate the map
    public void run(){
        var rand = new System.Random();
        while(queue.getSize() > 0){
            //Get the possible tiles of the lowest entropy location
            var setTile = queue.pop();
            Tl[] possible = new Tl[setTile.tiles.Count];
            setTile.tiles.CopyTo(possible);

            //Pick randomly from possible tiles, then update according to rules
            if(possible.Length == 0){
                map[setTile.x, setTile.y] = Tl.error_0;
                Debug.Log("Impossible Combination at " + setTile.x + ", " + setTile.y);
            } else{
                int temp = rand.Next(0, possible.Length);
                map[setTile.x, setTile.y] = possible[temp];
            }

            update(setTile.x, setTile.y);
        }
    }

    //Debugging
    public void printMap(){
        Debug.Log("Printing Map");
        for(int i = 0; i < map.GetLength(0); i++){
            for(int j = 0; j < map.GetLength(1); j++){
                Debug.Log("x: " + i + " y: " + j + " tile: " + map[i,j].ToString());
            }
        }
    }
}