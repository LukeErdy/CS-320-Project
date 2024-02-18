using System;
using UnityEngine;

public class Actor : MonoBehaviour
{
    //HP variables
    private float _currentHP = 1;
    private float _maxHP = 1;
    public float currentHP { get { return _currentHP; } }
    public float maxHP { get { return _maxHP; } }

    public float walkForce = 1f;
    public float jumpForce = 1f;

    //depends on terrain
    protected float lowThreshold = -100f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float posX { get { if (rb) return rb.position[0]; else return 0; } }
    public float posY { get { if (rb) return rb.position[1]; else return 0; } }
    

    public void AdjustHealth(float change)
    {
        _currentHP += change;
        if (_currentHP < 0f)
        {
            _currentHP = 0f;
            //TODO: trigger death sequence
        }
        else if (_currentHP > maxHP)
        {
            _currentHP = maxHP;
        }

        if (this is Player p)
        {
            p.healthBar.SetHealth((_currentHP / _maxHP) * 100f);
        }
    }

    public void SetMaxHP(float value)
    {
        if (value > 0)
        {
            _maxHP = _currentHP = value;
        }
        else throw new ArgumentException("MaxHP must be positive.");
    }
}