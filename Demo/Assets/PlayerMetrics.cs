using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{
    public HealthBar healthBar;
    public XPBar xpBar;

    float maximumHealth = 30;
    float currentHealth;

    float requiredXP = 45;
    float currentXP;

    float lowThreshold = -20f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maximumHealth;
        currentXP = 0;
        AdjustXP(0);
    }

    void Update()
    {
        // mainly for testing purposes
        if (transform.position.y < lowThreshold) { // you lose health from falling
            AdjustHealth(-0.01f);
        }

        if (Input.GetButtonDown("Jump")) { // and gain XP from spamming space (you're welcome Caleb)
            AdjustXP(3f);
        }
    }

    void AdjustHealth(float change)
    {
        currentHealth += change;
        if (currentHealth < 0f) {
            currentHealth = 0f;
        } else if (currentHealth > maximumHealth) {
            currentHealth = maximumHealth;
        }

        healthBar.SetHealth((currentHealth / maximumHealth) * 100f);
    }

    void AdjustXP(float change)
    {
        currentXP += change;
        if (currentXP >= requiredXP) {
            currentXP = 0f;
            requiredXP *= 1.6f;
        }

        xpBar.SetXP((currentXP / requiredXP) * 100f);
    }
}
