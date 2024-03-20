using System.Timers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Sprite[] movingSprites;
    private uint spriteIndex;
    private uint spriteTimer;
    public Actor Source;
    public Actor Target;
    public int Dir = 1;
    public uint Speed = 1;
    public uint Damage = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Update/check velocity
        rb.velocity = new Vector2(Dir * Speed, 0); 
        //Update sprite
        if (Dir >= 0) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
        if (spriteTimer >= 20)
        {
            spriteIndex = spriteIndex + 1 >= movingSprites.Length ? 0 : spriteIndex + 1;
            spriteRenderer.sprite = movingSprites[spriteIndex];
            spriteTimer = 0;
        }
        else spriteTimer++;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
      
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //kill the thing it collided with
        var actor = col.gameObject.GetComponent<Actor>();
        if (actor) {
            actor.AdjustHealth(-1*Damage);
        }
        Destroy(gameObject);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        
    }
}