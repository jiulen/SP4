using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public float duration;
    private float elaspe;
    // Start is called before the first frame update
    protected void Start()
    {
        elaspe = 0;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (elaspe >= duration)
        {
            Destroy(gameObject);
        }
        elaspe += Time.deltaTime;
    }
}
