using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBarFollow : MonoBehaviour
{
    GameObject player;
    Vector3 offset;

    void Start()
    {
        player = GameObject.Find("Player"); // Find Player
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        float newXPosition = player.transform.position.x + offset.x;
        float newYPosition = player.transform.position.y + offset.y;

        transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
    }
}
