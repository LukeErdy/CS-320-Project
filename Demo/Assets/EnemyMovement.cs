using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float dirX;
    private float walkForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //TODO: how to get player's location?
        dirX = Input.GetAxisRaw("Horizontal")*-1; //move opposite direction of player?
        rb.velocity = new Vector2(walkForce * dirX, rb.velocity.y);
    }
}