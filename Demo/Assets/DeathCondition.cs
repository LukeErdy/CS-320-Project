using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCondition : MonoBehaviour
{
    float lowThreshold = -10f;
    float highThreshold = 7f;
    float spawnX = -4.66f;
    float spawnY = 0.23f;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < lowThreshold || transform.position.y > highThreshold)
        {
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
        }
    }
}
