using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRoad : MonoBehaviour
{
    [SerializeField]
    GridFollower follower;
    [SerializeField]
    Grid grid;
    Vector3Int StartingCell;
    Vector3Int EndingCell;
    Vector3Int LastAdjacent;
    bool roadMaking = false;

    [SerializeField]
    GameObject hiddenFollowerFollower;
    LineRenderer hiddenLine;
    [SerializeField]
    CellHead cellLeader;

    bool HoldBuilding;
    Coroutine currentBuilder;
    [SerializeField] AStar aStar;
    // Start is called before the first frame update
    void Start()
    {
        hiddenLine = hiddenFollowerFollower.GetComponent<LineRenderer>();
        hiddenFollowerFollower.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y-1);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        EndingCell = follower.getGridPos();
        hiddenFollowerFollower.transform.position = grid.CellToWorld(StartingCell);
        if(Input.GetMouseButtonDown(0)){
            hiddenFollowerFollower.SetActive(true);
            StartingCell = follower.getGridPos();
            roadMaking = true;
            hiddenLine.SetPosition(0,follower.getGridPos());
            hiddenLine.SetPosition(1,mouse);
        }
        if(Input.GetMouseButton(0)){
            hiddenLine.SetPosition(0,hiddenFollowerFollower.transform.position);
            hiddenLine.SetPosition(1,mouse);
            
            CheckDist();
        }
        if(roadMaking && Input.GetMouseButtonUp(0)){
            if(HoldBuilding){
                HoldBuilding = false;
                StopCoroutine(currentBuilder);
            }
            else{
                DetermineAndBuildRoad(StartingCell, EndingCell);
            }
            roadMaking = false;
            hiddenFollowerFollower.SetActive(false);
            hiddenLine.SetPosition(0,Vector3.down);
            hiddenLine.SetPosition(1,Vector3.down);
        }
    }
    void DetermineAndBuildRoad(Vector3Int start, Vector3Int end){
        float dist = Vector3Int.Distance(start,end);
        if(dist == 0){
            return;
        }
        Direction[] startEnd = DetermineDirection(start, end);
        cellLeader.updateOneWayCell(start,end,startEnd[0]);
        //cellLeader.updateCells(start,end,startEnd[0],startEnd[1]);
        StartingCell = end;

    }
    IEnumerator exceptBuildRoad(Vector3Int start, Vector3Int end){
        Debug.Log("ExceptionRoad fired off");
        float distance = Vector3.Distance(start,end);
        Vector3Int[] path;
        HoldBuilding = true;
        Vector3Int cStart = start;
        StartCoroutine(aStar.AStarCells(start,end));
        while(cStart != end){
            if(aStar.CellPath != null){
                path = aStar.CellPath;
                aStar.CellPath = null;
            for(int i = 1; i<path.Length;i++){
                DetermineAndBuildRoad(cStart,path[i]);
                cStart = path[i];
                yield return null;
            }
            /*
            angle = Vector3.Angle(cStart,end);
            Direction d = null;
            for(int i = 0; i < 8; i++){
                if(roundToAngle(angle, angleIncrements * i)){
                    d = Direction.intCast(i);
                    break;
                }
            }
            if(d == null){
                throw new ArgumentNullException();
            }
            cEnd = start + d.Translation;
            bool isLegal = cellLeader.updateOneWayCell(cStart,cEnd,d);
            int dClockwise = d.Index;
            while(!isLegal){
                dClockwise++;
                if(dClockwise == 8){
                    dClockwise = 0;
                }
                d = Direction.intCast(dClockwise);
                cEnd = cStart + Direction.intCast(dClockwise).Translation;
                isLegal = cellLeader.updateOneWayCell(cStart,cEnd,d);
            }
            cStart = cEnd;
            distance = Vector3.Distance(cStart,end);
            yield return new WaitForSeconds(2f);
            */
            }
            yield return null;
        }
        StartingCell = end;
        HoldBuilding = false;
       
    }
    float angleEpsilon = 22.5f;
    bool roundToAngle(float f,float target){
        return Mathf.Abs(f-target) < angleEpsilon;
    }
    /// <summary>
    /// Determines direction of two cells connection
    /// </summary>
    /// <param name="start">Start cell</param>
    /// <param name="end">End cell</param>
    /// <returns>Returns direction array of <c>Start</c> then <c>End</c></returns>
    /// 
    Direction[] DetermineDirection(Vector3Int start, Vector3Int end){
        int xDist = end.x - start.x;
        int zDist = end.z - start.z;
        if(xDist > 1) xDist = 1;
        if(zDist > 1) zDist = 1;
        if(xDist < -1) xDist = -1;
        if(zDist < -1) zDist = -1;
        Vector2Int dist = new Vector2Int(xDist,zDist);
        //TODO move
        if(dist.Equals(Vector2Int.up)){
            return new Direction[]{Direction.N,Direction.S};
        }
        else if(dist.Equals(-Vector2Int.up)){
            return new Direction[]{Direction.S,Direction.N};
        }
        else if(dist.Equals(Vector2Int.right)){
            return new Direction[]{Direction.E,Direction.W};
        }
        else if(dist.Equals(Vector2Int.left)){
            return new Direction[]{Direction.W,Direction.E};
        }
        else if(dist.Equals(new Vector2Int(1,1))){
            return new Direction[]{Direction.NE,Direction.SW};
        }
        else if(dist.Equals(new Vector2Int(-1,-1))){
            return new Direction[]{Direction.SW,Direction.NE};
        }
        else if(dist.Equals(new Vector2Int(-1,1))){
            return new Direction[]{Direction.NW,Direction.SE};
        }
        else if(dist.Equals(new Vector2Int(1,-1))){
            return new Direction[]{Direction.SE,Direction.NW};
        }
        throw new InvalidOperationException();
    }
    void CheckDist(){
        float dist = Vector3Int.Distance(StartingCell, EndingCell);
        if(!HoldBuilding){
            if(dist >= 2){
                HoldBuilding = true;
                if(dist > 3){
                    currentBuilder = StartCoroutine(exceptBuildRoad(StartingCell,EndingCell));
                }
                else{
                    DetermineAndBuildRoad(StartingCell, LastAdjacent);
                    HoldBuilding = false;
                }
            
            }
            else{
            LastAdjacent = EndingCell;
            }
        }

    }
}
