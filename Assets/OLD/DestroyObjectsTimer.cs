using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectsTimer : MonoBehaviour
{
    private float destroyedtimer = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (destroyedtimer <= 0)
            Destroy(this.gameObject);
        else
            destroyedtimer -= Time.deltaTime;
    }
}
