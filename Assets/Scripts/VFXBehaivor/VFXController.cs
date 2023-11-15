using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    private void Start()
    {
        //destroy after 2 seconds
        Destroy(gameObject, 0.5f);
    }
}
