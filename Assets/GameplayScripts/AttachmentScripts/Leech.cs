using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Leech : MonoBehaviour
{
    GameObject suckee;

    ParticleSystem blood;

    private void Start()
    {
        blood = GetComponentInChildren<ParticleSystem>();
    }

    //attached to piece by wizard leech
    //kills that piece once they reach their opponent
    public IEnumerator Attach(GameObject piece)
    {
        suckee = piece;
        
        switch (piece.name.Remove(piece.name.Length - 7))
        {
            case "Soldier":
                transform.parent = piece.transform;
                transform.localPosition = new Vector3(0.6f, 1.3f, -0.12f);
                transform.localRotation = Quaternion.Euler(-90, -90, 0);
                //add blood spurt???
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

        //add blood spurt
        yield return BloodSpurt();
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
        yield return new WaitForSeconds(0.2f);

        //Destroy(blood);
        blood.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        yield return null;
    }
}
