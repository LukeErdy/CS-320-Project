using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] moving;
    private int movingIndex = 0;
    private Timer spriteTimer;
    public Sprite[] idle;
    private Rigidbody2D rb;
    private float posX { get { return rb.position[0]; } }
    private float posY { get { return rb.position[1]; } }

    //grounding fields
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    //TODO: inherit these fields from an Enemy class
    private float walkForce = 1f;
    private float jumpForce = 1f;
    private float sightRadius = 10f;
    private bool canFly = false;

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
        movingIndex = movingIndex+1 >= moving.Length ? 0 : movingIndex + 1;
    }

    private void Update()
    {
        //Calculate velocity based on target player's location
        var playerLoc = GameObject.Find("Player").transform.position; //returns (x,y,z)
        float velocity_x = walkForce * (playerLoc.x - posX);
        float velocity_y = (playerLoc.y - posY);
        //Debug.Log("velocity_y: " + velocity_y);
        if (velocity_y>jumpForce && (CheckIfGrounded() || canFly)) //see if the enemy can jump higher; bounded by its jumpForce
        {
            rb.velocity = new Vector2(velocity_x, velocity_y/jumpForce);
        }
        else rb.velocity = new Vector2(velocity_x, 0); //rb.velocity.y

        //Change sprite based on movement direction
        float dirX = playerLoc.x - posX;
        float dirY = playerLoc.y - posY;
        //Debug.Log("dirY: " + dirY);
        if (dirX > 0) spriteRenderer.flipX = false;
        else if (dirX < 0) spriteRenderer.flipX = true;

        spriteRenderer.sprite = moving[movingIndex];
    }

    private bool CheckIfGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
}