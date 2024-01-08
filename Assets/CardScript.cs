using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CardScript : MonoBehaviour
{
    Player player;
    bool selected;
    public Light hoverLight;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        selected = false;

        hoverLight = transform.GetChild(0).gameObject.GetComponent<Light>();
        hoverLight.enabled = false;
    }

    private void OnMouseOver()
    {
        //highlight card
        hoverLight.enabled = true;
        
        if (Input.GetMouseButtonDown(0))
        {
            //tell game master this was chosen
            selected = true;
            player.selectingPosition = true;
            player.piece = gameObject.GetComponent<CardScript>();
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
        //move to the position and rotate to (90, 0, 0)
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        Quaternion startRot = Quaternion.Euler(90, 0, 0);
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
        yield return null;
    }
}
