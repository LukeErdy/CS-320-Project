using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Player : Actor
{
    //Debug
    bool canFly = false;

    //Sprite variables
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;
    private Sprite[] currentSprites = null;
    private int spriteIndex = 0;

    //Health and XP variables
    public HealthBar healthBar;
    public XPBar xpBar;
    float requiredXP = 45;
    public float currentXP;

    //Other variables
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    private float lastVelocity = 0f;
    public bool chatLock = false;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walkForce = 9;
        jumpForce = 15;
        SetMaxHP(30);
        AdjustXP(0);
        meleeDmg = 2;
    }

    private void UpdateMovement()
    {
        // Get the walk direction and apply a horizontal force to the player
        float dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(walkForce * dirX, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (CheckIfGrounded() || canFly)) // Using Unity's input manager for greater flexibility
        {
            // Apply a vertical force to the object to which this script is assigned (in this case, the player)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //AdjustXP(3f);// and gain XP from spamming space (you're welcome Caleb)
        }

        //Change sprite
        if (currentSprites == null) currentSprites = rightSprites;
        if (dirX == 1) currentSprites = rightSprites;
        else if (dirX == -1) currentSprites = leftSprites;
        spriteIndex = Input.GetKey("q") ? 1 : 0;                //if attacking, change to attacking sprite
        spriteRenderer.sprite = currentSprites[spriteIndex];
    }

    public void CheckHealth()
    {
        if (currentHP <= 0)
        {
            //TODO: How do we want to handle death? rn I'm just respawning him
            GameSession.Instance.IncreasePlayerDeath(); // this is not working :(
            rb.velocity = new Vector2(0, 0);
            transform.position = new Vector3(Generate.MIN_X + 1, Generate.MAX_Y - 1, transform.position.z);
            AdjustHealth(maxHP);
        }

        // mainly for testing purposes
        if (transform.position.y < lowThreshold)
        { // you lose health from falling
            AdjustHealth(-0.01f);
        }
    }

    private void CheckFallDamage()
    {
        if (lastVelocity < -30f && CheckIfGrounded())
        {
            AdjustHealth(lastVelocity * 0.009f);
        }
        lastVelocity = rb.velocity.y;
    }

    private void Update()
    {
        CheckHealth();
        UpdateMovement();
        CheckFallDamage();
    }

    private bool IsFacing(Enemy e)
    {
        var enemyDir = (e.transform.position - this.transform.position).normalized.x;
        var playerDir = Input.GetAxis("Horizontal");
        //Debug.Log($"enemy: {enemyDir}, player: {playerDir}");
        if (enemyDir < 0 && playerDir < 0) return true;
        else if (enemyDir > 0 && playerDir > 0) return true;
        else return false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("OnCollisionEnter2D: " + col.gameObject);
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (IsFacing(enemy) && Input.GetKey("q"))
            {
                enemy.AdjustHealth(-1 * meleeDmg);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //Debug.Log("OnCollisionStay2D: " + col.gameObject);
        var enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (IsFacing(enemy) && Input.GetKey("q")) {
                enemy.AdjustHealth(-1 * meleeDmg);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        //Debug.Log("OnCollisionExit2D: " + col.gameObject);
    }

    public void AdjustXP(float change)
    {
        currentXP += change;
        if (currentXP >= requiredXP)
        {
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
