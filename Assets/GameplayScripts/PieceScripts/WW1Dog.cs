using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WW1Dog : PieceScript
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

        //only attack in 4th and 5th rows
        float x = transform.position.x;
        float z = transform.position.z;
        if (enemyPiece)
        {
            if (GameMaster.Instance.board[(int)x, (int)z - 4] != null)
                Attack(GameMaster.Instance.board[(int)x, (int)z - 4]);
            if (GameMaster.Instance.board[(int)x, (int)z - 3] != null)
                Attack(GameMaster.Instance.board[(int)x, (int)z - 3]);
        }
        else
        {
            if (GameMaster.Instance.board[(int)x, (int)z + 4] != null)
                Attack(GameMaster.Instance.board[(int)x, (int)z + 4]);
            if (GameMaster.Instance.board[(int)x, (int)z + 3] != null)
                Attack(GameMaster.Instance.board[(int)x, (int)z + 3]);
        }

        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public override IEnumerator Move(float x, float z)
    {
        yield return null; //dog doesn't move (shell shocked)
    }

    public override void Attack(GameObject enemyPiece) 
    {
        try
        {
            //attach text bubble to them
            //"oh god, it's just looking at me..."
            StartCoroutine(DisplayAttackText(enemyPiece));
            enemyPiece.GetComponent<PieceScript>().Defend(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    IEnumerator DisplayAttackText(GameObject enemyPiece)
    {
        //load text
        GameObject dogDeathText = Resources.Load("UI/dogDeathText") as GameObject; //MAKE PREFAB
        Vector3 position = new Vector3(enemyPiece.transform.position.x, enemyPiece.GetComponent<PieceScript>().height + 1f, enemyPiece.transform.position.z);
        Quaternion rotation = Quaternion.identity; //ROTATE TOWARDS CAMERA???
        dogDeathText = Instantiate(dogDeathText, position, rotation);

        //wait a second
        yield return new WaitForSeconds(2f);

        //destroy
        Destroy(dogDeathText, 0f);

        yield return null;
    }

    public override void Defend(GameObject enemyPiece) 
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
    }
}
