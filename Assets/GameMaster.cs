using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    Player player;

    Button endTurnButton;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        player.yourTurn = true;

        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButton.onClick.AddListener(EndTurn);
    }

    void EndTurn()
    {
        player.yourTurn = false;

        //wait for board state
        //board = enemy.getMove(board);

        //update board (move enemy pieces)

        player.yourTurn = true;
    }
}