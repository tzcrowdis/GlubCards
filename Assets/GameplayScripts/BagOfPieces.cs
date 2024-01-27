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

    public int[,] activeSlots = new int[6, 2];

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
        endSize = new Vector3(startSize.x * 1.15f, startSize.y * 1.15f, startSize.z * 1.15f);
        float delta = (endSize.x - startSize.x) / 20f;
        scaleChange = new Vector3(delta, delta, delta);
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
        if (!FullHand())
        {
            //pull a random piece
            int randPiece = UnityEngine.Random.Range(0, pieces.Count);
            string pieceName = pieces[randPiece];
            GameObject piecePrefab = Resources.Load("Pieces/" + pieceName) as GameObject;

            //remove it from pieces [commented out for testing]
            //pieces.Remove(pieceName);

            //add it to game
            Vector3 startingPosition = new Vector3(UnityEngine.Random.Range(1.8f, 2.2f), 3f, UnityEngine.Random.Range(-1.5f, -2f));
            Quaternion startingRotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), 0f, UnityEngine.Random.Range(0f, 360f));

            chosenPiece = Instantiate(piecePrefab, startingPosition, startingRotation);
            chosenPiece.GetComponent<PieceScript>().enemyPiece = false;
        }
        else
        {
            //display message that user cant pull more pieces unless they place more
            Debug.Log("Hand is full.");
        }
    }

    bool FullHand()
    {
        int sum = 0;
        for (int i = 0; i < activeSlots.GetLength(0); i++)
        {
            for (int j = 0; j < activeSlots.GetLength(1); j++)
            {
                sum += activeSlots[i, j];
            }
        }

        if (sum == activeSlots.Length)
            return true;
        else
            return false;
    }

    public (float, float) GetNearestOpenSlot(float i, float j)
    {
        //calc distance to nearest point
        float[,] distMatrix = new float[activeSlots.GetLength(0), activeSlots.GetLength(1)];
        for (int x = 0; x < distMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < distMatrix.GetLength(1); y++)
            {
                if (activeSlots[x, y] == 0)
                    distMatrix[x, y] = (float)Math.Sqrt(Math.Pow(i - x, 2) + Math.Pow(j - y, 2));
                else
                    distMatrix[x, y] = float.MaxValue;
            }
        }

        //find minimums index
        float min = float.MaxValue;
        int openI = int.MaxValue;
        int openJ = int.MaxValue;
        for (int x = 0; x < distMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < distMatrix.GetLength(1); y++)
            {
                if (distMatrix[x, y] < min)
                {
                    min = distMatrix[x, y];
                    openI = x;
                    openJ = y;
                }
            }
        }

        //set slot as occupied
        try
        {
            activeSlots[openI, openJ] = 1;
        }
        catch
        {
            Debug.Log("Maximum number of active pieces reached.");
        }
        
        return (openI, openJ);
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
