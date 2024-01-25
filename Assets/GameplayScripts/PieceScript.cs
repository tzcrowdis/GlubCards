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

    bool hovering;
    float hoverHeight; //y
    float upSpeed;
    float hoverRotation; //y
    float rotSpeed;

    Canvas infoCanvas;
    bool displayInfo;
    float infoDelay;
    float t;

    public Light hoverLight;
    public bool moving;
    public bool enemyPiece; //set when prefab is instantiated
    public bool updatedMaster {get; private set;}
    public float height { get; protected set; }


    //INDIVIDUAL PIECE METHODS
    public virtual IEnumerator Act(System.Action onComplete)
    {
        moving = true;
        //individual logic here
        moving = false;
        onComplete?.Invoke();
        yield return null;
    }

    public virtual IEnumerator Move(float x, float z)
    {
        yield return null;
    }

    public virtual void Attack(GameObject enemyPiece) { }

    public virtual void Defend(GameObject enemyPiece) { }


    //COMMON PIECE METHODS
    public virtual void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        selected = false;

        infoCanvas = gameObject.GetComponentInChildren<Canvas>();
        infoCanvas.transform.Rotate(new Vector3(0f, 180f, 0f));
        infoCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        displayInfo = false;
        infoDelay = 1.5f;
        t = 0f;

        hoverLight = transform.GetChild(0).gameObject.GetComponent<Light>();
        hoverLight.enabled = false;

        hovering = false;
        hoverHeight = 0.25f + height;
        upSpeed = 0.01f;
        hoverRotation = 180f;
        rotSpeed = 1f;

        inPlay = false;

        updatedMaster = false;

        moving = false;
    }

    void Update()
    {
        if (displayInfo)
        {
            t += Time.deltaTime;
            if (t > infoDelay)
            {
                infoCanvas.GetComponent<CanvasGroup>().alpha = 1f;
            }
            else if (t > infoDelay * 0.75f)
            {
                infoCanvas.GetComponent<CanvasGroup>().alpha += 0.025f;
            }
        }
        
        if ((hovering || selected) && !inPlay)
        {
            //raise and twist
            if (transform.position.y < hoverHeight && transform.rotation.eulerAngles.y < hoverRotation)
            {
                transform.position += new Vector3(0f, upSpeed, 0f);
                transform.Rotate(new Vector3(0f, rotSpeed, 0f));
            }
            else
            {
                transform.position = new Vector3(transform.position.x, hoverHeight, transform.position.z);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, hoverRotation, transform.rotation.eulerAngles.z);
            }
        }
        else 
        {
            if (!enemyPiece && (!selected || inPlay))
            {
                //put back
                if (transform.position.y > height && transform.rotation.eulerAngles.y > 0f)
                {
                    transform.position -= new Vector3(0f, upSpeed, 0f);
                    transform.Rotate(new Vector3(0f, -rotSpeed, 0f));
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, height, transform.position.z);
                    transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    void OnMouseOver()
    {
        displayInfo = true;
        
        if (player.selectingPiece && !inPlay && !enemyPiece)
        {
            //highlight piece
            //hoverLight.enabled = true;
            hovering = true;

            if (Input.GetMouseButtonDown(0))
            {
                //tell player this was chosen
                selected = true;
                player.piece = gameObject.GetComponent<PieceScript>();

                //display info box correctly on board
                infoCanvas.GetComponent<CanvasGroup>().alpha = 0f;
                infoCanvas.transform.Rotate(new Vector3(0f, 180f, 0f));

                //Initialize the piece on the board
                //GameMaster.Instance.InitializePiece(this);
                //inPlay = true;
            }
        }
    }

    private void OnMouseExit()
    {
        displayInfo = false;
        infoCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        t = 0f;

        hovering = false;
        
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
        Quaternion startRot = Quaternion.Euler(0, 0, 0); //MAKE SURE ALL PIECES ARE IMPORTED CORRECTLY
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

        //Initialize the piece on the board
        GameMaster.Instance.InitializePiece(this);
        inPlay = true;

        yield return null;
    }

    public void UpdateBoardPosition(Vector3 oldPos, Vector3 pos)
    {
        GameMaster.Instance.SetPieceLocOnBoard(gameObject, oldPos, pos);
    }

}
