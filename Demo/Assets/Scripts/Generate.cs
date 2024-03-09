//Comment these defines to disable level gen and revert to original level
//#define OLD_GEN
#define WFC_GEN
//Cave generation isn't done, toggle on to test
// #define CAVE_GEN

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//TODO: Set player position above ground level


class distSort : IComparer{
    int IComparer.Compare(object a, object b){
        var aTup = ((double d, int x1, int y1, int x2, int y2))a;
        var bTup = ((double d, int x1, int y1, int x2, int y2))b;
        return (int)(aTup.d - bTup.d);
    }
}


public class Generate : MonoBehaviour{
    public const int MIN_X = -10;
    public const int MAX_X = 59;
    public const int MIN_Y = -42;
    public const int MAX_Y = 19;

    const int MAX_AMP = 10;
    const int MIN_AMP = 3;
    const int MAX_PER = 10;
    const int MIN_PER = 5;

    const int xDim = MAX_X-MIN_X;
    const int yDim = MAX_Y-MIN_Y;
    
    public int[] grassTiles = new int[xDim];
    public bool IsInitialized { get; private set; } = false; //needed for EnemySpawner

    //TODO replace this method 2 from https://stackoverflow.com/a/56604959
    public Tilemap tilemap;
    //[0] = null; [1] = grass; [2] = dirt; [3] = stone; [4] = gravel
    public Tile[] tiles = new Tile[6];

