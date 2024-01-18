using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreContainer : MonoBehaviour
{
    Vector3[] pPositions;
    Vector3[] ePositions;
    Quaternion rotation;

    GameObject scorePiece;

    public int playerHp { get; private set; }
    public int enemyHp { get; private set; }

    void Start()
    {
        playerHp = 3;
        enemyHp = 3;
        
        pPositions = new Vector3[playerHp];
        ePositions = new Vector3[enemyHp];

        for (int i = 0; i < playerHp; i++) //assumes playerHp == enemyHp
        {
            pPositions[i] = new Vector3(UnityEngine.Random.Range(-0.25f * i, 0.25f * i), 7f, UnityEngine.Random.Range(0f, 0.2f));
            ePositions[i] = new Vector3(UnityEngine.Random.Range(6f - 0.25f * i, 6f + 0.25f), 7f, UnityEngine.Random.Range(5.5f, 5.7f));
        }

        scorePiece = Resources.Load("Dagger") as GameObject;
    }

    //UNCOMMENT TO TEST
    
    private void Update()
    {
        //testing
        /*
        IncrementScore(true);
        IncrementScore(true);
        IncrementScore(true);
        IncrementScore(false);
        IncrementScore(false);
        IncrementScore(false);
        */
    }
    

    public void IncrementScore(bool player)
    {
        if (player)
        {
            playerHp--;
            DropPiece(player);
        }
        else
        {
            enemyHp--;
            DropPiece(player);
        }
    }

    void DropPiece(bool player)
    {
        //controls randomish drop
        if (player)
        {
            rotation = Quaternion.Euler(UnityEngine.Random.Range(-85f, -95f), UnityEngine.Random.Range(85f, 95f), 0f);
            Instantiate(scorePiece, pPositions[playerHp], rotation, gameObject.transform);
        }
        else
        {
            rotation = Quaternion.Euler(UnityEngine.Random.Range(-85f, -95f), UnityEngine.Random.Range(85f, 95f), 0f);
            Instantiate(scorePiece, ePositions[enemyHp], rotation, gameObject.transform);
        }
    }
}
