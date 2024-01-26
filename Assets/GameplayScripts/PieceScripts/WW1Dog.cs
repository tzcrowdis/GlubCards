using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WW1Dog : PieceScript
{
    Canvas atkCanvas;
    
    public override void Start()
    {
        hp = 1f;
        dmg = 1f;
        height = 0.13f;

        atkCanvas = GameObject.Find("AttackCanvas").GetComponent<Canvas>();
        atkCanvas.enabled = false;

        base.Start();
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;

        //only attack in 4th and 5th rows
        float x = transform.position.x;
        float z = transform.position.z;
        if (enemyPiece)
        {
            if (GameMaster.Instance.board[(int)x, (int)z - 4] != null)
                yield return Attack(GameMaster.Instance.board[(int)x, (int)z - 4]);
            if (GameMaster.Instance.board[(int)x, (int)z - 3] != null)
                yield return Attack(GameMaster.Instance.board[(int)x, (int)z - 3]);
        }
        else
        {
            if (GameMaster.Instance.board[(int)x, (int)z + 4] != null)
                yield return Attack(GameMaster.Instance.board[(int)x, (int)z + 4]);
            if (GameMaster.Instance.board[(int)x, (int)z + 3] != null)
                yield return Attack(GameMaster.Instance.board[(int)x, (int)z + 3]);
        }

        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public override IEnumerator Move(float x, float z)
    {
        yield return null; //dog doesn't move (shell shocked)
    }

    public override IEnumerator Attack(GameObject enemyPiece) 
    {
        //attach text bubble to them
        //"oh god, it's just looking at me..."
        yield return DisplayAttackText(enemyPiece);
        yield return enemyPiece.GetComponent<PieceScript>().Defend(gameObject);
    }

    IEnumerator DisplayAttackText(GameObject enemyPiece)
    {
        Vector3 position = new Vector3(enemyPiece.transform.position.x, enemyPiece.GetComponent<PieceScript>().height + 1f, enemyPiece.transform.position.z);
        Quaternion rotation = Quaternion.identity; //ROTATE TOWARDS CAMERA???
        atkCanvas.transform.position = position;
        atkCanvas.transform.rotation = rotation;

        atkCanvas.enabled = true;
        yield return new WaitForSeconds(2f);
        atkCanvas.enabled = false;

        yield return null;
    }

    public override IEnumerator Defend(GameObject enemyPiece) 
    {
        try
        {
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
