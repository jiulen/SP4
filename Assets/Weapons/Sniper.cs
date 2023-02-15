using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : WeaponBase
{
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
    }

    //Update is called once per frame
    void Update()
    {
        UpdateWeaponBase();

        if (Input.GetButton("Fire1"))
        {
        }
    }
}
