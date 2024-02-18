using System;
using System.Timers;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Sprite[] currentSprites = null;
    public Sprite[] movingSprites;
    public Sprite[] attackingSprites;
    private int spriteIndex = 0;
    private Timer spriteTimer;
    private Rigidbody2D rb;
    private float posX { get { return rb.position[0]; } }
    private float posY { get { return rb.position[1]; } }

    //depends on terrain
    float lowThreshold = -20f;

    //TODO: inherit these fields from an Enemy class
    private float walkForce = 1f;
    private float jumpForce = 2f;
    private float sightRadius = 5f;
    private float currentHP;
    private float maxHP = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteTimer = new Timer(100);
        spriteTimer.Elapsed += ChangeSprite;
        spriteTimer.AutoReset = true;
        spriteTimer.Enabled = true;
        currentHP = maxHP;
    }

    private void ChangeSprite(System.Object source, ElapsedEventArgs e)
    {
        if (null == currentSprites) currentSprites = movingSprites;
        spriteIndex = spriteIndex + 1 >= currentSprites.Length ? 0 : spriteIndex + 1;
    }

    private bool WithinSightRadius(Vector2 targetLoc)
    {
        double distance = Math.Sqrt(Math.Pow(Math.Abs(posX - targetLoc.x), 2) + Math.Pow(Math.Abs(posY - targetLoc.y), 2));
        return distance <= sightRadius;
    }

    private void Update()
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("OnCollisionEnter2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            //change to bite sprite
            spriteIndex = 0;
            currentSprites = attackingSprites;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //Debug.Log("OnCollisionStay2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            //check if rodent is facing correct direction in order for the bite to be effective
            /*ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
            col.GetContacts(contacts);
            for (int i = 0; i < contactCount; i++)
                Debug.Log(contacts[i].point + " " + contacts[i].normal);*/
            var player = col.gameObject.GetComponent<PlayerMetrics>();
            Bite(player);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        //Debug.Log("OnCollisionExit2D: " + col.gameObject);
        if (col.gameObject.name.Equals("Player"))
        {
            spriteIndex = 0;
            currentSprites = movingSprites;
        }
    }

    //TODO: move to Rodent class
    private void Bite(PlayerMetrics p)
    {
        if(p && spriteIndex==1) p.AdjustHealth(-1);
    }

    public void AdjustHealth(float change)
    {
        currentHP += change;
        if (currentHP < 0f)
        {
            currentHP = 0f;
        }
        else if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }
}