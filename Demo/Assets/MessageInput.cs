using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInput : MonoBehaviour
{   
    public OpenChatBox chatBox;
    public SpriteRenderer spriteRenderer;
    public int messageIndex = 0;
    public int spriteCount = 9;
    public Sprite[] spriteArray;

    void Start()
    {
        chatBox = GameObject.Find("ChatBox").GetComponent<OpenChatBox>();
    }


    void Update()
    {
        if ( chatBox.keyStrokes % 2 == 0 ) {
            spriteRenderer.sprite = spriteArray[9];
            return;
        }

        if ( Input.GetKeyDown( "1" ) ) messageIndex = 0;
        else if ( Input.GetKeyDown( "2" ) ) messageIndex = 1;
        else if ( Input.GetKeyDown( "3" ) ) messageIndex = 2;
        else if ( Input.GetKeyDown( "4" ) ) messageIndex = 3;
        else if ( Input.GetKeyDown( "5" ) ) messageIndex = 4;
        else if ( Input.GetKeyDown( "6" ) ) messageIndex = 5;
        else if ( Input.GetKeyDown( "7" ) ) messageIndex = 6;
        else if ( Input.GetKeyDown( "8" ) ) messageIndex = 7;
        else if ( Input.GetKeyDown( "9" ) ) messageIndex = 8;

        int realIndex = messageIndex % spriteCount;
        spriteRenderer.sprite = spriteArray[realIndex];
    }
}
