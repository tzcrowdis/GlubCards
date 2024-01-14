using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWPlayerController : MonoBehaviour
{
    public bool locked;

    float speed = 4f;
    float horizontal;
    float vertical;
    
    void Start()
    {
        locked = false;
    }

    void Update()
    {
        if (!locked)
        {
            //forward
            //backward
            if (Input.GetKey(KeyCode.W))
            {
                vertical = 1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                vertical = -1f;
            }
            else { vertical = 0f; }

            //left
            //right
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1f;
            }
            else {  horizontal = 0f; }

            transform.position = transform.position + new Vector3(horizontal * speed * Time.deltaTime, 0, vertical * speed * Time.deltaTime);
        }
    }
}
