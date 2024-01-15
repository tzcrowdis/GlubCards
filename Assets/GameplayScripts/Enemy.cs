using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using TMPro; //for testing

public class Enemy : MonoBehaviour
{
    TextMeshProUGUI turnText;

    List<string> pieces = new List<string>();

    void Start()
    {
        turnText = GameObject.Find("Turn Visualization").GetComponent<TextMeshProUGUI>();
        turnText.alpha = 0f;
    }

    void LoadPieceList(string fileName)
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

    void PlacePiece(string pieceName, Vector3 position) //upgrades???
    {
        GameObject piecePrefab = Resources.Load("Pieces/" + pieceName) as GameObject;
        Instantiate(piecePrefab, position, Quaternion.identity); //adjust orientation???
    }

    public virtual GameObject[,] getPlacements(GameObject[,] board) //override this with individual AI logic
    {
        turnText.alpha = 1f;
        StartCoroutine(FadeText());

        return board;
    }

    IEnumerator FadeText()
    {
        for (float alpha = 1f; alpha >= 0; alpha -= 0.01f)
        {
            turnText.alpha = alpha;
            yield return null;
        }

        turnText.alpha = 0f;
        yield return null;
    }
}
