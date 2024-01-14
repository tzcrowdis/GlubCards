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

    static List<PieceScript> activePieceScripts = new List<PieceScript>();

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

        //init board
        board = new GameObject[5, 5];
    }

    void Update()
    {
        if (playerDone) //put in update to wait for piece coroutines/animations
        {
            //move players pieces

            foreach(PieceScript allPieces in activePieceScripts) 
            {
                if (!allPieces.updatedMaster)
                    allPieces.Move();
            }
            //find all player pieces
            //link their movement coroutines
            //start the coroutine of all linked pieces
            //set piecesMoved to true when completed

            if (playerPiecesMoved)
            {
                //wait for board state
                if (!enemyDone)
                {
                    board = enemy.getPlacements(board);
                    enemyDone = true;
                }
                
                //move enemy pieces
                //set enemy pieces moved to true when completed

                if (enemyPiecesMoved)
                {
                    player.StartTurn();
                    playerDone = false;
                    enemyDone = false;
                    playerPiecesMoved = false;
                    enemyPiecesMoved = false;
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
        activePieceScripts.Add(piece);
        Debug.Log($"Added piece script {activePieceScripts.Last()} to board");
    }

    public int[] GetPieceLocation(GameObject piece) 
    {
        return new int[] {(int)piece.transform.position.x, (int)piece.transform.position.y};
    }

}