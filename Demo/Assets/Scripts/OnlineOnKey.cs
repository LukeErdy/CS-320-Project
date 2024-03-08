using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineOnKey : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite Online;
    public Sprite Offline;
    public int keyStrokes = 0;


    void Start()
    {
        
    }

    void Update()
    {
        if ( keyStrokes % 2 == 0 ) spriteRenderer.sprite = Online;
	else spriteRenderer.sprite = Offline;
        
        if ( Input.GetKeyDown( "o" ) ) keyStrokes++;
    }
}
