using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite facingRight;
    public Sprite facingLeft;
    // public Sprite jumping;
    
    private Rigidbody2D rb; // So that we only have to call GetComponent once
    float dirX;

    float walkForce = 9f;
    float jumpForce = 15f;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the walk direction and apply a horizontal force to the player
        dirX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw instead of GetAxis so that it returns to 0 immediately
        rb.velocity = new Vector2(walkForce * dirX, rb.velocity.y);

        if (Input.GetButtonDown("Jump")) // Using Unity's input manager for greater flexibility
        {
            // Apply a vertical force to the object to which this script is assigned (in this case, the player)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (dirX == 1) spriteRenderer.sprite = facingRight;
        else if (dirX == -1) spriteRenderer.sprite = facingLeft;
    }
}
