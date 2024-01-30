using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaneSelector : MonoBehaviour
{
    ParticleSystem hoverEffect;

    GameObject holo;
    
    public bool selected;
    public bool on;
    public Material transparentMat;

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

            /*
            //enable particles
            var emission = hoverEffect.emission;
            emission.enabled = true;

            //get z location for emission from piece
            if (player.piece != null)
                hoverEffect.gameObject.transform.position = new Vector3(hoverEffect.gameObject.transform.position.x,
                                                                            hoverEffect.gameObject.transform.position.y,
                                                                            player.piece.GetStartPositionZ());
            */

            if (holo == null)
                CreateHoverObj();

            if (Input.GetMouseButtonDown(0))
            {
                selected = true;
                OnMouseExit();
            }
        }

    }

    void OnMouseExit()
    {
        //disable particles
        var emission = hoverEffect.emission;
        emission.enabled = false;

        //Coroutine will wait until piece isn't moving to destroy
        if (holo != null)
            Destroy(holo);
    }

    void TurnOff()
    {
        
    }

    //Kinda innefficient, creates a new object every hover but works
    void CreateHoverObj()
    {
        //Create object
        Debug.Log("No hover object exists, creating...");
        holo = new GameObject("holo");

        if (player.piece != null)
        {
            GameObject piece = player.piece.gameObject;
            //Create a ghost piece above the tile
            Vector3 spawnPosition = transform.position + new Vector3(0, 0.5f, player.piece.GetStartPositionZ() - 2f);
            holo = Instantiate(piece, spawnPosition, Quaternion.identity);

            //Remove scripting component
            Destroy(holo.GetComponent<PieceScript>());

            //Remove collider component (prevents overriding mouse hover)
            Destroy(holo.GetComponent<Collider>());

            //Make the ghost piece transparent
            Renderer[] renderers = holo.GetComponentsInChildren<MeshRenderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    Color color = renderer.materials[i].color;
                    color.a = 0.5f;
                    renderer.materials[i].CopyPropertiesFromMaterial(transparentMat);
                    renderer.materials[i].SetColor("_Color", color);
                }
            }

           
        }
    }
}
