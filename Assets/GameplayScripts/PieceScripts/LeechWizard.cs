using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechWizard : PieceScript
{
    public override void Start()
    {
        hp = 1f;
        dmg = 1f;
        height = 0.16f;

        base.Start();
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;

        //places leeches on strongest enemy opponent

        //determine strongest enemy opponent


        //check if leech already attached
        //if not then attach leech

        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public override IEnumerator Move(float x, float z) { yield return null; }

    public override IEnumerator Attack(GameObject enemyPiece) { yield return null; }

    public override IEnumerator Defend(GameObject enemyPiece) 
    { 
        //dodge first attack
        //by moving left or right if free space
        //otherwise die
        
        yield return null;
    }
}
