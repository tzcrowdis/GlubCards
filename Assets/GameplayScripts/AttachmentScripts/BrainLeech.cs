using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainLeech : MonoBehaviour
{
    GameObject suckee;

    ParticleSystem blood;

    private void Start()
    {
        blood = GetComponentInChildren<ParticleSystem>();
    }

    //attached to piece by brain leech wizard
    //kills that piece once they reach their opponent
    public IEnumerator Attach(GameObject piece)
    {
        suckee = piece;

        //attach leech
        switch (piece.name.Remove(piece.name.Length - 7))
        {
            case "Soldier":
                transform.parent = piece.transform;
                transform.localPosition = new Vector3(-0.308f, 2.77f, -0.237f);
                transform.localRotation = Quaternion.Euler(-53, 90, 0); //should be -90 but getting flipped by 180 somewhere
                break;
            case "WW1Dog":
                Debug.Log("dog found");
                break;
            case "CrossEye":
                Debug.Log("cross eye found");
                break;
            case "TheVoid":
                Debug.Log("void found");
                break;
        }

        //change suckee's allegiance
        piece.GetComponent<PieceScript>().inPlay = true;
        suckee.GetComponent<PieceScript>().enemyPiece = !suckee.GetComponent<PieceScript>().enemyPiece;

        //add blood spurt
        yield return BloodSpurt();

        yield return null;
    }

    void Update()
    {
        if (suckee != null)
        {
            if (suckee.GetComponent<PieceScript>().enemyPiece && suckee.transform.position.z == 0)
            {
                StartCoroutine(Kill(suckee));
            }
            else if (!suckee.GetComponent<PieceScript>().enemyPiece && suckee.transform.position.z == GameMaster.Instance.board.GetLength(1) - 1)
            {
                StartCoroutine(Kill(suckee));
            }
        }
    }

    IEnumerator Kill(GameObject victim)
    {
        yield return new WaitForSeconds(0.1f);

        //play death animation

        GameMaster.Instance.RemovePieceFromBoard(victim);
        Destroy(victim, 0f);

        yield return null;
    }

    IEnumerator BloodSpurt()
    {
        yield return new WaitForSeconds(1f);

        Destroy(blood);

        yield return null;
    }
}
