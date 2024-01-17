using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Pawn : PieceScript
{
    public override void Start() 
    {
        //set any unique variables here
        hp = 1f;
        dmg = 1f;
        height = 0.16f; //based on model USE SETTER???

        //Use base to call parent functions so generics get assigned
        base.Start();
    }

    public override IEnumerator Move(System.Action onComplete) //ALWAYS NEED TO ADAPT FOR ENEMY OWNED PIECE
    {
        moving = true;
        float x = transform.position.x;
        float z = transform.position.z;

        if (!enemyPiece)
        {
            //pawns logic
            //check board one space ahead
            //move or attack
            if (GameMaster.Instance.board[(int)x - 1, (int)z] != null)
            {
                Attack(GameMaster.Instance.board[(int)x - 1, (int)z]);
            }
            else
            {
                z++;
            } 
        }
        else
        {
            //pawns logic as enemy piece
            //check board one space ahead
            //move or attack
            if (GameMaster.Instance.board[(int)x - 1, (int)z - 2] != null)
            {
                Attack(GameMaster.Instance.board[(int)x - 1, (int)z - 2]);
            }
            else
            {
                z--;
            }
        }

        //set piece specific move vars
        Vector3 nextPos = new Vector3(x, height, z);
        //Quaternion nextRot = Quaternion.Euler(0, 180, 0);
        Vector3 currentPos = transform.position;
        Vector3 startPos = transform.position;
        //Quaternion currentRot = transform.rotation;
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, nextPos, t);
            //transform.rotation = Quaternion.Lerp(currentRot, nextRot, t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = nextPos;
        //transform.rotation = nextRot;

        moving = false;

        onComplete?.Invoke();

        yield return null;
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