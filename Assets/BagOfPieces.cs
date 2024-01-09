using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagOfPieces : MonoBehaviour
{
    GameObject[] pieces;

    public bool pulling;

    bool grow;
    Vector3 startSize;
    Vector3 endSize;
    Vector3 scaleChange;

    private void Start()
    {
        pulling = false;
        grow = false;

        startSize = transform.localScale;
        endSize = new Vector3(2.25f, 2.25f, 2.25f);
        scaleChange = new Vector3(0.01f, 0.01f, 0.01f);
    }

    private void Update()
    {
        
        //hover effect during turn sequence
        if (grow)
        {
            if (transform.localScale.x < endSize.x) 
            {
                transform.localScale += scaleChange;
            }
            else
            {
                transform.localScale = endSize;
            }
        }
        else
        {
            if (transform.localScale.x > startSize.x)
            {
                transform.localScale -= scaleChange;
            }
            else
            {
                transform.localScale = startSize;
            }
        }
    }

    void OnMouseOver()
    {
        if (pulling)
        {
            //enable some hover effect
            grow = true;

            if (Input.GetMouseButtonDown(0))
            {
                //pull a random piece
                //remove it from pieces
                //add it to player hand (script and in game)
                pulling = false;
                grow = false;
            }
        }
    }

    void OnMouseExit()
    {
        //disable hover effect
        grow = false;
    }
}
