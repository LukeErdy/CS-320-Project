//Comment this line to disable level gen and revert to original level
// #define DO_GENERATE'
// #define OLD_RULES
# define NEW_RULES

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
        const int xDim = MAX_X-MIN_X;
        const int yDim = MAX_Y-MIN_Y;

        //WFC TESTING===================================================

        int testX = xDim-1, testY = yDim;
        // int testX = 10, testY = 10;
        WaveFuncColl test = new WaveFuncColl(testX,testY);
        for(int i = 0; i < testX; i++){
            // Debug.Log("Setting " + i + ", 2");
            test.set(i, 30, Tl.grass_0);
        }
        // test.queue.print();
        test.run();

        for(int i = 0; i < testX; i++){
            for(int j = 0; j < testY; j++){
                tilemap.SetTile(new Vector3Int(i+MIN_X, j+MIN_Y, 0), tiles[(int)test.map[i,j]]);
            }
        }


        //END WFC TESTING===============================================


        #if DO_GENERATE
        //TODO: Set player position above ground level

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
                // Debug.Log(i + ", " + j + ": " + (i*y + j));

                #if NEW_RULES
                queue[indices[i,j]] = (i, j, new HashSet<Tl>(new Tl[] 
                    {Tl.empty, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}));
                #endif

                #if OLD_RULES
                queue[indices[i,j]] = (i, j, new HashSet<Tl>(new Tl[] 
                    {Tl.empty, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}));
                #endif
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
        //TODO: check for empty queue
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
        indices[output.x, output.y] = -1;
        size--;
        return output;
    }

    public void update(int x, int y, HashSet<Tl> intersect){
        // Debug.Log("Updating " + x + ", " + y);
        if(x < 0 || y < 0 || x >= indices.GetLength(0) || y >= indices.GetLength(1)) return;


        int i = indices[x,y];
        if(i == -1) return;
        // Debug.Log(queue[i].x + " " + queue[i].y);

        // Debug.Log("Original set");
        // foreach(Tl tile in queue[i].tiles){
        //     Debug.Log(tile.ToString());
        // }
        // Debug.Log("Intersecting set");
        // foreach(Tl tile in intersect){
        //     Debug.Log(tile.ToString());
        // }

        queue[i].tiles.IntersectWith(intersect);

        // Debug.Log("New set");
        // foreach(Tl tile in queue[i].tiles){
        //     Debug.Log(tile.ToString());
        // }

        while(true){
            int par = (i-1)/2;
            if(par < 0 || queue[i].tiles.Count >= queue[par].tiles.Count) break;
            else{
                swap(i, par);
                i = par;
            }
        }
    }

    public int getSize(){ return size+1; }

    public void print(){
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
        #if NEW_RULES
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
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.grass_0})), //TODO
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.grass_0})) //TODO
        };
        rules[(int)Tl.dirt_0] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0})),
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
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.gravel_0})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.stone_0, Tl.gravel_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.grass_0, Tl.dirt_0, Tl.stone_0, Tl.gravel_0}))
        };
        rules[(int)Tl.error_0] = new WFCRule[] {};
        #endif


        #if OLD_RULES
        rules = new WFCRule[Enum.GetNames(typeof(Tl)).Length][];
        rules[(int)Tl.empty] = new WFCRule[] {
            new WFCRule(0, 1, new HashSet<Tl>(new Tl[] {Tl.empty})),
            new WFCRule(0,-1, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.dirt_0})),
            new WFCRule(1, 0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0})),
            new WFCRule(-1,0, new HashSet<Tl>(new Tl[] {Tl.empty, Tl.grass_0, Tl.dirt_0}))
        };
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
        rules[(int)Tl.error_0] = new WFCRule[] {};
        #endif
    }

    void update(int x, int y){
        Tl tile = map[x,y];
        // Debug.Log((int)tile + " " + rules.Length);
        for(int i = 0; i < rules[(int)tile].Length; i++){
            int newX = x + rules[(int)tile][i].x;
            int newY = y + rules[(int)tile][i].y;
            queue.update(newX, newY, rules[(int)tile][i].tiles);
        }
    }

    public void set(int x, int y, Tl tile){
        // queue.print();
        queue.update(x, y, new HashSet<Tl>(new Tl[] {}));
        (int x, int y, HashSet<Tl> tiles) setTile = queue.pop();
        map[x,y] = tile;
        update(x, y);
    }

    public void run(){
        // queue.print();
        var rand = new System.Random();
        while(queue.getSize() > 0){
            //Get the possible tiles of the lowest entropy location
            (int x, int y, HashSet<Tl> tiles) setTile = queue.pop();
            Tl[] possible = new Tl[setTile.tiles.Count];
            setTile.tiles.CopyTo(possible);


            //Pick randomly from possible tiles, then update according to rules
            if(possible.Length == 0){
                map[setTile.x, setTile.y] = Tl.error_0;
                Debug.Log("Impossible Combination at " + setTile.x + ", " + setTile.y);
            } else{
                int temp = rand.Next(0, possible.Length);
                // Debug.Log("[0," + (possible.Length) + "]: " + temp);
                map[setTile.x, setTile.y] = possible[temp];
                // Debug.Log("Setting " + setTile.x + "," + setTile.y + " to " + possible[temp].ToString());
            }
            
            // Debug.Log("Popping " + setTile.x + ", " + setTile.y + " Setting to " + map[setTile.x,setTile.y].ToString());
            // queue.print();
            
            update(setTile.x, setTile.y);
        }
        // printMap();
    }

    public void printMap(){
        Debug.Log("Printing Map");
        for(int i = 0; i < map.GetLength(0); i++){
            for(int j = 0; j < map.GetLength(1); j++){
                Debug.Log("x: " + i + " y: " + j + " tile: " + map[i,j].ToString());
            }
        }
    }
}