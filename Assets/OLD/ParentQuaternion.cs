using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentQuaternion : MonoBehaviour
{
    public Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }
    void Update()
    {
        ResetRotation();
    }
    public void ResetRotation()
    {
        transform.rotation = originalRotation;
    }
}
