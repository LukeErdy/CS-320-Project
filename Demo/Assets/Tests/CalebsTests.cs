using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class CalebsTests{
    //Level Dimensions copied from Generate.cs
    const int MIN_X = -10;
    const int MAX_X = 59;
    const int MIN_Y = -42;
    const int MAX_Y = 19;


    // (Tilemap, Generate) createMap(){
    //     GameObject game = new GameObject();
    //     Tilemap map = game.AddComponent<Tilemap>();
    //     Generate gen = game.AddComponent<Generate>();
    //     gen.tilemap = map;
    //     for(int i = 0; i < 6; i++) gen.tiles[i] = ScriptableObject.CreateInstance<Tile>();
    //     gen.tiles[0].name = "None";
    //     gen.tiles[1].name = "grass_0";
    //     gen.tiles[2].name = "dirt_0";
    //     gen.tiles[3].name = "stone_0";
    //     gen.tiles[4].name = "gravel_0";
    //     gen.tiles[5].name = "error_0";
    //     gen.Start();
    //     return (map, gen);
    // }    
    

    // //Black Box Acceptance Test
    // //Checks that the generated map contains at least one of every
    // //non-error tile, including None
    // [Test]
    // public void ContainsAllNormalTiles(){
    //     var (map, gen) = createMap();

    //     Dictionary<string,int> tileCount = new Dictionary<string,int>();
    //     for(int i = MIN_X; i < MAX_X; i++){
    //         for(int j = MIN_Y; j < MAX_Y; j++){
    //             int count = 0;
    //             string tileName = map.GetTile(new Vector3Int(i, j, 0)).name;
    //             tileCount.TryGetValue(tileName, out count);
    //             tileCount[tileName] = count+1;
    //         }
    //     }
    //     for(int i = 0; i < 5; i++)
    //         Assert.Greater(tileCount[gen.tiles[i].name], 0);
    // }


    // //Black Box Acceptance Test
    // //Checks that all tiles are valid
    // [Test]
    // public void NoInvalidTiles(){
    //     var (map, gen) = createMap();

    //     for(int i = MIN_X; i < MAX_X; i++){
    //         for(int j = MIN_Y; j < MAX_Y; j++){
    //             Assert.AreNotEqual(map.GetTile(new Vector3Int(i, j, 0)).name, "error_0");
    //         }
    //     }
    // }


    // //Black Box Acceptance Test
    // //Checks that only empty tiles are above a grass tile and that
    // //no empty tiles are below a grass tile
    // [Test]
    // public void SkyAboveGround(){
    //     var (map, gen) = createMap();

    //     for(int i = MIN_X; i < MAX_X; i++){
    //         int j = MAX_Y-1;
    //         for(; j > MIN_Y; j--){
    //             string curTileName = map.GetTile(new Vector3Int(i, j, 0)).name;
    //             if(curTileName == "grass_0") break;
    //             Assert.AreEqual("None", curTileName);
    //         } j--;

    //         Assert.Greater(j, MIN_Y);
            
    //         for(; j > MIN_Y; j--){
    //             string curTileName = map.GetTile(new Vector3Int(i, j, 0)).name;
    //             Assert.AreNotEqual("grass_0", curTileName);
    //             Assert.AreNotEqual("None", curTileName);
    //         }
    //     }
    // }


    // //White Box Test
    // //Tests that the hill doesn't decrease in height when not inverted
    // //and that it produces hills of the correct height
    // //Branch coverage is achieved with this and HillGoesDown()
    // [Test]
    // public void HillGoesUp(){
    //     int width = 10;
    //     int amplitude = 5;
    //     var testHill = new Hill(width, amplitude, false);

    //     int prev = -1;
    //     for(int i = 0; i < width; i++){
    //         int curr = testHill.getY(i);
    //         Assert.GreaterOrEqual(curr, prev);
    //         prev = curr;
    //     }
    //     //checks amplitude-1 rather than amplitude because height starts at 0
    //     Assert.AreEqual(amplitude-1, prev); 
    // }


    // //White Box Test
    // //Tests that the hill doesn't increase in height when inverted
    // //and that it produces hills of the correct height
    // //Branch coverage is achieved with this and HillGoesUp()
    // [Test]
    // public void HillGoesDown(){
    //     int width = 10;
    //     int amplitude = 5;
    //     var testHill = new Hill(width, amplitude, true);

    //     int prev = 1;
    //     for(int i = 0; i < width; i++){
    //         int curr = testHill.getY(i);
    //         Assert.LessOrEqual(curr, prev);
    //         prev = curr;
    //     }
    //     Assert.AreEqual(-(amplitude-1), prev);
    // }


    // //Integration test
    // //Tests that the wave function collapse is properly creating a
    // //WFCQueue and it's being filled with the correct number of items
    // [Test]
    // public void FullQueue(){
    //     const int xDim = MAX_X-MIN_X;
    //     const int yDim = MAX_Y-MIN_Y;
    //     WaveFuncColl WFC = new WaveFuncColl(xDim,yDim);

    //     var inQueue = new bool[xDim,yDim];
        
    //     foreach(var coord in WFC.queue.queue){
    //         inQueue[coord.x, coord.y] = true;
    //     }

    //     for(int i = 0; i < xDim; i++){
    //         for(int j = 0; j < yDim; j++){
    //             Assert.True(inQueue[i,j]);
    //         }
    //     }
    // }
}