using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PieceScript : MonoBehaviour //handles administrative generic piece tasks
{
    Player player;

    bool selected;

    bool inPlay;

    public Light hoverLight;

    GameMaster master;
    bool updatedMaster;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        selected = false;

        hoverLight = transform.GetChild(0).gameObject.GetComponent<Light>();
        hoverLight.enabled = false;

        inPlay = false;

        master = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        updatedMaster = false;
    }

    private void OnMouseOver()
    {
        if (player.selectingPiece && !inPlay)
        {
            //highlight card
            hoverLight.enabled = true;

            if (Input.GetMouseButtonDown(0))
            {
                //tell player this was chosen
                selected = true;
                player.piece = gameObject.GetComponent<PieceScript>();
                inPlay = true;
            }
        }
    }

    private void OnMouseExit()
    {
        if (!selected)
        {
            hoverLight.enabled = false;
        }
    }

    public IEnumerator MoveToStart(Vector3 startPos)
    {
        if (!updatedMaster)
        {
            UpdateBoardPosition(startPos);
            updatedMaster = true;
        }

        //set y direction based on piece height
        startPos.y = 0.5f;
        
        //move to the position and rotate to face enemy
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        Quaternion startRot = Quaternion.Euler(0, 0, 0);
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, startPos, t);
            transform.rotation = Quaternion.Lerp(currentRot, startRot, t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
        transform.rotation = startRot;
        updatedMaster = false;
        yield return null;
    }

    void UpdateBoardPosition(Vector3 pos)
    {
        //tell game master pieces position on board
        master.AddPlayerPieceToBoard(gameObject, pos);
    }
}
