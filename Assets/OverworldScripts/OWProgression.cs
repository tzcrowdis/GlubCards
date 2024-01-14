using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class OWProgression : MonoBehaviour
{
    //controls progression of the overworld based on player choices
    //will have to rework for new events

    bool[] finalBosses;

    Transform divider;

    private void Start()
    {
        finalBosses = new bool[4];
        ReadGameEvents();

        //set dividers moving
        for (int i = 0; i < finalBosses.Length - 1; i++) //minus 1 bc no divider for final final boss
        {
            if (finalBosses[i])
            {
                divider = GameObject.Find("Divider" + i.ToString()).transform;
                StartCoroutine(RaiseDivider(divider));
            }
        }
    }

    void ReadGameEvents()
    {
        try
        {
            using (StreamReader sr = new StreamReader("Assets/GameFiles/GameEvents.txt"))
            {
                string status;
                int ind = 0;
                while ((status = sr.ReadLine()) != null)
                {
                    if (status.Split("=")[1] == "true")
                        finalBosses[ind] = true;
                    else
                        finalBosses[ind] = false;
                    ind++;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Game Events file could not be read:");
            Debug.Log(e.Message);
        }
    }

    IEnumerator RaiseDivider(Transform div)
    {
        //move out of view
        Vector3 startPos = div.position;
        Vector3 endPos = new Vector3(0f, 10f, 0f);
        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            div.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime;
            yield return null;
        }

        div.position = endPos;
        yield return null;
    }
}
