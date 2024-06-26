using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool yourTurn; //set by game master

    public GameObject[] startGrid = new GameObject[6];

    public bool selectingPiece;

    bool pullPiece;
    BagOfPieces bag;

    public PieceScript piece;
    Vector3 startPos;

    float hp;

    void Start()
    {
        selectingPiece = false;
        pullPiece = true;

        bag = GameObject.Find("BagOfPieces").GetComponent<BagOfPieces>();

        hp = 3;
    }

    void Update()
    {
        if (yourTurn)
        {
            if (pullPiece)
            {
                //pull piece from bag
                bag.pulling = true;

                //turn on piece selection
                TurnOnPieceSelection();
                pullPiece = false;
            }

            if (selectingPiece)
            {
                if (piece != null) //selecting position
                {
                    TurnOnGridSelection();

                    //move cards to selected starting position
                    for (int i = 0; i < startGrid.Length; i++)
                    {
                        if (startGrid[i].GetComponent<LaneSelector>().selected == true)
                        {
                            startPos = startGrid[i].transform.position;
                            startPos.z = piece.GetStartPositionZ(); //SET STARTING POSITION BY FUNCTION IN PIECE

                            StartCoroutine(piece.MoveToStart(startPos));

                            TurnOffGridSelection();
                            TurnOffPieceSelection();
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
            CameraScript.Instance.ChangeCameraView(CameraScript.Views.Board_View);

        if (Input.GetKeyDown(KeyCode.W))
            CameraScript.Instance.ChangeCameraView(CameraScript.Views.Top_View);

        if (Input.GetKeyDown(KeyCode.D))
            CameraScript.Instance.ChangeCameraView(CameraScript.Views.Bag_View);
    }

    public void StartTurn()
    {
        yourTurn = true;
        selectingPiece = false;
        pullPiece = true;
        //more setup here
    }

    public void EndTurn()
    {
        yourTurn = false;
        TurnOffPieceSelection();
        //more cleanup here
    }

    void TurnOnPieceSelection()
    {
        selectingPiece = true;
    }

    void TurnOffPieceSelection()
    {
        piece = null;
        selectingPiece = false;
    }

    void TurnOnGridSelection()
    {
        for (int i = 0; i < startGrid.Length; i++)
        {
            startGrid[i].GetComponent<LaneSelector>().on = true;
        }
    }

    void TurnOffGridSelection()
    {
        for (int i = 0; i < startGrid.Length; i++)
        {
            startGrid[i].GetComponent<LaneSelector>().on = false;
            startGrid[i].GetComponent<LaneSelector>().selected = false;
        }
    }

    public void TakeDmg(GameObject attacker)
    {
        hp -= attacker.GetComponent<PieceScript>().dmg;

        GameMaster.Instance.UpdateScore(gameObject);
    }
}
