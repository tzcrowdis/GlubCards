using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BoomCoon : PieceScript
{
    private ParticleSystem explosion;
    private bool fired;

    public override void Start() 
    {
        hp = 2f;
        dmg = 5f;
        height = 0.16f;
        fired = false;

        explosion = GetComponentInChildren<ParticleSystem>();

        base.Start();
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        if (enemyPiece)
        {
            throw new Exception("Enemy piece logic not set up for BoomCoon");
        }
        moving = true;

        float x = transform.position.x;
        float z = transform.position.z;
        GameObject enemy;

        //Check if tile exists in front
        if (z + 1 < GameMaster.Instance.board.GetLength(1))
        {
            enemy = GetFirstPieceFront();

            if (!fired && enemy != null && enemy.GetComponent<PieceScript>().enemyPiece)
            {
                fired = true;
                yield return StartCoroutine(Attack(enemy));
            }
            else
            {
                z++;
                yield return Move(x, z);
            }
        }
        else
        {
            if (!fired)
            {
                PlayExplosion();
                fired = true;
                //attack enemy
                Debug.Log("Attacking Enemy Directly");
                GameMaster.Instance.enemy.TakeDmg(gameObject);
            }
            else
            {
                GameMaster.Instance.RemovePieceFromBoard(gameObject);
                Destroy(gameObject, 0f);
                onComplete = null;
            }            
        }

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
        PlayExplosion(enemyPiece.transform.position);        
        yield return enemyPiece.GetComponent<PieceScript>().Defend(gameObject);

        yield return null;
    }

    public override IEnumerator Defend(GameObject enemyPiece)
    {
        hp -= enemyPiece.GetComponent<PieceScript>().dmg;
        if (hp <= 0)
        {
            yield return DeathAnim();

            GameMaster.Instance.RemovePieceFromBoard(gameObject);
            Destroy(gameObject, 0f);
        }

        yield return null;
    }

    //returns the first piece in front or null
    GameObject GetFirstPieceFront()
    {
        GameObject[,] board = GameMaster.Instance.board;
        for (int i = (int)transform.position.z + 1; i < board.GetLength(1); i++)
        {
            if (board[(int)transform.position.x, i] != null)
                return board[(int)transform.position.x, i];

        }
        return null;
    }
    void PlayExplosion(Vector3 enemy)
    {
        Vector3 newPos = new(enemy.x,
                            explosion.transform.position.y,
                            enemy.z);
        explosion.transform.position = newPos;
        explosion.Play();
            
    }
    void PlayExplosion()
    {
        Vector3 newpos = new(explosion.transform.position.x,
                                explosion.transform.position.y,
                                GameMaster.Instance.board.GetLength(1) + 1);
        explosion.transform.position = newpos;
        explosion.Play();
    }
}
