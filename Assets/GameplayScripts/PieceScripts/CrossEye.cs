using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossEye : PieceScript
{
    // Start is called before the first frame update
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
        float x = transform.position.x;
        float z = transform.position.z;
        GameObject spaceInFront;

        //moves diagonally towards farthest side


        moving = false;
        yield return null;
    }

    public override void Attack(GameObject enemyPiece) { }

    public override void Defend(GameObject enemyPiece) { }
}
