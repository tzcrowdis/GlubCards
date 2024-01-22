using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Scripting.APIUpdating;

public abstract class PieceScript : MonoBehaviour //handles administrative generic piece tasks
{
    //basic gameplay variables
    public float hp { get; protected set; }
    public float dmg { get; protected set; }

    //management variables
    Player player;
    bool selected;
    bool inPlay;
    Canvas infoCanvas;
    public Light hoverLight;
    public bool moving;
    public bool enemyPiece; //set when prefab is instantiated
    public bool updatedMaster {get; private set;}
    public float height { get; protected set; }

    public virtual void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        selected = false;

        infoCanvas = gameObject.GetComponentInChildren<Canvas>();
        infoCanvas.enabled = false;

        hoverLight = transform.GetChild(0).gameObject.GetComponent<Light>();
        hoverLight.enabled = false;

        inPlay = false;

        updatedMaster = false;

        moving = false;
    }

    //individual piece methods
    public virtual void Defend(GameObject enemyPiece) { }

    public virtual void Attack(GameObject enemyPiece) { }

    public virtual IEnumerator Move(float x, float z)
    {
        yield return null;
    }

    public virtual IEnumerator Act(System.Action onComplete)
    {
        moving = true;
        //individual logic here
        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    //every piece methods
    void OnMouseOver()
    {
        infoCanvas.enabled = true;
        
        if (player.selectingPiece && !inPlay && !enemyPiece)
        {
            //highlight card
            hoverLight.enabled = true;

            if (Input.GetMouseButtonDown(0))
            {
                //tell player this was chosen
                selected = true;
                player.piece = gameObject.GetComponent<PieceScript>();

                //Initialize the piece on the board
                GameMaster.Instance.InitializePiece(this);
                inPlay = true;
            }
        }
    }

    private void OnMouseExit()
    {
        infoCanvas.enabled = false;
        
        if (!selected && !enemyPiece)
        {
            hoverLight.enabled = false;
        }
    }

    public IEnumerator MoveToStart(Vector3 startPos)
    {
        if (!updatedMaster)
        {
            UpdateBoardPosition(transform.position, startPos);
            updatedMaster = true;
        }

        //move to the position and rotate to face enemy
        startPos.y = height;
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        Quaternion startRot = Quaternion.Euler(0, 180, 0); //changed y rotation
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
        updatedMaster = false;
        yield return null;
    }

    public void UpdateBoardPosition(Vector3 oldPos, Vector3 pos)
    {
        GameMaster.Instance.SetPieceLocOnBoard(gameObject, oldPos, pos);
    }

}
