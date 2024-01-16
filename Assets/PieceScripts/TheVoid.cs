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
    public override void Start() 
    {
        //set any unique variables here
        hp = 1f;
        dmg = 1f;
        height = 0.5f;

        mesh = GetComponent<MeshRenderer>();

        //Use base to call parent functions so generics get assigned
        base.Start();
    }

    public override IEnumerator Move(System.Action onComplete) 
    {
        moving = true;

        GameObject[,] board = GameMaster.Instance.board;

        //Get random position on board
        int destX = UnityEngine.Random.Range(0, board.GetLength(0));
        int destY = UnityEngine.Random.Range(0, board.GetLength(1));

        //start fade out
        float t = 0f;
        Color colorTemp = mesh.material.color;

        while (t < 1)
        {
            t = LerpAlpha(colorTemp, t, 1f, 0f);
            yield return null;
        }

        LerpAlpha(colorTemp, 0, 0f, 0f);
        t = 0;

        while (t < 1)
        {
            t = LerpAlpha(colorTemp, t, 0f, 1f);
            yield return null;
        }

        LerpAlpha(colorTemp, 1, 1f, 1f);

        moving = false;

        onComplete?.Invoke();

        yield return null;
    }

    public float LerpAlpha(Color colorTemp, float t, float from, float to)
    {
        colorTemp.a = Mathf.Lerp(from, to, t);
        mesh.material.color = colorTemp;
        return t + Time.deltaTime;
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
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
