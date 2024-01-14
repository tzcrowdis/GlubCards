using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class OWBossStarter : MonoBehaviour
{
    OWPlayerController player;
    
    Canvas interactMsg;
    Canvas engageMsg;

    Button[] buttons;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<OWPlayerController>();
        
        interactMsg = gameObject.transform.GetChild(0).GetComponent<Canvas>();
        engageMsg = gameObject.transform.GetChild(1).GetComponent<Canvas>();
        interactMsg.enabled = false;
        engageMsg.enabled = false;

        buttons = engageMsg.GetComponentsInChildren<Button>(); //assumes start fight button is higher in the hierarchy than walk away button
        buttons[0].onClick.AddListener(StartFight);
        buttons[1].onClick.AddListener(WalkAway);
    }

    private void OnTriggerEnter(Collider other)
    {
        interactMsg.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            //display boss information
            interactMsg.enabled = false;
            engageMsg.enabled = true;

            //lock player movement until decision
            player.locked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //remove all messages
        interactMsg.enabled = false;
        engageMsg.enabled = false;
    }

    void StartFight()
    {
        Debug.Log("Chose to fight " + gameObject.name);
    }

    void WalkAway()
    {
        Debug.Log("Walking away from " + gameObject.name);

        player.locked = false;
    }
}
