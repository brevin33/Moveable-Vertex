using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
