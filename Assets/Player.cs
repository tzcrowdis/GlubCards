using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool yourTurn; //set by game master [ADD SETTER?]

    public GameObject[] startGrid = new GameObject[5];

    public bool selectingPiece;
    bool pullPiece;

    public CardScript piece;
    Vector3 startPos;

    void Start()
    {
        selectingPiece = false;
        pullPiece = true;
    }

    void Update()
    {
        if (yourTurn)
        {
            if (pullPiece)
            {
                //pull piece from bag

                selectingPiece = true;

                //turn on hover effect for pieces in hand

                //set pullPiece to false
            }

            if (selectingPiece)
            {
                if (piece != null) //selecting position
                {
                    //turn on hover effect for grid

                    //move cards to selected starting position
                    for (int i = 0; i < startGrid.Length; i++)
                    {
                        if (startGrid[i].GetComponent<GridElemScript>().selected == true)
                        {
                            startPos = startGrid[i].transform.position;
                            startPos.z = (float)-0.75;
                            piece.hoverLight.enabled = false;
                            StartCoroutine(piece.MoveToStart(startPos));

                            //clean up variables here?
                            //maybe have master do it?
                        }
                    }
                }
            }
        }
    }
}
