using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    //Debug
    bool canFly = false;

    // Player Sprites
    public SpriteRenderer spriteRenderer;
    public Sprite facingRight;
    public Sprite facingLeft;
    public bool chatLock = false;
    // public Sprite jumping;

    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    
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
        // Prevent player movement if chatbox is open
        if ( chatLock ) return;

        // Get the walk direction and apply a horizontal force to the player
        dirX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw instead of GetAxis so that it returns to 0 immediately
        rb.velocity = new Vector2(walkForce * dirX, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (CheckIfGrounded() || canFly)) // Using Unity's input manager for greater flexibility
        {
            // Apply a vertical force to the object to which this script is assigned (in this case, the player)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Change sprite based on movement direction
        if (dirX == 1) spriteRenderer.sprite = facingRight;
        else if (dirX == -1) spriteRenderer.sprite = facingLeft;
    }

    public bool CheckIfGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer)) {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * castDistance, boxSize);
    }
}