    // Start is called before the first frame update
    public void Start(){
        #if WFC_GEN
        int testX = xDim, testY = yDim;
        // int testX = 5, testY = 5;
        WaveFuncColl test = new WaveFuncColl(testX,testY);
        var rand = new System.Random();

        //Generate a horizontal line of grass
        // for(int i = 0; i < testX; i++) test.set(i, 30, Tl.grass_0);

        #if !CAVE_GEN
        //Generate Grassy Hills
        //FIXME: Some empty tiles with high amplitude (~30)
        int startY=yDim-30, width=0, wCount=0, currH, prevH=-1, amplitude;
        bool goDown = true;
        Hill nextHill = null;

        for(int i = 0; i < xDim; i++){
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
            grassTiles[i] = currH;
            prevH = currH;
            wCount++;
        }
        #endif


        #if CAVE_GEN
        //Generate Caves
        const int numNodes = 20;
        const int numEntrances = 3;
        const int maxConnections = (int)(1.5*numNodes);
        const int maxConnPerNode = 6;

        var caveNodes = new (int x, int y)[numNodes];
        var edgeList = new (double d, int x1, int y1, int x2, int y2)[((numNodes*(numNodes-1))/2)];
        int ELC = 0;
        for(int i = 0; i < numNodes; i++){
            caveNodes[i] = (rand.Next(xDim), rand.Next(yDim));
            for(int j = 0; j < i; j++){
                edgeList[ELC++] = (
                    Math.Sqrt(Math.Pow(caveNodes[i].x-caveNodes[j].x, 2)+Math.Pow(caveNodes[i].y-caveNodes[j].y, 2)), 
                    caveNodes[i].x, caveNodes[i].y, caveNodes[j].x, caveNodes[j].y);
            }
        }
        
        // IComparer myComparer = new distSort();
        Array.Sort(edgeList, new distSort());
        foreach(var item in edgeList){
            Debug.Log("d = " + item.d + "; x1 = " + item.x1 + "; y1 = " + item.y1 + "; x2 = " + item.x2 + "; y2 = " + item.y2);
        }
        #endif


        //Generate Tiles
        test.run();
        #if CAVE_GEN
        foreach(var coord in caveNodes) test.set(coord.x, coord.y, Tl.error_0);
        #endif
        

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

        for(int i = 0; i < xDim; i += width+1){
            width = rand.Next(MIN_PER, MAX_PER);
            amplitude = rand.Next(MIN_AMP, MAX_AMP);

            var nextHill = new Hill(width, amplitude, startBool);
            for(int j = 0; j <= width; j++){
                if(i+j >= xDim-1) break;
                int colH = startY+nextHill.getY(j);
                //FIXME: Got an index OOB error once
                map[i+j, colH] = Tl.grass_0;
                grassTiles[i+j] = colH;
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
        IsInitialized = true;
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
class WFCRule{
    public int x, y;
    public Dictionary<Tl,float> tiles;
    public WFCRule(int xIn, int yIn, Dictionary<Tl,float> tilesIn){ x=xIn; y=yIn; tiles=tilesIn; }
}


class WFCQueue{
    int[,] indices;
    int size; //Actual size minus one
    (float ent, int x, int y, Dictionary<Tl,float> tiles)[] queue;

    public WFCQueue(int x, int y){
        indices = new int[x,y];
        queue = new (float ent, int x, int y, Dictionary<Tl,float> tiles)[x*y]; 
        size = queue.Length-1;

        //Initialize the queue with each tile being able to be any of
        //the possible types and the indices pointing to the correct
        //location in the queue
        for(int i = 0; i < x; i++){
            for(int j = 0; j < y; j++){
                indices[i,j] = i*y + j;
                queue[indices[i,j]] = ((float)1/4, i, j, new Dictionary<Tl,float>{
                    {Tl.empty, (float)1/4},
                    {Tl.dirt_0, (float)1/4},
                    {Tl.stone_0, (float)1/4},
                    {Tl.gravel_0, (float)1/4}
                });
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


    public (float ent, int x, int y, Dictionary<Tl,float> tiles) pop(){
        if(size < 0) return (0, 0, 0, null); //This is to make WaveFuncColl.set() work after the map has been generated
        var output = queue[0];
        queue[0] = queue[size];

        int i = 0;
        while(true){
            int lCh = (2*i) + 1;
            int rCh = (2*i) + 2;
            if((lCh >= size || queue[i].ent >= queue[lCh].ent) &&
                (rCh >= size || queue[i].ent >= queue[rCh].ent)){
                break;
            } else if(queue[i].ent < queue[lCh].ent){
                swap(i, lCh);
                i = lCh;
            } else if(queue[i].ent < queue[rCh].ent){
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
    public void update(int x, int y, Dictionary<Tl,float> intersect){
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

        //TODO: Comment
        var Keys = queue[i].tiles.Keys.ToList();
        float divisor = 0;
        for(int j = 0; j < Keys.Count(); j++){
            if(!intersect.ContainsKey(Keys[j])) continue;
            queue[i].tiles[Keys[j]] *= intersect[Keys[j]];
            divisor += queue[i].tiles[Keys[j]];
        }
        float newEntropy = 0;
        for(int j = 0; j < Keys.Count(); j++){
            queue[i].tiles[Keys[j]] /= divisor;
            if(queue[i].tiles[Keys[j]] > newEntropy) newEntropy = queue[i].tiles[Keys[j]];
        }

        queue[i].ent = (newEntropy != 0) ? newEntropy : 2;

        while(true){
            int par = (i-1)/2;
            //This will move the tile up the queue even if it's equal
            //to the parent to ensure WaveFunColl.set() works
            //TODO: I don't think "par < 0" is necessary
            if(i == 0 || par < 0 || queue[i].ent < queue[par].ent) break;
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
            Debug.Log("x: " + queue[i].x + " y: " + queue[i].y + " Entropy: " + queue[i].ent);
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
            new WFCRule(0, 1, new Dictionary<Tl,float>{
                {Tl.empty, 1},
                {Tl.dirt_0, 0},
                {Tl.stone_0, 0},
                {Tl.gravel_0, 0}
            }),
            new WFCRule(0,-1, new Dictionary<Tl,float>{
                {Tl.empty, 1},
                {Tl.dirt_0, 0},
                {Tl.stone_0, 0},
                {Tl.gravel_0, 0}
            }),
            new WFCRule(1, 0, new Dictionary<Tl,float>{
                {Tl.empty, 1},
                {Tl.dirt_0, 0},
                {Tl.stone_0, 0},
                {Tl.gravel_0, 0}
            }),
            new WFCRule(-1,0, new Dictionary<Tl,float>{
                {Tl.empty, 1},
                {Tl.dirt_0, 0},
                {Tl.stone_0, 0},
                {Tl.gravel_0, 0}
            })
        };
        rules[(int)Tl.grass_0] = new WFCRule[] {
            new WFCRule(0, 1, new Dictionary<Tl,float>{
                {Tl.empty, 1},
                {Tl.dirt_0, 0},
                {Tl.stone_0, 0},
                {Tl.gravel_0, 0}
            }),
            new WFCRule(0,-1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, (float)1/2},
                {Tl.stone_0, 0},
                {Tl.gravel_0, (float)1/2}
            })
        };
        rules[(int)Tl.dirt_0] = new WFCRule[] {
            new WFCRule(0, 1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.89f},
                {Tl.stone_0, 0.01f},
                {Tl.gravel_0, 0.10f}
            }),
            new WFCRule(0,-1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.79f},
                {Tl.stone_0, 0.01f},
                {Tl.gravel_0, 0.20f}
            }),
            new WFCRule(1, 0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.89f},
                {Tl.stone_0, 0.01f},
                {Tl.gravel_0, 0.10f}
            }),
            new WFCRule(-1,0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.89f},
                {Tl.stone_0, 0.01f},
                {Tl.gravel_0, 0.10f}
            })
        };
        rules[(int)Tl.stone_0] = new WFCRule[] {
            new WFCRule(0, 1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.01f},
                {Tl.stone_0, 0.89f},
                {Tl.gravel_0, 0.10f}
            }),
            new WFCRule(0,-1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.01f},
                {Tl.stone_0, 0.98f},
                {Tl.gravel_0, 0.01f}
            }),
            new WFCRule(1, 0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.01f},
                {Tl.stone_0, 0.90f},
                {Tl.gravel_0, 0.09f}
            }),
            new WFCRule(-1,0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.01f},
                {Tl.stone_0, 0.90f},
                {Tl.gravel_0, 0.09f}
            })
        };
        rules[(int)Tl.gravel_0] = new WFCRule[] {
            new WFCRule(0, 1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.25f},
                {Tl.stone_0, 0.01f},
                {Tl.gravel_0, 0.74f}
            }),
            new WFCRule(0,-1, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.01f},
                {Tl.stone_0, 0.39f},
                {Tl.gravel_0, 0.60f}
            }),
            new WFCRule(1, 0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.05f},
                {Tl.stone_0, 0.10f},
                {Tl.gravel_0, 0.85f}
            }),
            new WFCRule(-1,0, new Dictionary<Tl,float>{
                {Tl.empty, 0},
                {Tl.dirt_0, 0.05f},
                {Tl.stone_0, 0.10f},
                {Tl.gravel_0, 0.85f}
            })
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
        queue.update(x, y, new Dictionary<Tl,float>());
        var setTile = queue.pop();
        map[x,y] = tile;
        update(x, y);
    }

    //Generate the map
    public void run(){
        //TODO
        var rand = new System.Random();
        while(queue.getSize() > 0){
            var setTile = queue.pop();

            double randVal = rand.NextDouble();
            foreach(var tile in setTile.tiles){
                // Debug.Log(tile.Value);
                randVal -= tile.Value;
                if(randVal < 0){
                    map[setTile.x, setTile.y] = tile.Key;
                    break;
                }
            }
            if(randVal >= 0){
                map[setTile.x, setTile.y] = Tl.error_0;
                Debug.Log("Impossible Combination at " + setTile.x + ", " + setTile.y + "; RandVal = " + randVal);
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