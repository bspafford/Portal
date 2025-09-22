using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowMovement : MonoBehaviour
{
    float speed = 400.0f;
    Rigidbody rb;
    Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * 7.5f, ForceMode.Force);
    }
}
