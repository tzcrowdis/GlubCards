using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagOfPieces : MonoBehaviour
{
    string[] pieces;
    GameObject chosenPiece;

    public bool pulling;

    bool grow;
    Vector3 startSize;
    Vector3 endSize;
    Vector3 scaleChange;

    private void Start()
    {
        //load pieces player has from file containing names
        //naming convention for upgrades?
        
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
                transform.localScale += scaleChange;
            else
                transform.localScale = endSize;
        }
        else
        {
            if (transform.localScale.x > startSize.x)
                transform.localScale -= scaleChange;
            else
                transform.localScale = startSize;
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
                chosenPiece = Resources.Load("Pieces/PawnTemp") as GameObject;

                //remove it from pieces

                //add it to game
                Instantiate(chosenPiece, new Vector3(3f, 0.25f, -1f), Quaternion.identity);
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
