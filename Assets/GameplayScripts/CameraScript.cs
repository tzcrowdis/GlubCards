using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Build;
using UnityEngine;

//Using this so I can see and adjust the camera angles without being in-game
[ExecuteAlways]
public class CameraScript : MonoBehaviour
{
    public enum Views
    {
        Board_View,
        Top_View,
        Bag_View

    };

    [SerializeField] Views currentView = Views.Board_View;

    [Min(0.01f)]
    [SerializeField] float CameraSpeed;

    [Serializable]
    public class ViewTransform
    {
        
        [HideInInspector] public String Name;
        public Views View;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [SerializeField] private List<ViewTransform> viewTransforms;

    bool moving;

    public static CameraScript Instance { get; private set; }

    

    // Start is called before the first frame update
    void Start()
    {
        moving = false;

        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Called when camera is changed in editor
    void OnValidate()
    { 
        for (int i = 0; i < viewTransforms.Count; i++) 
        {   
            viewTransforms[i].Name = viewTransforms[i].View.ToString();
        }
        ChangeCameraView(currentView);
    }

    public void ChangeCameraView(Views v) 
    {
        ViewTransform view = GetViewTform(v);
        if (view == null)
        {
            Debug.LogError($"Failed to move to camera view transform {v}, it does not exist.");
            return;
        }

        if (moving)
            return;

        StartCoroutine(MoveCameraToView(v));
    }

    private IEnumerator MoveCameraToView(Views v)
    {   
        moving = true;
        
        //move to the position and rotate to face enemy
        Vector3 currentPos = transform.parent.position;
        Quaternion currentRot = transform.parent.rotation;
        ViewTransform destination = GetViewTform(v); 

        float t = 0;
        float endTime = 1;

        while (t < endTime)
        {
            transform.parent.position = Vector3.Lerp(currentPos, destination.Position, t);
            transform.parent.rotation = Quaternion.Lerp(currentRot, Quaternion.Euler(destination.Rotation), t);
            t += Time.deltaTime * CameraSpeed;
            yield return null;
        }

        transform.parent.position = destination.Position;
        transform.parent.rotation = Quaternion.Euler(destination.Rotation);
        moving = false;
        yield return null;
    }

    private ViewTransform GetViewTform(Views v) 
    { 
        return viewTransforms.Find(i => i.Name ==  v.ToString()); 
    }


}
