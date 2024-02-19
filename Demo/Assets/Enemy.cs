using System;
using System.Timers;
using UnityEngine;

public class Enemy : Actor
{
    protected Sprite[] currentSprites = null;
    public Sprite[] movingSprites;
    public Sprite[] attackingSprites;
    protected int spriteIndex = 0;
    protected Timer spriteTimer;

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
        if (null == currentSprites) currentSprites = movingSprites;
        spriteIndex = spriteIndex + 1 >= currentSprites.Length ? 0 : spriteIndex + 1;
    }

    protected bool WithinSightRadius(Vector2 targetLoc)
    {
        double distance = Math.Sqrt(Math.Pow(Math.Abs(posX - targetLoc.x), 2) + Math.Pow(Math.Abs(posY - targetLoc.y), 2));
        return distance <= sightRadius;
    }

    public void Update()
    {
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
            if (distX > 0) spriteRenderer.flipX = false;
            else if (distX < 0) spriteRenderer.flipX = true;
            spriteRenderer.sprite = currentSprites[spriteIndex];
        }

        //If enemy falls below a certain threshold, kill them
        if (transform.position.y < lowThreshold)
        {
            AdjustHealth(-1*maxHP);
            Destroy(gameObject);
        }
    }

    protected void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("OnCollisionEnter2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            //change to bite sprite
            spriteIndex = 0;
            currentSprites = attackingSprites;
        }
    }

    protected void OnCollisionStay2D(Collision2D col)
    {
        //Debug.Log("OnCollisionStay2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            //TODO: check if rodent is facing correct direction in order for the bite to be effective
            /*ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
            col.GetContacts(contacts);
            for (int i = 0; i < contactCount; i++)
                Debug.Log(contacts[i].point + " " + contacts[i].normal);*/
            var player = col.gameObject.GetComponent<Player>();
            if (spriteIndex == 1) player.AdjustHealth(-1);
        }
    }

    protected void OnCollisionExit2D(Collision2D col)
    {
        //Debug.Log("OnCollisionExit2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            spriteIndex = 0;
            currentSprites = movingSprites;
        }
    }
}