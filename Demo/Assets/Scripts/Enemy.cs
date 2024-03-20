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
    protected int spriteIndex = 0;
    protected Timer spriteTimer;

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
        spriteTimer = new Timer(100);
        spriteTimer.Elapsed += ChangeSprite;
        spriteTimer.AutoReset = true;
        spriteTimer.Enabled = true;
    }

    protected void ChangeSprite(System.Object source, ElapsedEventArgs e)
    {
        if (isDying) return;
        if (null == currentSprites) currentSprites = movingSprites;
        spriteIndex = spriteIndex + 1 >= currentSprites.Length ? 0 : spriteIndex + 1;
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
        GameSession.Instance.IncreaseEnemiesKilled();
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

        //Finally remove GameObject from scene
        Destroy(gameObject);
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
            //Debug.Log("playerLoc: " + playerLoc + "    distY: " + distY + "      normaldirY: " + dir.y);
            if (distY >= jumpForce) rb.velocity = new Vector2(walkForce * distX, jumpForce * dir.y);
            else rb.velocity = new Vector2(walkForce * distX, 0);

            //Change sprite based on movement direction
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
        Debug.Log($"player: {p.posY}, enemy: {this.posY}");
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