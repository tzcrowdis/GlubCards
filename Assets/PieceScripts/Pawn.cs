using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Pawn : PieceScript
{
    public override void Start() 
    {
        //set any unique variables here
        Debug.Log($"Child script is {GetType()}");
        Debug.Log($"Parent script is {GetType().BaseType}");
        //Use base to call parent functions so generics get assigned
        base.Start();
    }

    public override IEnumerator Move() 
    {
        moving = true;

        //Unique logic to move would go here for every piece

        //pawns logic
        //get board position
        //check board one space ahead
        //move or attack
        float x = transform.position.x;
        float z = transform.position.z + 1;

        //move to the position
        Vector3 currentPos = transform.position;
        Vector3 nextPos = new Vector3(x, 0.5f, z);
        Quaternion currentRot = transform.rotation;
        Quaternion nextRot = Quaternion.Euler(0, 0, 0);
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, nextPos, t);
            transform.rotation = Quaternion.Lerp(currentRot, nextRot, t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = nextPos;
        transform.rotation = nextRot;

        moving = false;
        if (enemyPiece)
            GameMaster.Instance.eInd++;
        else
            GameMaster.Instance.pInd++;
        yield return null;
    }
}
