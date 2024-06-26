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

    public float hp { get; protected set; }

    public virtual void Start()
    {
        turnText = GameObject.Find("Turn Visualization").GetComponent<TextMeshProUGUI>();
        turnText.alpha = 0f;
    }

    //override this with individual AI logic
    public virtual GameObject[,] GetPlacements(GameObject[,] board) 
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
        position.y = 0.16f; //ASSUMES THIS IS DEFAULT MODEL HEIGHT
        Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
        GameObject pieceObj = Instantiate(piecePrefab, position, rotation);
        pieceObj.GetComponent<PieceScript>().enemyPiece = true;
        GameMaster.Instance.InitializePiece(pieceObj.GetComponent<PieceScript>());

        pieces.Remove(pieceName);
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

    public virtual void SetHp() { }

    public virtual void TakeDmg(GameObject attacker) { }
}
