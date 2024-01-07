using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public bool selectingPosition;
    public GameObject[] startGrid = new GameObject[5];

    public CardScript selectedCard;
    Vector3 startPos;
    
    void Start()
    {
        selectingPosition = false;
    }

    void Update()
    {
        //move cards to selected starting position
        if (selectingPosition)
        {
            for (int i = 0; i < startGrid.Length; i++)
            {
                if (startGrid[i].GetComponent<GridElemScript>().selected == true)
                {
                    startPos = startGrid[i].transform.position;
                    startPos.z = (float)-0.75;
                    selectedCard.hoverLight.enabled = false;
                    StartCoroutine(selectedCard.MoveToStart(startPos));
                }
            }
        }
    }
}
