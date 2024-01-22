using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TheVoid : PieceScript
{
    MeshRenderer mesh;
    readonly float moveSpeed = 2f;
    public override void Start() 
    {
        //set any unique variables here
        hp = 1f;
        dmg = 4f;
        height = 0.5f;

        mesh = GetComponent<MeshRenderer>();

        //Use base to call parent functions so generics get assigned
        base.Start();
    }

    //Not currently compatible as an enemy piece
    public override IEnumerator Act(System.Action onComplete) 
    {
        moving = true;

        GameObject[,] board = GameMaster.Instance.board;

        //Get random position on board
        //TODO, avoid spaces with friendly pieces
        int rngX = UnityEngine.Random.Range(0, board.GetLength(0));
        int rngZ = UnityEngine.Random.Range(0, board.GetLength(1));
        int destX = (int)Math.Round(transform.position.x);
        int destZ = (int)Math.Round(transform.position.y);
        bool found = false;

        //Find a null space on the board
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == null || board[i, j].GetComponent<PieceScript>().enemyPiece)
                {
                    destX = i + 1;
                    destZ = j + 1;
                }
                if (destX != -1 && j >= rngZ && i >= rngX)
                {
                    found = true;
                    break;
                }       
            }
            if (found)
            break;
        }

        Debug.Log($"Void is trying to go to space {destX}, {destZ}");

        //start fade out
        float t = 0f;
        Color colorTemp = mesh.material.color;

        while (t < 1)
        {
            t = LerpAlpha(colorTemp, t, 1f, 0f);
            yield return null;
        }

        //end fade out
        LerpAlpha(colorTemp, 0, 0f, 0f);
        t = 0;

        //Check if a piece is on the board
        GameObject objOnBoard = GameMaster.Instance.board[destX - 1, destZ - 1];
        if (objOnBoard == this)
            objOnBoard = null;

        if (objOnBoard != null)
        {
            transform.position = new Vector3(destX, 2f, destZ);
        }
        else
            transform.position = new Vector3(destX, 0.5f, destZ);

        while (t < 1)
        {
            t = LerpAlpha(colorTemp, t, 0f, 1f);
            yield return null;
        }

        LerpAlpha(colorTemp, 1, 1f, 1f);

        //Attack if piece exists
        if (objOnBoard != null)
        {
            Attack(objOnBoard);
            yield return new WaitForSeconds(0.5f);

            //lower to board
            t = 0;
            Vector3 start = transform.position;
            Vector3 end = transform.position - new Vector3(0f, 2f - height, 0f);
            while (t < 1)
            {
                transform.position = Vector3.Lerp(start, end, t);
                t += Time.deltaTime * moveSpeed;
                yield return null;
            }
            transform.position = end;
        }

        moving = false;

        onComplete?.Invoke();

        yield return null;
    }

    public float LerpAlpha(Color colorTemp, float t, float from, float to)
    {
        colorTemp.a = Mathf.Lerp(from, to, t);
        mesh.material.color = colorTemp;
        return t + (Time.deltaTime * moveSpeed);
    }

    public override void Attack(GameObject enemyPiece)
    {
        try
        {
            enemyPiece.GetComponent<PieceScript>().Defend(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public override void Defend(GameObject enemy)
    {
        try
        {
            hp -= enemy.GetComponent<PieceScript>().dmg;
            if (hp <= 0)
            {
                GameMaster.Instance.RemovePieceFromBoard(gameObject);
                Destroy(gameObject, 0f); //ADJUST 0f TO TIME OF DEATH ANIM
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
