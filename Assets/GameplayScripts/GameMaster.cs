using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    Player player;
    bool playerDone;
    bool playerPiecesMoved;

    Enemy enemy;
    bool enemyDone;
    bool enemyPiecesMoved;

    Button endTurnButton;

    GameObject[,] board; //grid that stores game objects [should it just be names?]

    static List<PieceScript> activePlayerPieces = new List<PieceScript>();
    static List<PieceScript> activeEnemyPieces = new List<PieceScript>();
    public int pInd;
    public int eInd;

    public static GameMaster Instance { get; private set; } //good for one instance

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            UnityEngine.Object.Destroy(Instance.gameObject);

        player = GameObject.Find("Player").GetComponent<Player>();
        player.StartTurn();
        playerDone = false;

        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        enemy = GameObject.Find("Enemy").GetComponent<Enemy>();
        enemyDone = false;

        playerPiecesMoved = false;
        enemyPiecesMoved = false;

        board = new GameObject[5, 5]; //would need board init function if we incorporate environmental tiles

        pInd = 0;
        eInd = 0;
    }

    void Update()
    {
        if (playerDone)
        {
            //move all players pieces
            if (pInd < activePlayerPieces.Count)
            {
                if (!activePlayerPieces[pInd].moving)
                    StartCoroutine(activePlayerPieces[pInd].Move());
            }
            else
            {
                playerPiecesMoved = true;
            }

            if (playerPiecesMoved)
            {
                //wait for board state
                if (!enemyDone)
                {
                    board = enemy.getPlacements(board);
                    enemyDone = true;
                }

                //move all enemy pieces
                if (eInd < activeEnemyPieces.Count)
                {
                    if (!activeEnemyPieces[eInd].moving)
                        StartCoroutine(activeEnemyPieces[eInd].Move());
                }
                else
                {
                    enemyPiecesMoved = true;
                }

                //cleanup
                if (enemyPiecesMoved)
                {
                    player.StartTurn();
                    playerDone = false;
                    enemyDone = false;
                    playerPiecesMoved = false;
                    enemyPiecesMoved = false;
                    pInd = 0;
                    eInd = 0;
                }
            }
        }

    }

    void EndTurn()
    {
        player.EndTurn();
        playerDone = true;
    }

    public void SetPieceLocOnBoard(GameObject piece, Vector3 coord)
    {
        board[(int)coord.x - 1, (int)coord.z - 1] = piece;
    }

    public void InitializePiece(PieceScript piece)
    {
        if (piece.enemyPiece)
        {
            activeEnemyPieces.Add(piece);
            Debug.Log($"Added enemy piece {activeEnemyPieces.Last()} to board");
        }
        else
        {
            activePlayerPieces.Add(piece);
            Debug.Log($"Added enemy piece {activePlayerPieces.Last()} to board");
        }
    }

    public int[] GetPieceLocation(GameObject piece) 
    {
        return new int[] {(int)piece.transform.position.x, (int)piece.transform.position.y};
    }

}