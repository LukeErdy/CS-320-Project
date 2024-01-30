using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCondition : MonoBehaviour
{
    float threshold = -10f;
    float spawnX = -4.66f;
    float spawnY = 0.23f;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
        }
    }
}
