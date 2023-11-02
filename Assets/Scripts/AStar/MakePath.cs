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
                StartCoroutine(Debug10Cars(path));
                path = null;
            }
            
            
        }
    }
    IEnumerator Debug10Cars(PathingCell[] path){
        int count = 0;
        while(count < 10){
            DebugCar(path);
            yield return new WaitForSeconds(.1f);
            count ++;
        }

    }
    /*
    private async void beginAStar(){
            Vector3[] RealPath = null;
            try{
                Vector3Int[] path = await aStar.Pathfind(beginning.Value,destination.Value);
                RealPath = MakeRealPath(path);
                String vec3 = "";
                foreach(Vector3 p in RealPath){
                    vec3 += p.ToString() + " ";
                }
                Debug.Log(vec3);
                DebugCar(RealPath);
            }
            catch(Exception e){
                Debug.LogException(e);
            }
            beginning = null;
            destination = null;
    }
    */
    private void DebugCar(PathingCell[] path){
        PathingCell start = path[0];
        GameObject car = Instantiate(debugCar,start.pos,Quaternion.identity);
        float initRot = start.NextDirection.Index * 45f;
        car.transform.eulerAngles = new Vector3(0,initRot,0);
        CarBehavior carB = car.GetComponent<CarBehavior>();
        float speed = UnityEngine.Random.Range(minSpeed,maxSpeed);
        carB.setPath(path,speed);
    }
    /*
    private Vector3[] MakeRealPath(PathingCell[] path)
    {
        if(path.Length == 0){
            return null;
        }
        Vector3[] ActualPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i ++){
            ActualPath[i] = grid.CellToWorld(path[i]);
        }
        return ActualPath;
    }
    */
}
