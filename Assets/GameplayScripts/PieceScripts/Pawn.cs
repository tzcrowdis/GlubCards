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
        hp = 1f;
        dmg = 1f;
        height = 0.16f;

        base.Start();
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;

        float x = transform.position.x;
        float z = transform.position.z;
        GameObject spaceInFront;

        if (!enemyPiece)
        {
            //pawns logic
            //check board one space ahead
            //move or attack
            try
            {
                spaceInFront = GameMaster.Instance.board[(int)x, (int)z + 1];
                if (spaceInFront != null)
                {
                    if (spaceInFront.GetComponent<PieceScript>().enemyPiece)
                        Attack(spaceInFront);
                }
                else
                {
                    z++;
                }
            }
            catch
            {
                //attack enemy
                Debug.Log("Attacking Enemy Directly");
                GameMaster.Instance.enemy.TakeDmg(gameObject);
            }
        }
        else
        {
            //pawns logic as enemy piece
            //check board one space ahead
            //move or attack
            try
            {
                spaceInFront = GameMaster.Instance.board[(int)x, (int)z - 1];
                if (spaceInFront != null)
                {
                    if (!spaceInFront.GetComponent<PieceScript>().enemyPiece)
                        Attack(spaceInFront);
                }
                else
                {
                    z--;
                }
            }
            catch
            {
                //attack player
                Debug.Log($"{gameObject.name} is attacking Player Directly");
                GameMaster.Instance.player.TakeDmg(gameObject);
            }
        }

        yield return Move(x, z);

        moving = false;
        onComplete?.Invoke();

        yield return null;
    }

    public override IEnumerator Move(float x, float z)
    {
        //set piece specific move vars
        Vector3 nextPos = new Vector3(x, height, z);
        Vector3 currentPos = transform.position;
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, nextPos, t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = nextPos;
        yield return null;
    }

    public override IEnumerator Attack(GameObject enemyPiece)
    {
        yield return enemyPiece.GetComponent<PieceScript>().Defend(gameObject);

        yield return null;
    }

    public override IEnumerator Defend(GameObject enemyPiece)
    {
        Debug.Log("called defend");
        
        try
        {
            Debug.Log("hello");
            hp -= enemyPiece.GetComponent<PieceScript>().dmg;
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

        yield return null;
    }
}
