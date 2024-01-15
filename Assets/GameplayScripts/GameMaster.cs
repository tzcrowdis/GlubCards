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
    public int psInd;

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

        psInd = 0;
    }

    void Update()
    {
        if (playerDone) //put in update to wait for piece coroutines/animations
        {
            //move all players pieces
            if (psInd < activePieceScripts.Count)
            {
                if (!activePieceScripts[psInd].moving)
                    StartCoroutine(activePieceScripts[psInd].Move());
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
                //set enemy pieces moved to true when completed

                if (enemyPiecesMoved)
                {
                    player.StartTurn();
                    playerDone = false;
                    enemyDone = false;
                    playerPiecesMoved = false;
                    enemyPiecesMoved = false;
                    psInd = 0;
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