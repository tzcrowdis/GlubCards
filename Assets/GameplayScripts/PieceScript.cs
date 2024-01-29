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

    Rigidbody rb;
    bool falling;

    Canvas infoCanvas;
    bool displayInfo;
    float infoDelay;
    float t;

    //public Light hoverLight;
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

    public virtual IEnumerator Move(float x, float z) { yield return null; }

    public virtual IEnumerator Attack(GameObject enemyPiece) { yield return null; }

    public virtual IEnumerator Defend(GameObject enemyPiece) { yield return null; }

    public virtual float GetStartPositionZ()
    {
        return 0f; //override for pieces that don't start at 0
    }


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

        //hoverLight = transform.GetChild(0).gameObject.GetComponent<Light>();
        //hoverLight.enabled = false;

        hovering = false;
        hoverHeight = 0.25f;
        upSpeed = 0.01f;
        hoverRotation = 180f;
        rotSpeed = 10f;

        if (!enemyPiece)
            falling = true;
        rb = GetComponent<Rigidbody>();
        if (enemyPiece)
            Destroy(rb);

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
            if (!falling & !enemyPiece && (!selected || inPlay))
            {
                //put back
                if (transform.position.y > 0f && transform.rotation.eulerAngles.y > 0f)
                {
                    transform.position -= new Vector3(0f, upSpeed, 0f);
                    transform.Rotate(new Vector3(0f, -rotSpeed, 0f));
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
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

                //Initialize the piece on the board NOW IN MOVE TO START
            }
        }

        //add right click for info box???
    }

    private void OnMouseExit()
    {
        displayInfo = false;
        infoCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        t = 0f;

        hovering = false;
        
        if (!selected && !enemyPiece)
        {
            //hoverLight.enabled = false;
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

        //free slot
        BagOfPieces.Instance.activeSlots[(int)currentPos.x, -(int)currentPos.z - 1] = 0;

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

    void OnCollisionEnter(Collision collision) //make virtual for mirror???
    {
        if (falling)
        {
            //stop rigid body
            Destroy(rb);

            //move to open slot
            float x, z;
            (x, z) = BagOfPieces.Instance.GetNearestOpenSlot(transform.position.x, -(int)transform.position.z - 1);

            if (x < float.MaxValue & z < float.MaxValue)
            {
                StartCoroutine(MoveToSlot(x, -(z + 1))); //convert z to world coord
            }

            //WAIT FOR COROUTINE???

            falling = false;
        }
    }

    IEnumerator MoveToSlot(float x, float z)
    {
        //move to the slot and rotate to face board
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        Vector3 slotPos = new Vector3(x, 0f, z);
        Quaternion slotRot = Quaternion.Euler(0, 0, 0);
        float t = 0;
        float endTime = 1;
        float speed = 2f;

        while (t < endTime)
        {
            transform.position = Vector3.Lerp(currentPos, slotPos, speed * t);
            transform.rotation = Quaternion.Lerp(currentRot, slotRot, speed * t);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = slotPos;
        transform.rotation = slotRot;
        yield return null;
    }
}
