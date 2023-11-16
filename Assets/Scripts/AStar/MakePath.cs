using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class MakePath : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] GameObject selector;
    [SerializeField] CellHead cellLead;
    [SerializeField] AStar aStar;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    Vector3Int? beginning;
    Vector3Int? destination;
    bool noDoubleOps = false;
    [SerializeField] GameObject debugCar;
    [SerializeField] GameObject debugCarSpawner;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !beginning.HasValue){
            Vector3Int cellInQuestion = grid.WorldToCell(selector.transform.position);
            if(cellLead.hasCell(cellInQuestion)){
                beginning = cellInQuestion;
            }
            else{
                Debug.LogWarning("BadPlacement");
            }
            noDoubleOps = true;
        }
        if(Input.GetMouseButtonUp(0)){
            noDoubleOps = false;
        }
        if(Input.GetMouseButtonDown(1) && beginning.HasValue){
            beginning = null;
        }
        if(Input.GetMouseButtonDown(0) && beginning.HasValue && !noDoubleOps){
            Vector3Int cellInQuestion = grid.WorldToCell(selector.transform.position);
            if(cellLead.hasCell(cellInQuestion)){
                destination = cellInQuestion;
            }
            else{
                Debug.LogWarning("BadPlacement");
                beginning = null;
            }
            noDoubleOps = true;
        }
        if(beginning.HasValue && destination.HasValue){
            PathingCell[] path = aStar.CompleteCoAstar(beginning.Value,destination.Value);
            if(path != null){
                beginning=null;
                destination=null;
                Debug10Cars(path);
                path = null;
            }
            
            
        }
    }
    void Debug10Cars(PathingCell[] path){
        var ob = Instantiate(debugCarSpawner,path[0].pos,Quaternion.identity);
        var cS = ob.GetComponent<CarStorage>();
        cS.Setup(1,path,maxSpeed);
    }
}
