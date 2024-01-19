using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    string boss; //set by script that starts levels
    
    public Player player;
    bool playerDone;
    bool playerPiecesMoved;

    public Enemy enemy;
    bool enemyDone;
    bool enemyPiecesMoved;

    Button endTurnButton;
    Button restartButton;

    public GameObject[,] board;

    static List<PieceScript> activePlayerPieces = new List<PieceScript>();
    static List<PieceScript> activeEnemyPieces = new List<PieceScript>();
    public int pInd;
    public int eInd;
    Vector3 startPos;

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

        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        restartButton.onClick.AddListener(RestartLevel);
        restartButton.gameObject.SetActive(false);

        boss = "Imbecile"; //TEMPORARY
        enemy = GameObject.Find(boss).GetComponent<Enemy>();
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
                {
                    startPos = activePlayerPieces[pInd].transform.position;
                    //start coroutine, runs callback function when done
                    StartCoroutine(activePlayerPieces[pInd].Move(() => {
                        SetPieceLocOnBoard(activePlayerPieces[pInd].gameObject, startPos, activePlayerPieces[pInd].transform.position);
                        pInd++;
                    }));
                }
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
                else if (eInd < activeEnemyPieces.Count)
                {
                    if (!activeEnemyPieces[eInd].moving)
                    {
                        startPos = activeEnemyPieces[eInd].transform.position;
                        //start coroutine, runs callback function when done
                        StartCoroutine(activeEnemyPieces[eInd].Move(() => {
                            SetPieceLocOnBoard(activeEnemyPieces[eInd].gameObject, startPos, activeEnemyPieces[eInd].transform.position);
                            eInd++;
                        }));
                    }
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

    public void RemovePieceFromBoard(GameObject dying)
    {
        board[(int)dying.transform.position.x - 1, (int)dying.transform.position.z - 1] = null;

        if (dying.GetComponent<PieceScript>().enemyPiece)
            activeEnemyPieces.Remove(dying.GetComponent<PieceScript>());
        else
            activePlayerPieces.Remove(dying.GetComponent<PieceScript>());
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
            Debug.Log($"Added player piece {activePlayerPieces.Last()} to board");
        }
    }

    public int[] GetPieceLocation(GameObject piece) 
    {
        return new int[] {(int)piece.transform.position.x, (int)piece.transform.position.z};
    }

    public void UpdateScore(GameObject player)
    {
        ScoreContainer score = GameObject.Find("ScoreContainer").GetComponent<ScoreContainer>();
        if (player.name == "Player")
        {
            score.IncrementScore(true);
            if (score.playerHp == 0)
            {
                //enemy wins
                Debug.Log("Loser");
                playerDone = false; //don't continue last turn
                BagOfPieces.Instance.DisableBag();
                restartButton.gameObject.SetActive(true);
                endTurnButton.gameObject.SetActive(false);
            }
        }
        else
        {
            score.IncrementScore(false);
            if (score.enemyHp == 0)
            {
                //player wins
                Debug.Log("Winner");
                playerDone = false; //don't continue last turn
                BagOfPieces.Instance.DisableBag();
                restartButton.gameObject.SetActive(true);
                endTurnButton.gameObject.SetActive(false);
            }
        }
    }

    private void RestartLevel()
    {
        //reloads scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}