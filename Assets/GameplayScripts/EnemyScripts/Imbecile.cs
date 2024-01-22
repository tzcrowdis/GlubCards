using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Imbecile : Enemy
{
    public override void Start()
    {
        base.Start();
        LoadPieceList("Imbecile");
    }

    int FindRow(GameObject[,] board)
    {
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int z = board.GetLength(1) - 1; z >= 0; z--)
            {
                if (board[x, z] != null && !board[x, z].GetComponent<PieceScript>().enemyPiece)
                {
                    return x;
                }
            }
        }
        return -1;
    }

    public override GameObject[,] GetPlacements(GameObject[,] board)
    {
        //send pawn at enemy pieces
        int row = FindRow(board);
        if (row >= 0)
        {
            if (pieces.Count > 0)
                PlacePiece(pieces[0], new Vector3(row, 0.5f, board.GetLength(1) - 1));
        }
        
        turnText.alpha = 1f;
        StartCoroutine(FadeText());

        return board;
    }

    public override void SetHp() 
    {
        hp = 3;
    }

    public override void TakeDmg(GameObject attacker)
    {
        hp -= attacker.GetComponent<PieceScript>().dmg;

        GameMaster.Instance.UpdateScore(gameObject);
    }
}
