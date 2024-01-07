using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridElemScript : MonoBehaviour
{
    ParticleSystem hoverEffect;
    
    public bool selected;
    
    void Start()
    {
        //get particle system
        hoverEffect = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        var emission = hoverEffect.emission;
        emission.enabled = false;

        selected = false;
    }

    void OnMouseOver()
    {
        //enable particles
        var emission = hoverEffect.emission;
        emission.enabled = true;

        if (Input.GetMouseButtonDown(0))
        {
            selected = true;
        }
    }

    void OnMouseExit()
    {
        //disable particles
        var emission = hoverEffect.emission;
        emission.enabled = false;
    }
}
