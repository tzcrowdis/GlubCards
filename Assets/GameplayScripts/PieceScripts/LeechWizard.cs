using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechWizard : PieceScript
{
    bool dodged;
    bool focusing;

    ParticleSystem dodgeSmoke;

    public override void Start()
    {
        hp = 1f;
        dmg = 1f;
        height = 0.16f;

        dodged = false;
        focusing = false;

        dodgeSmoke = GetComponentInChildren<ParticleSystem>();

        base.Start();
    }

    public override IEnumerator Act(System.Action onComplete)
    {
        moving = true;

        //places leech on most hp enemy opponent

        //determine juiciest enemy opponent
        PieceScript juiciestEnemy = null;
        if (!focusing)
        {
            float maxHp = 0f;
            for (int i = 0; i < GameMaster.Instance.board.GetLength(0); i++)
            {
                for (int j = 0; j < GameMaster.Instance.board.GetLength(1); j++)
                {
                    if (GameMaster.Instance.board[i, j] != null && (GameMaster.Instance.board[i, j].GetComponent<PieceScript>().enemyPiece ^ enemyPiece))
                    {
                        if (GameMaster.Instance.board[i, j].GetComponent<PieceScript>().hp > maxHp)
                        {
                            maxHp = GameMaster.Instance.board[i, j].GetComponent<PieceScript>().hp;
                            juiciestEnemy = GameMaster.Instance.board[i, j].GetComponent<PieceScript>();
                        }
                    }
                }
            }
        }

        //check if leech already attached
        //if not then attach leech
        if (juiciestEnemy == null)
            Debug.Log("No enemies with blood on board.");
        else if (juiciestEnemy.transform.Find("Leech") == null)
        {
            GameObject leechPrefab = Resources.Load("Attachments/Leech") as GameObject;
            leechPrefab = Instantiate(leechPrefab, new Vector3(0f, -1f, 0f), Quaternion.identity);
            yield return leechPrefab.GetComponent<Leech>().Attach(juiciestEnemy.gameObject);
            focusing = true;
        }

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
        if (dodged)
        {
            hp -= enemyPiece.GetComponent<PieceScript>().dmg;
            if (hp <= 0)
            {
                yield return DeathAnim();
                
                GameMaster.Instance.RemovePieceFromBoard(gameObject);
                Destroy(gameObject, 0f);
            }
        }
        else
        {
            if (GameMaster.Instance.board[(int)transform.position.x - 1, (int)transform.position.z] == null)
            {
                //dodge left
                Vector3 newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                GameMaster.Instance.SetPieceLocOnBoard(gameObject, transform.position, newPos);
                transform.position = newPos;
                dodged = true;

                //PLAY PARTICLE EFFECT
                dodgeSmoke.transform.position = new Vector3(newPos.x + 1, transform.position.y, transform.position.z);
                dodgeSmoke.Play();
                yield return new WaitForSeconds(0.5f);
                dodgeSmoke.Stop();
            }
            else if (GameMaster.Instance.board[(int)transform.position.x + 1, (int)transform.position.z] == null)
            {
                //dodge right
                Vector3 newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                GameMaster.Instance.SetPieceLocOnBoard(gameObject, transform.position, newPos);
                transform.position = newPos;
                dodged = true;

                //PLAY PARTICLE EFFECT
                dodgeSmoke.transform.position = new Vector3(newPos.x - 1, transform.position.y, transform.position.z);
                dodgeSmoke.Play();
                yield return new WaitForSeconds(0.5f);
                dodgeSmoke.Stop();
            }
        }

        yield return null;
    }
}
