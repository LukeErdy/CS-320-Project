using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatFollowPlayer : MonoBehaviour
{
    GameObject player;
    Vector3 offset;

    void Start()
    {
        player = GameObject.Find("Player");
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        float newXPosition = player.transform.position.x + offset.x;
        float newYPosition = player.transform.position.y + offset.y;

        transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
    }
}
