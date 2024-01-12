using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; //for testing

public class Enemy : MonoBehaviour
{
    TextMeshProUGUI turnText;

    void Start()
    {
        turnText = GameObject.Find("Turn Visualization").GetComponent<TextMeshProUGUI>();
        turnText.alpha = 0f;
    }

    public GameObject[,] getPlacements(GameObject[,] board)
    {
        //fill in AI to choose piece placements based on board state

        turnText.alpha = 1f;
        StartCoroutine(FadeText());

        return board;
    }

    IEnumerator FadeText()
    {
        for (float alpha = 1f; alpha >= 0; alpha -= 0.01f)
        {
            turnText.alpha = alpha;
            yield return null;
        }

        turnText.alpha = 0f;
        yield return null;
    }
}
