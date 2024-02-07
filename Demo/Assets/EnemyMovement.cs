using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite facingRight;
    public Sprite facingLeft;
    private Rigidbody2D rb;
    private float posX { get { return rb.position[0]; } }
    private float posY { get { return rb.position[1]; } }
    //likely to be inherited from an Enemy class
    private float walkForce = 1f;
    private float jumpForce = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var playerLoc = GameObject.Find("Player").transform.position; //returns (x,y,z)
        rb.velocity = new Vector2(walkForce*(playerLoc.x - posX), jumpForce*(playerLoc.y - posY));

        //Debug.Log("Player Location: " + playerLoc);
        //Debug.Log("Velocity: " + rb.velocity);

        //Change sprite based on movement direction
        float dirX = playerLoc.x - posX;
        //Debug.Log("DirX: " + dirX);
        if (dirX > 0) spriteRenderer.sprite = facingRight;
        else if (dirX < 0) spriteRenderer.sprite = facingLeft;

        //TODO: change sprite if upside down?
    }
}