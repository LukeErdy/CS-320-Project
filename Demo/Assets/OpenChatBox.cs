using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatBox : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public PlayerMovement player;
    public Sprite Open;
    public Sprite Close;
    public int keyStrokes = 0;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if ( keyStrokes % 2 == 0 ) {
            spriteRenderer.sprite = Close;
            if ( player.chatLock ) player.chatLock = false;
        }
        else {
            spriteRenderer.sprite = Open;
            if ( !player.chatLock ) player.chatLock = true;
        }
        
        if ( Input.GetKeyDown( "/" ) ) keyStrokes++;
    }
}