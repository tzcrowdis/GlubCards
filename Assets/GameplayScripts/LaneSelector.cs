using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneSelector : MonoBehaviour
{
    ParticleSystem hoverEffect;
    
    public bool selected;
    public bool on;

    Player player;
    
    void Start()
    {
        //get particle system
        hoverEffect = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        var emission = hoverEffect.emission;
        emission.enabled = false;

        selected = false;
        on = false;

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void OnMouseOver()
    {
        if (on)
        {
            //enable particles
            var emission = hoverEffect.emission;
            emission.enabled = true;

            //get z location for emission from piece
            if (player.piece != null)
                hoverEffect.gameObject.transform.position = new Vector3(hoverEffect.gameObject.transform.position.x,
                                                                            hoverEffect.gameObject.transform.position.y,
                                                                            player.piece.GetStartPositionZ());

            if (Input.GetMouseButtonDown(0))
            {
                selected = true;
            }
        }
    }

    void OnMouseExit()
    {
        //disable particles
        var emission = hoverEffect.emission;
        emission.enabled = false;
    }
}
