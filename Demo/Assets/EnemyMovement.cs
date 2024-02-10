using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex = 0;
    private Timer spriteTimer;
    private Rigidbody2D rb;
    private float posX { get { return rb.position[0]; } }
    private float posY { get { return rb.position[1]; } }

    //TODO: inherit these fields from an Enemy class
    private float walkForce = 1f;
    private float jumpForce = 1f;
    private float sightRadius = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteTimer = new Timer(100);
        spriteTimer.Elapsed += ChangeSprite;
        spriteTimer.AutoReset = true;
        spriteTimer.Enabled = true;
    }

    private void ChangeSprite(System.Object source, ElapsedEventArgs e)
    {
        spriteIndex = spriteIndex + 1 >= sprites.Length ? 0 : spriteIndex + 1;
    }

    private bool WithinSightRadius(Vector2 targetLoc)
    {
        double distance = Math.Sqrt(Math.Pow(Math.Abs(posX-targetLoc.x), 2) + Math.Pow(Math.Abs(posY-targetLoc.y), 2));
        return distance <= sightRadius;
    }

    private void Update()
    {
        var playerLoc = GameObject.Find("Player").transform.position;
        if (WithinSightRadius((Vector2)playerLoc)) {
            //Calculate velocity
            var dir = (playerLoc - rb.transform.position).normalized;
            float distX = playerLoc.x - posX;
            float distY = playerLoc.y - posY;
            Debug.Log("playerLoc: " + playerLoc + "    distY: " + distY + "      normaldirY: " + dir.y);
            if(distY>=jumpForce) rb.velocity = new Vector2(walkForce * distX, jumpForce * dir.y);
            else rb.velocity = new Vector2(walkForce * distX,0);

            //Change sprite based on movement direction
            if (distX > 0) spriteRenderer.flipX = false;
            else if (distX < 0) spriteRenderer.flipX = true;
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

}