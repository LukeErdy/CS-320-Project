using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Actor
{
    //Debug
    bool canFly = false;

    // Player Sprites
    public Sprite facingRight;
    public Sprite facingLeft;
    // public Sprite jumping;

    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    float dirX;

    public HealthBar healthBar;

    //XP variables
    public XPBar xpBar;
    float requiredXP = 45;
    public float currentXP;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walkForce = 9;
        jumpForce = 15;
        SetMaxHP(30);
        AdjustXP(0);

    }

    private void UpdateMovement()
    {
        // Get the walk direction and apply a horizontal force to the player
        dirX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw instead of GetAxis so that it returns to 0 immediately
        rb.velocity = new Vector2(walkForce * dirX, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (CheckIfGrounded() || canFly)) // Using Unity's input manager for greater flexibility
        {
            // Apply a vertical force to the object to which this script is assigned (in this case, the player)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AdjustXP(3f);// and gain XP from spamming space(you're welcome Caleb)
        }

        // Change sprite based on movement direction
        if (dirX == 1) spriteRenderer.sprite = facingRight;
        else if (dirX == -1) spriteRenderer.sprite = facingLeft;
    }

    private void CheckHealth()
    {
        if (currentHP <= 0)
        {
            //TODO: How do we want to handle death?
            //rn I'm just respawning him
            rb.velocity = new Vector2(0, 0);
            transform.position = new Vector3(0, 10, transform.position.z);
            AdjustHealth(maxHP);
        }

        // mainly for testing purposes
        if (transform.position.y < lowThreshold)
        { // you lose health from falling
            AdjustHealth(-0.01f);
        }
    }

    private void Update()
    {
        CheckHealth();
        UpdateMovement();
    }

    public void AdjustXP(float change)
    {
        currentXP += change;
        if (currentXP >= requiredXP) {
            currentXP = 0f;
            requiredXP *= 1.6f;
        }

        xpBar.SetXP((currentXP / requiredXP) * 100f);
    }


    public bool CheckIfGrounded()
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
