using System;
using System.Timers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Message : Actor
{
    public GameObject player;
    public Sprite[] spriteArray;
    public int spriteIndex = 0;
    public float viewRadius = 50;

    public void Start()
    {
        player = GameObject.Find("Player");
    }
        
    protected bool WithinView(Vector2 playerPosition)
    {
        double distance = System.Math.Sqrt(System.Math.Pow(System.Math.Abs(transform.position.x - playerPosition.x), 2) + System.Math.Pow(System.Math.Abs(transform.position.y - playerPosition.y), 2));
        return distance <= 1000;
    }

    private void Update()
    {
        WithinView(new Vector2 (10, 10));
    }
}
