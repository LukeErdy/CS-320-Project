using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Encapsulates the behavior of an entity with HP, a <see cref="SpriteRenderer"/>, and a <see cref="Rigidbody2D"/>.
/// </summary>
public class Actor : MonoBehaviour
{
    //HP variables
    private float _currentHP = 1;
    private float _maxHP = 1;
    public float currentHP { get { return _currentHP; } }
    public float maxHP { get { return _maxHP; } }

    //Moving/location variables
    public float walkForce = 1f;
    public float jumpForce = 1f;
    public int lowThreshold {get { return Generate.MIN_Y*3; }} 

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float posX { get { if (rb) return rb.position[0]; else return 0; } }
    public float posY { get { if (rb) return rb.position[1]; else return 0; } }

    //Attack variables
    protected uint meleeDmg = 1;
    protected uint rangedDmg = 1;

    public void AdjustHealth(float change)
    {
        _currentHP += change;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
            if (this is Enemy e) StartCoroutine(e.Die());
            //TODO: trigger death sequence for player
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