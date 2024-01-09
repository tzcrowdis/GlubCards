using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    Player player;

    Button endTurnButton;

    string[] board; //grid that stores names of game objects

    Enemy enemy; 

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        player.StartTurn();

        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        enemy = GameObject.Find("Enemy").GetComponent<Enemy>();

        //init board
    }

    void EndTurn()
    {
        player.EndTurn();

        //wait for board state
        board = enemy.getPlacements(board);

        //update board (move enemy pieces)

        player.StartTurn();
    }
}