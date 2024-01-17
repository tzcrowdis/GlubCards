using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        body.constraints = RigidbodyConstraints.FreezePosition;
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
