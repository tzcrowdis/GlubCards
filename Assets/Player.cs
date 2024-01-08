using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool yourTurn; //set by game master [ADD SETTER?]

    public GameObject[] startGrid = new GameObject[5];

    public bool selectingPosition;
    public bool selectingPiece;
    

    public CardScript piece;
    Vector3 startPos;

    void Start()
    {
        selectingPosition = false;
        selectingPiece = false;
    }

    void Update()
    {
        if (yourTurn)
        {
            if (!selectingPiece) { selectingPiece = true; }

            if (selectingPiece)
            {
                //pull piece from bag

                //turn on hover effect for pieces in hand

                if (piece != null)
                {
                    //move cards to selected starting position
                    if (selectingPosition)
                    {
                        //turn on hover effect for grid

                        for (int i = 0; i < startGrid.Length; i++)
                        {
                            if (startGrid[i].GetComponent<GridElemScript>().selected == true)
                            {
                                startPos = startGrid[i].transform.position;
                                startPos.z = (float)-0.75;
                                piece.hoverLight.enabled = false;
                                StartCoroutine(piece.MoveToStart(startPos));
                            }
                        }
                    }
                }
            }
        }
    }
}
