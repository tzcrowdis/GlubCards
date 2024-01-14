using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
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

    [Serializable]
    public class ViewTransform
    {
        
        [HideInInspector] public String Name;
        public Views View;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [SerializeField] private List<ViewTransform> viewTransforms;
  


    // Start is called before the first frame update
    void Start()
    {
        
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
        MoveCameraToView(currentView);
    }

    public void ChangeCameraView(Views v) 
    {
        //return if camera is already there
        if (currentView == v)
            return;
        else
            currentView = v;
        MoveCameraToView(v);
    }

    //TODO - make this an animation
    private void MoveCameraToView(Views v)
    {   
        ViewTransform view = GetViewTform(v);
        
        if (view == null)
        {
            Debug.LogError($"Failed to move to camera view transform {v}, it does not exist.");
            return;
        } 

        transform.parent.SetPositionAndRotation
        (
            GetViewTform(v).Position, 
            Quaternion.Euler(GetViewTform(v).Rotation)
        );
    }

    private ViewTransform GetViewTform(Views v) 
    { 
        return viewTransforms.Find(i => i.Name ==  currentView.ToString()); 
    }


}
