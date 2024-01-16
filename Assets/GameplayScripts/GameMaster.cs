using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    string boss; //set by script that starts levels
    
    Player player;
    bool playerDone;
    bool playerPiecesMoved;

    Enemy enemy;
    bool enemyDone;
    bool enemyPiecesMoved;

    Button endTurnButton;

    [HideInInspector]
    public GameObject[,] board;

    static List<PieceScript> activePlayerPieces = new List<PieceScript>();
    static List<PieceScript> activeEnemyPieces = new List<PieceScript>();
    public int pInd;
    public int eInd;

    public static GameMaster Instance { get; private set; }

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

        boss = "Imbecile"; //TEMPORARY
        enemy = GameObject.Find(boss).GetComponent<Enemy>(); //MAKE SURE BOSS NAME CAN BE PASSED
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
                    board = enemy.GetPlacements(board);
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
                    /* CHECKS IF BOARD UPDATED PROPERLY
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (board[i, j] != null)
                                Debug.Log($"({i}, {j}) {board[i, j].name}");
                            else
                                Debug.Log($"({i}, {j}) Empty");
                        }
                    }
                    */
                    
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

    public void SetPieceLocOnBoard(GameObject piece, Vector3 oldCoord, Vector3 coord)
    {
        if (oldCoord.z >= 0) //may need to update this check
            board[(int)oldCoord.x - 1, (int)oldCoord.z - 1] = null;
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
        return new int[] {(int)piece.transform.position.x, (int)piece.transform.position.z};
    }
}