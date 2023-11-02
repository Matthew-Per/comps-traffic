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

public class AStar : MonoBehaviour
{
    [SerializeField]
    CellHead cellHead;

    List<string> workingAStar = new List<string>();
    Dictionary<string, Vector3Int[]> finishedPaths = new Dictionary<string, Vector3Int[]>();
    public Vector3Int[] CellPath = null;
    /*
        [SerializeField] Vector3Int DebugStart;
        [SerializeField] Vector3Int DebugEnd;
        [SerializeField] bool startA;
        async void Update(){
            if(startA){
                startA = false;
                Vector3Int[] path = await AStarPathAsync(DebugStart,DebugEnd);
                Debug.Log("end");
                for(int i = 0; i < path.Length-1; i++){
                    Vector3 start = cellHead.getCellWorld(path[i]);
                    Vector3 end = cellHead.getCellWorld(path[i+1]);
                    Debug.DrawLine(start,end);
                }
            }
        }
        public async Task<Vector3Int[]> Pathfind(Vector3Int a,Vector3Int b){
            return await AStarPathAsync(a,b);
        }
    */
    /// <summary>
    /// MUST be constantly called or on an interval for the path to be retrieved.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public Vector3Int[] CompleteCoAstar(Vector3Int a, Vector3Int b)
    {
        string key = VectorsToKeys(a, b);
        if (!workingAStar.Contains(key))
        {
            workingAStar.Add(key);
            StartCoroutine(AStarPathRoutine(a, b));
        }
        if (finishedPaths.ContainsKey(key))
        {
            workingAStar.Remove(key);
            var temp = finishedPaths[key];
            finishedPaths.Remove(key);
            return temp;
        }
        return null;
    }
    public Vector3Int[] TryCompletedPathRetrieval(Vector3Int a, Vector3Int b)
    {
        string key = VectorsToKeys(a, b);
        if (finishedPaths.ContainsKey(key))
        {
            Vector3Int[] temp = finishedPaths[key];
            finishedPaths.Remove(key);
            return temp;
        }
        return null;
    }
    IEnumerator AStarPathRoutine(Vector3Int start, Vector3Int finish)
    {
        Dictionary<Vector3Int, AStarCoord> open = new Dictionary<Vector3Int, AStarCoord>();
        List<Vector3Int> closed = new List<Vector3Int>();
        HashSet<int> visitedCars = new HashSet<int>();
        AStarCoord curr;
        if (!cellHead.hasCell(start) || !cellHead.hasCell(finish))
        {
            yield break;
        }
        if (finishedPaths.ContainsKey(VectorsToKeys(start, finish)))
        {
            yield break;
            //if finished path already exists in finished paths
        }
        var startCell = cellHead.getCell(start);
        open.Add(start, new AStarCoord(startCell));
        curr = open[start];
        while (open.Count > 0)
        {
            curr = open.OrderBy(KeyValuePair => KeyValuePair.Value.fCost).First().Value;
            if (curr.pos.Equals(finish))
            {
                Vector3Int[] finalPath = constructPath(curr);
                string finalKey = VectorsToKeys(start, finish);
                finishedPaths.Add(finalKey, finalPath);
                yield break;
            }
            open.Remove(curr.pos);
            closed.Add(curr.pos);

            KeyValuePair<Direction, CellBehavior>[] OutNeighbors = curr.cell.GetOutbounds();
            foreach (KeyValuePair<Direction, CellBehavior> kvp in OutNeighbors)
            {
                CellBehavior neighbor = kvp.Value;
                Debug.Log(neighbor);
                Debug.Log("Cell " +neighbor +" has cars of Count: " + neighbor.CarCount + "and list size of: " + neighbor.CurrentCarIds.Count);
                Vector3Int neighborPos = neighbor.CellPosition;
                Direction dir = kvp.Key;
                if (closed.Contains(neighborPos))
                {
                    continue;
                }
                AStarCoord neighborCoord = new AStarCoord(kvp.Value);
                neighborCoord.gCost = curr.gCost + traversalCost(curr.pos, neighbor, ref visitedCars);
                neighborCoord.hCost = heuristic(neighborPos, finish);
                neighborCoord.fCost = neighborCoord.gCost + neighborCoord.hCost;
                neighborCoord.parent = curr;
                neighborCoord.parentToThis = dir;
                if (!open.ContainsKey(neighborPos))
                {
                    open.Add(neighborPos, neighborCoord);
                }
                else if (open.ContainsKey(neighborPos) && open[neighborPos].fCost > neighborCoord.fCost)
                {
                    open[neighborPos] = neighborCoord;
                }
            }
            yield return null;
        }
        yield break;
    }
    public IEnumerator AStarCells(Vector3Int start, Vector3Int finish)
    {
        Dictionary<Vector3Int, IgnoreCellCoord> open = new Dictionary<Vector3Int, IgnoreCellCoord>();
        List<Vector3Int> closed = new List<Vector3Int>();
        IgnoreCellCoord curr;
        open.Add(start, new IgnoreCellCoord(start));
        curr = open[start];
        while (open.Count > 0)
        {
            curr = open.OrderBy(KeyValuePair => KeyValuePair.Value.fCost).First().Value;
            if (curr.pos.Equals(finish))
            {
                Debug.Log("created AStar from " + start.ToString() + "to " + finish.ToString() + "using coroutine");
                Vector3Int[] finalPath = cellPath(curr);
                CellPath = finalPath;
                yield break;
            }
            open.Remove(curr.pos);
            closed.Add(curr.pos);

            for (int i = 0; i < 8; i++)
            {
                Vector3Int neighborPos = curr.pos + Direction.intCast(i).Translation;
                if (closed.Contains(neighborPos))
                {
                    continue;
                }
                IgnoreCellCoord neighborCoord = new IgnoreCellCoord(neighborPos);
                neighborCoord.gCost = curr.gCost + getDistance(curr.pos, neighborPos);
                neighborCoord.hCost = cellHeuristic(neighborPos, finish);
                neighborCoord.fCost = neighborCoord.gCost + neighborCoord.hCost;
                neighborCoord.parent = curr;
                if (!open.ContainsKey(neighborPos))
                {
                    open.Add(neighborPos, neighborCoord);
                }
                else if (open.ContainsKey(neighborPos) && open[neighborPos].fCost > neighborCoord.fCost)
                {
                    open[neighborPos] = neighborCoord;
                }
            }
            yield return null;
        }
        yield break;
    }
    private string VectorsToKeys(Vector3Int a, Vector3Int b)
    {
        return "[" + a.ToString() + "," + b.ToString() + "]";
    }
    /*
        async Task<Vector3Int[]> AStarPathAsync(Vector3Int start, Vector3Int finish){
            open = new Dictionary<Vector3Int,AStarCoord>();
            closed = new List<Vector3Int>();
            AStarCoord curr;
            if(!cellHead.hasCell(start) || !cellHead.hasCell(finish)){
                return null;
            }
            open.Add(start,new AStarCoord(start));
            curr = open[start];
            while(open.Count > 0){
                curr = open.OrderBy(KeyValuePair => KeyValuePair.Value.fCost).First().Value;
                if(curr.pos.Equals(finish)){
                    Debug.Log("created AStar from " + start.ToString() + "to " + finish.ToString());
                    return constructPath(curr);
                }
                open.Remove(curr.pos);
                closed.Add(curr.pos);

                CellBehavior[] OutNeighbors = cellHead.getCell(curr.pos).GetOutbounds();
                foreach(CellBehavior kvp in OutNeighbors){
                    Vector3Int neighborPos = kvp.CellPosition;
                    if(closed.Contains(neighborPos)){
                        continue;
                    }
                    AStarCoord neighborCoord = new AStarCoord(neighborPos);
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
                await Task.Yield();
            }
            return null;
        }
        Vector3Int[] AStarPath(Vector3Int start, Vector3Int finish){
            open = new Dictionary<Vector3Int,AStarCoord>();
            closed = new List<Vector3Int>();
            AStarCoord curr;
            if(!cellHead.hasCell(start) || !cellHead.hasCell(finish)){
                return null;
            }
            open.Add(start,new AStarCoord(start));
            curr = open[start];
            while(open.Count > 0){
                curr = open.OrderBy(KeyValuePair => KeyValuePair.Value.fCost).First().Value;
                if(curr.pos.Equals(finish)){
                    Debug.Log("created AStar from " + start.ToString() + "to " + finish.ToString());
                    return constructPath(curr);
                }
                open.Remove(curr.pos);
                closed.Add(curr.pos);

                CellBehavior[] OutNeighbors = cellHead.getCell(curr.pos).GetOutbounds();
                foreach(CellBehavior kvp in OutNeighbors){
                    Vector3Int neighborPos = kvp.CellPosition;
                    if(closed.Contains(neighborPos)){
                        continue;
                    }
                    AStarCoord neighborCoord = new AStarCoord(neighborPos);
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
            }
            return null;
        }
        */
    Vector3Int[] constructPath(AStarCoord end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        AStarCoord curr = end;
        while (curr != null)
        {
            path.Add(curr.pos);
            curr = curr.parent;
        }
        path.Reverse();
        return path.ToArray();
    }
    Vector3Int[] cellPath(IgnoreCellCoord end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        IgnoreCellCoord curr = end;
        while (curr != null)
        {
            path.Add(curr.pos);
            curr = curr.parent;
        }
        path.Reverse();
        return path.ToArray();
    }
    private float heuristic(Vector3Int a, Vector3Int b)
    {
        return getDistance(a, b);
    }
    private float traversalCost(Vector3Int a, CellBehavior b, ref HashSet<int> CheckedCars)
    {
        float Cost = 0;
        if (b.group != Group.Building)
        {
            int CarCost = 0;
            int CarCostMultiplier = 3;
            foreach (int id in b.CurrentCarIds)
            {
                //Debug.Log(id);
                if (CheckedCars.Add(id))
                {
                    CarCost += CarCostMultiplier;
                    Debug.Log("worked");
                }
            }
            Cost += CarCost;
        }
        Debug.Log("traversal cost from: "+ a.ToString() + " to " + b.CellPosition.ToString() + ": " + Cost);
        return Cost;
    }
    private float cellHeuristic(Vector3Int a, Vector3Int b)
    {
        return getDistance(a, b);
    }
    private float getDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

}