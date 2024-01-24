using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BagOfPieces : MonoBehaviour
{
    List<string> pieces = new List<string>();
    GameObject chosenPiece;

    public bool pulling;

    bool grow;
    Vector3 startSize;
    Vector3 endSize;
    Vector3 scaleChange;

    public static BagOfPieces Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            UnityEngine.Object.Destroy(Instance.gameObject);

        //load pieces player has from file containing names
        ReadPlayerPieces();
        
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
                if (pieces.Count > 0)
                {
                    PlaceRandomPiece();
                }
                else
                {
                    Debug.Log("Out of pieces.");
                }

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

    public void DisableBag()
    {
        pulling = false;
    }

    void PlaceRandomPiece()
    {
        //pull a random piece
        int randPiece = UnityEngine.Random.Range(0, pieces.Count);
        string pieceName = pieces[randPiece];
        GameObject piecePrefab = Resources.Load("Pieces/" + pieceName) as GameObject;

        //remove it from pieces [commented out for testing]
        //pieces.Remove(pieceName);

        //add it to game
        Vector3 startingPosition = new Vector3(0, 0, 0);
        Quaternion startingRotation = Quaternion.identity;
        switch (pieceName)
        {
            case "Soldier":
                startingPosition = new Vector3(2f, 0f, -1.5f); //X AND Z STILL IMPERFECT
                startingRotation = Quaternion.Euler(0f, 180f, 0f); //180 BC SOLDIER IMPORTED INCORRECTLY
                break;
            default:
                startingPosition = new Vector3(2f, 0f, -1.5f); //X AND Z STILL IMPERFECT
                startingRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
        }

        chosenPiece = Instantiate(piecePrefab, startingPosition, startingRotation);
        chosenPiece.GetComponent<PieceScript>().enemyPiece = false;
    }

    void ReadPlayerPieces()
    {
        try
        {
            using (StreamReader sr = new StreamReader("Assets/GameFiles/PlayerPieces.txt"))
            {
                string name;
                while ((name = sr.ReadLine()) != null)
                {
                    pieces.Add(name);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Player Piece file could not be read:");
            Debug.Log(e.Message);
        }
    }
}
