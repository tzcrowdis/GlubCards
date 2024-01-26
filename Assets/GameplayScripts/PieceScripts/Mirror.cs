using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : PieceScript
{
    

    public override void Start()
    {
        hp = 1f;
        dmg = 0f;
        height = 0.16f;

        base.Start();
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        //HANDLE REFLECTING PROJECTILES
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;

        //mirror doesn't act

        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public override IEnumerator Move(float x, float z)
    {
        yield return null; //mirror doesn't move
    }

    public override IEnumerator Attack(GameObject enemyPiece)
    {
        //mirror doesn't attack
        yield return null;
    }

    public override IEnumerator Defend(GameObject enemyPiece) 
    {
        try
        {
            //check if physical or laser???
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
