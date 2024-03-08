using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatBox : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Player player;
    public ChatBox chatBox;
    public Sprite Open;
    public Sprite Close;
    public int keyStrokes = 0;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        chatBox = GameObject.Find("ChatBox").GetComponent<ChatBox>();
    }

    void Update()
    {
        if ( keyStrokes % 2 == 0 ) {
            spriteRenderer.sprite = Close;
            if ( player.chatLock )  {
                player.chatLock = false;
                chatBox.chatLock = false;
            }
        }
        else {
            spriteRenderer.sprite = Open;
            if ( !player.chatLock ) {
                player.chatLock = true;
                chatBox.chatLock = true;
            }
        }
        
        if ( Input.GetKeyDown( "/" ) ) keyStrokes++;
    }
}