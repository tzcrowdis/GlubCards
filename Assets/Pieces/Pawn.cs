using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

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

    public override void Move() 
    {
        //Unique logic to move would go here for every piece
        StartCoroutine(MoveToPosition(new Vector3(transform.position.x,0.5f,transform.position.z + 1f)));
    }
    
}
