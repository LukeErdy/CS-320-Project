using System;
using System.Timers;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : Actor
{
    private Sprite[] currentSprites = null;
    public Sprite[] movingSprites;
    public Sprite[] attackingSprites;
    protected uint spriteIndex = 0;
    protected uint spriteTimer;
    private const uint spriteFrequency = 100;

    private bool isDying = false;
    protected float sightRadius = 10f;

    public override string ToString()
    {
        return $"{name} ({posX}, {posY})";
    }

    public void Start()
    {
        if (null == currentSprites) currentSprites = movingSprites;
        rb = GetComponent<Rigidbody2D>();
        SetMaxHP(10);
    }

    protected bool WithinSightRadius(Vector2 targetLoc)
    {
        double distance = Math.Sqrt(Math.Pow(Math.Abs(posX - targetLoc.x), 2) + Math.Pow(Math.Abs(posY - targetLoc.y), 2));
        if (distance <= sightRadius) GameSession.Instance.BattleStatus(true);
        else GameSession.Instance.BattleStatus(false);
        return distance <= sightRadius;
    }

    public IEnumerator Die()
    {
        //Reset sprite
        isDying = true;
        rb.velocity = new Vector2(0, 0);
        currentSprites = movingSprites;
        spriteIndex = 0;
        spriteRenderer.sprite = currentSprites[spriteIndex];
        spriteRenderer.flipY = true;

        yield return new WaitForSeconds(1);

        //Give player XP for the kill
        var player = GameObject.Find("Player").GetComponent<Player>();
        player.AdjustXP(10);

        //Increase enemies killed
        try
        {
            var gs = GameObject.Find("GameSession").GetComponent<GameSession>();
            gs.IncreaseEnemiesKilled();
        } catch{ }

        //Finally remove GameObject from scene
        Destroy(gameObject);
    }
    
    protected void UpdateSprite()
    {
        if (isDying) return;
        if (null == currentSprites) currentSprites = movingSprites;
        if (spriteTimer >= spriteFrequency) { 
            spriteIndex = spriteIndex + 1 >= currentSprites.Length ? 0 : spriteIndex + 1;
            spriteTimer = 0;
        }
        else spriteTimer++;
    }

    private void Update()
    {
        if (isDying) return;
        var playerLoc = GameObject.Find("Player").transform.position;
        if (WithinSightRadius((Vector2)playerLoc))
        {
            //Calculate velocity
            var dir = (playerLoc - rb.transform.position).normalized;
            float distX = playerLoc.x - posX;
            float distY = playerLoc.y - posY;
            if (rb.gravityScale == 0)
            {
                rb.velocity = new Vector2(walkForce * dir.x, jumpForce * dir.y);
            }
            else
            {
                Debug.Log($"distX abs: {Math.Abs(distX)}  distY abs: {Math.Abs(distY)}");
                if (Math.Abs(distX) > Math.Abs(distY)) rb.velocity = new Vector2(walkForce * dir.x, 0);
                else rb.velocity = new Vector2(0, jumpForce * dir.y);
            }

            //Update sprite based on movement direction
            UpdateSprite();
            if (distX > 0) spriteRenderer.flipX = false; //false
            else if (distX < 0) spriteRenderer.flipX = true; //true
            spriteRenderer.sprite = currentSprites[spriteIndex];
        }
        //If enemy falls below a certain threshold, kill them
        if (transform.position.y < lowThreshold)
        {
            Die();
        }
    }

    private bool IsFacing(Player p)
    {
        //if colliding with player on x-axis
        //Debug.Log($"player: {p.posY}, enemy: {this.posY}");
        return (p.posY-1)<=this.posY && (p.posY + 1) >= this.posY;
    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        if (isDying) return;
        //Debug.Log("OnCollisionEnter2D: " + col.gameObject);
        var player = col.gameObject.GetComponent<Player>();
        if (player)
        {
            //change to attacking sprite
            spriteIndex = 0;
            currentSprites = attackingSprites;
        }
    }

    protected void OnCollisionStay2D(Collision2D col)
    {
        if (isDying) return;
        //Debug.Log("OnCollisionStay2D: " + col.gameObject);
        var player = col.gameObject.GetComponent<Player>();
        if (player && IsFacing(player))
        { 
            if((int)(currentSprites.Length/2) == spriteIndex) player.AdjustHealth(-1*meleeDmg);
        }
    }

    protected void OnCollisionExit2D(Collision2D col)
    {
        if (isDying) return;
        //Debug.Log("OnCollisionExit2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            spriteIndex = 0;
            currentSprites = movingSprites;
        }
    }
}