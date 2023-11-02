/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
public class newStarCoord{
    public readonly GenericNodeGroup grouping;
    public float fCost = 0f;
    public float gCost = 0f;
    public float hCost = 0f;
    public newStarCoord parent;
    public newStarCoord(GenericNodeGroup @new) {
        this.grouping = @new;
        parent = null;
    }

}
public class AStarGroups : MonoBehaviour
{
    [SerializeField]
    CellHead cellHead;

    List<string> workingAStar = new List<string>();
    Dictionary<string,Vector3Int[]> finishedPaths = new Dictionary<string,Vector3Int[]>();
    /// <summary>
    /// MUST be constantly called or on an interval for the path to be retrieved.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public Vector3Int[] CompleteCoAstar(Vector3Int a,Vector3Int b){
        string key = VectorsToKeys(a,b);
        if(!workingAStar.Contains(key)){
            workingAStar.Add(key);
            StartCoroutine(AStarPathRoutine(a,b));
        }
        if(finishedPaths.ContainsKey(key)){
            workingAStar.Remove(key);
            return finishedPaths[key];
        }
        return null;
    }
    public Vector3Int[] TryCompletedPathRetrieval(Vector3Int a,Vector3Int b){
        string key = VectorsToKeys(a,b);
        if(finishedPaths.ContainsKey(key)){
            Vector3Int[] temp = finishedPaths[key];
            finishedPaths.Remove(key);
            return temp;
        }
        return null;
    }
    //TODO: determine if coroutine is needed
    IEnumerator AStarPathRoutine(Vector3Int start, Vector3Int finish){
        
        Dictionary<Vector3Int,newStarCoord> open = new Dictionary<Vector3Int,newStarCoord>();
        List<Vector3Int> closed = new List<Vector3Int>();
        newStarCoord curr;
        if(!cellHead.hasCell(start) || !cellHead.hasCell(finish)){
            yield break;
        }
        if(finishedPaths.ContainsKey(VectorsToKeys(start,finish))){
            yield break;
            //if finished path already exists in finished paths
        }
        open.Add(start,new newStarCoord(start));
        curr = open[start];
        while(open.Count > 0){
            curr = open.OrderBy(KeyValuePair => KeyValuePair.Value.fCost).First().Value;
            if(curr.pos.Equals(finish)){
                Debug.Log("created AStar from " + start.ToString() + "to " + finish.ToString() + "using coroutine");
                Vector3Int[] finalPath = constructPath(curr);
                string finalKey = VectorsToKeys(start,finish);
                finishedPaths.Add(finalKey,finalPath);
                yield break;
            }
            open.Remove(curr.pos);
            closed.Add(curr.pos);
            
            CellBehavior[] OutNeighbors = cellHead.getCell(curr.pos).GetOutbounds();
            foreach(CellBehavior kvp in OutNeighbors){
                Vector3Int neighborPos = kvp.CellPosition;
                if(closed.Contains(neighborPos)){
                    continue;
                }
                newStarCoord neighborCoord = new newStarCoord(neighborPos);
                neighborCoord.gCost = curr.gCost + getDistance(curr.pos, neighborPos);
                neighborCoord.hCost = heuristic(neighborPos,finish);
                neighborCoord.fCost = neighborCoord.gCost + neighborCoord.hCost;
                neighborCoord.parent = curr;
                if(!open.ContainsKey(neighborPos)){
                    open.Add(neighborPos,neighborCoord);
                }
                else if(open.ContainsKey(neighborPos) && open[neighborPos].fCost > neighborCoord.fCost){
                    open[neighborPos] = neighborCoord;
                }
            }
            yield return null;
        }
        yield break;
    }
    private string VectorsToKeys(Vector3Int a,Vector3Int b){
        return "[" + a.ToString() + "," + b.ToString() + "]";
    }
    Vector3Int[] constructPath(newStarCoord end){
        List<Vector3Int> path = new List<Vector3Int>();
        newStarCoord curr = end;
        while(curr != null){
            path.Add(curr.pos);
            curr = curr.parent;
        }
        path.Reverse();
        return path.ToArray();
    }
    private float heuristic(Vector3Int a, Vector3Int b){
        //TODO work on;
        return getDistance(a,b);
    }
    private float getDistance(Vector3Int a, Vector3Int b){
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        //TODO: make manhattan
    }

}
*/