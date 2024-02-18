using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{
    public HealthBar healthBar;
    public XPBar xpBar;

    private float maxHP = 30;
    private float currentHP;

    float requiredXP = 45;
    public float currentXP;

    float lowThreshold = -20f;

    float spawnX = 0;
    float spawnY = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        currentXP = 0;
        AdjustXP(0);
    }

    void Update()
    {
        if (currentHP <= 0)
        {
            //TODO: How do we want to handle death?
            //rn I'm just respawning him
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
            AdjustHealth(maxHP);
        }

        // mainly for testing purposes
        if (transform.position.y < lowThreshold) { // you lose health from falling
            AdjustHealth(-0.01f);
        }

        if (Input.GetButtonDown("Jump")) { // and gain XP from spamming space (you're welcome Caleb)
            AdjustXP(3f);
        }
    }

    public void AdjustHealth(float change)
    {
        currentHP += change;
        if (currentHP < 0f) {
            currentHP = 0f;
        } else if (currentHP > maxHP) {
            currentHP = maxHP;
        }

        healthBar.SetHealth((currentHP / maxHP) * 100f);
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
}
