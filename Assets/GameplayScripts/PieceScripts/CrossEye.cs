using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CrossEye : PieceScript
{
    bool left;
    int startRow;
    int endRow;

    public override void Start()
    {
        hp = 1f;
        dmg = 1f;
        height = 0.15f;

        if (enemyPiece)
        {
            startRow = GameMaster.Instance.board.GetLength(1) - 1;
            endRow = 0;
        }
        else
        {
            startRow = 0;
            endRow = GameMaster.Instance.board.GetLength(1) - 1;
        }

        base.Start();
    }

    //moves diagonally towards farthest side
    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;
        float x = transform.position.x;
        float z = transform.position.z;

        //determine next position
        GameObject spaceInFront = null;
        if (enemyPiece)
        {
            if (z == startRow) //get initial direction
            {
                if (x <= 2f)
                {
                    x++;
                    left = true;
                }
                else
                {
                    x--;
                    left = false;
                }
            }
            else
            {
                if (left)
                {
                    if (x + 1 >= GameMaster.Instance.board.GetLength(0))
                    {
                        //enemy piece change to right
                        x--;
                        left = false;
                    }
                    else
                        x++;
                }
                else
                {
                    if (x - 1 < 0)
                    {
                        //enemy piece change to right
                        x++;
                        left = true;
                    }
                    else
                        x--;
                }
            }
            z--;
        }
        else //player piece logic
        {
            if (z == startRow)
            {
                if (x <= 2f)
                {
                    x++;
                    left = false;
                }
                else
                {
                    x--;
                    left = true;
                }
            }
            else
            {
                if (left)
                {
                    if (x - 1f < 0f)
                    {
                        //player piece change to right
                        x++;
                        left = false;
                    }
                    else //keep moving left
                        x--;
                }
                else
                {
                    if (x + 1 >= GameMaster.Instance.board.GetLength(0)) //check right bound
                    {
                        //player piece change to left
                        x--;
                        left = true;
                    }
                    else //keep moving right
                        x++;
                }
            }
            z++;
        }

        //catch if we attacked player directly
        try
        {
            spaceInFront = GameMaster.Instance.board[(int)x, (int)z];
        }
        catch
        {
            //attack directly
            if (enemyPiece)
            {
                Debug.Log($"{gameObject.name} is attacking Player Directly");
                GameMaster.Instance.player.TakeDmg(gameObject);
            }
            else
            {
                Debug.Log($"{gameObject.name} is attacking Enemy Directly");
                GameMaster.Instance.enemy.TakeDmg(gameObject);
            }
            spaceInFront = null;
            x = transform.position.x;
            z = transform.position.z;
        }

        //attack or wait if obstructed
        if (spaceInFront != null) 
        {
            if (spaceInFront.GetComponent<PieceScript>().enemyPiece ^ enemyPiece) //XOR enforces pieces aren't on the same side
                yield return Attack(spaceInFront);

            //dont move if attack
            x = transform.position.x;
            z = transform.position.z;
        }

        yield return Move(x, z);

        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public override IEnumerator Move(float x, float z)
    {
        Vector3 nextPos = new Vector3(x, height, z);
        Vector3 currentPos = transform.position;
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, nextPos, t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = nextPos;
        yield return null;
    }

    public override IEnumerator Attack(GameObject enemyPiece) 
    {
        yield return enemyPiece.GetComponent<PieceScript>().Defend(gameObject);

        yield return null;
    }

    public override IEnumerator Defend(GameObject enemy) 
    {
        hp -= enemy.GetComponent<PieceScript>().dmg;
        if (hp <= 0)
        {
            yield return DeathAnim();

            GameMaster.Instance.RemovePieceFromBoard(gameObject);
            Destroy(gameObject, 0f);
        }

        yield return null;
    }
}
