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
        //test move piece, in reality this should be called once per turn
        foreach(PieceScript allPieces in activePieceScripts) 
        {
            if (!allPieces.updatedMaster)
                allPieces.Move();
        }
    }

    void EndTurn()
    {
        player.EndTurn();
        playerDone = true;
    }

    public void AddPlayerPieceToBoard(GameObject piece, Vector3 coord)
    {
        board[(int)coord.x - 1, (int)coord.z - 1] = piece;
        //keep track of all active piece scripts
        activePieceScripts.Add(piece.GetComponent<PieceScript>());
        Debug.Log($"Added piece script {activePieceScripts.Last()} to board");
    }

    public int[] GetPieceLocation(GameObject piece) {
        //this might be risky but works in theory
        return new int[] {(int)piece.transform.position.x - 1, (int)piece.transform.position.y - 1};
    }

    public Vector3 GetBoardPosition(int[] coords) {
        return board[coords[0], coords[1]].transform.position;
    }

}