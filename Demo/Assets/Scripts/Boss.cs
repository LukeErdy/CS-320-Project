using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Actor
{
    public float maxHealth = 500;
    public float currHealth;
    public Sprite[] attackSprites;
    public bool isAttacking = false;
    private float baseAttackDamage = 12f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth <= 0) {
            Die();
        }
    }

    float BaseAttack()
    {
        var player = GameObject.Find("Player").GetComponent<Player>();
        player.AdjustXP(baseAttackDamage);
        return baseAttackDamage;
    }

    float TakeDamage(float damage)
    {
        currHealth += damage;
        return damage;
    }

    void Die()
    {
        // destroy object and sprite
    }
}
