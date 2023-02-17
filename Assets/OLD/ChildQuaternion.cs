using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildQuaternion : MonoBehaviour
{
    private Quaternion originalRotation;
    private ParentQuaternion parentRotation;

    private void Start()
    {
        originalRotation = transform.localRotation;
        parentRotation = transform.parent.GetComponent<ParentQuaternion>();
    }

    private void LateUpdate()
    {
        transform.rotation = parentRotation.transform.rotation * Quaternion.Inverse(parentRotation.originalRotation) * originalRotation;
    }
}
