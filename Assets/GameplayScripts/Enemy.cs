using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using TMPro; //for testing

public class Enemy : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    protected List<string> pieces = new List<string>();

    public virtual void Start()
    {
        turnText = GameObject.Find("Turn Visualization").GetComponent<TextMeshProUGUI>();
        turnText.alpha = 0f;
    }

    public virtual GameObject[,] GetPlacements(GameObject[,] board) //override this with individual AI logic
    {
        turnText.alpha = 1f;
        StartCoroutine(FadeText());

        return board;
    }

    protected void LoadPieceList(string file)
    {
        try
        {
            using (StreamReader sr = new StreamReader($"Assets/GameFiles/{file}Pieces.txt"))
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
            Debug.Log($"{file} Piece file could not be read:");
            Debug.Log(e.Message);
        }
    }

    protected void PlacePiece(string pieceName, Vector3 position) //upgrades???
    {
        GameObject piecePrefab = Resources.Load("Pieces/" + pieceName) as GameObject;
        position.y = 0.5f; //ASSUMES THIS IS DEFAULT MODEL HEIGHT
        GameObject pieceObj = Instantiate(piecePrefab, position, Quaternion.identity); //adjust orientation???
        pieceObj.GetComponent<PieceScript>().enemyPiece = true;
        GameMaster.Instance.InitializePiece(pieceObj.GetComponent<PieceScript>());

        //pieces.Remove(pieceName);
    }

    public IEnumerator FadeText()
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
