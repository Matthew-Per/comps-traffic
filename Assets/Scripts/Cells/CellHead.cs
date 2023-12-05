using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
/*
public enum Direction {
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW

}
*/
    /// <summary>
    /// Whatever <c>CellHead</c> attached to will have cells as children
    /// </summary>
public class CellHead : MonoBehaviour
{
    Dictionary<Vector3Int,Cell> ActiveCells = new Dictionary<Vector3Int, Cell>();
    [SerializeField]
    GameObject cellPrefab; 
    [SerializeField]
    Grid grid;
    Direction testing = Direction.N;
    [SerializeField] GroupHead groupLead;
    public float CellSize {get{return grid.cellSize.x;}}
    public bool updateOneWayCell(Vector3Int start,Vector3Int end, Direction dir){
        return WereJustUpdatingBothCellsAtOnce(start,dir,end);
    }
    /// <summary>
    /// Sets up cells for a building and the building itself, Requires AStar in order to setup building
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="startExit"></param>
    /// <param name="startEntrance"></param>
    /// <param name="endEntrance"></param>
    /// <param name="endExit"></param>
    /// <param name="build"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public void CreateBuiding(Vector3Int start, Vector3Int end, Direction startExit, Direction startEntrance, Direction endEntrance, Direction endExit, Building build, AStar a, GameObject destObj){
        if(hasCell(start) || hasCell(end)){
            throw new InvalidOperationException("Cannot overwrite cells with houses");
        }
        Cell home = InstantiateCell(start);
        Cell dest = InstantiateCell(end);
        Vector3Int HEXP = start + startExit.Translation;
        Vector3Int HENP = start - startEntrance.Translation;
        Cell HEXC = getCell(HEXP);
        if(HEXC == null){throw new Exception("elp home exit");}
        Cell HENC =  getCell(HENP);
        if(HENC == null){throw new Exception("elp home entrance");}
        //TODO:
        Vector3Int DEXP = end + endExit.Translation;
        Vector3Int DENP = end - endEntrance.Translation;
        Cell DEXC = getCell(DEXP);
        if(DEXC == null){throw new Exception("elp dest exit");}
        Cell DENC = getCell(DENP);
        if(DENC == null){throw new Exception("elp dest entrance");}
        Debug.Log(startExit.ToString()+","+startEntrance.ToString()+","+endExit.ToString()+","+endEntrance.ToString());
        home.addOutboundRoad(startExit, HEXC);
        HENC.addOutboundRoad(startEntrance,home);
        home.addInboundRoad(startEntrance.Opposite,HENC);
        HEXC.addInboundRoad(startExit.Opposite,home);
        dest.addOutboundRoad(endExit,DEXC);
        DENC.addOutboundRoad(endEntrance,dest);
        dest.addInboundRoad(endEntrance,DENC);
        DEXC.addInboundRoad(endExit.Opposite,dest);
        build.Setup(a,home,dest,destObj);
        if(HENC.Intersection) groupLead.UpdateIntersection(HENC);
        if(HEXC.Intersection) groupLead.UpdateIntersection(HEXC);
        if(DENC.Intersection) groupLead.UpdateIntersection(DENC);
        if(DEXC.Intersection) groupLead.UpdateIntersection(DEXC);
    }
    public void updateCells(Vector3Int start,Vector3Int end, Direction startDir,Direction endDir){
        cellUpdate(start,startDir,end);
        cellUpdate(end,endDir,start);
    }
    private bool WereJustUpdatingBothCellsAtOnce(Vector3Int start, Direction dir, Vector3Int end){
        //all my homies hate modularization
        Cell cBS;
        Cell cBE;
        if(ActiveCells.ContainsKey(start)){
            if(ActiveCells[start].GetConnections().ContainsKey(dir) && !ActiveCells[start].Intersection){ //if cell already has occupied direction
                Debug.LogError("Illegal Operation");
                return false;
            }
            cBS = ActiveCells[start];
        }
        else{
            cBS = InstantiateCell(start);
        }

        if(ActiveCells.ContainsKey(end)){
            cBE = ActiveCells[end];
        }
        else{
            cBE = InstantiateCell(end);
        }
        cBS.addOutboundRoad(dir,cBE);
        cBE.addInboundRoad(dir.Opposite,cBS); 
        if(cBS.Intersection){
            groupLead.UpdateIntersection(cBS);
        }
        if(cBE.Intersection){
            groupLead.UpdateIntersection(cBE);
        }
        return true;
    }
    private void cellUpdate(Vector3Int start, Direction dir, Vector3Int end){
    throw new NotImplementedException();
    }
    /*
    private void cellUpdate(Vector3Int start, Direction dir, Vector3Int end){
        CellBehavior cB;
        if(ActiveCells.ContainsKey(start)){
            if(ActiveCells[start].GetOutbounds().ContainsKey(dir)){ //if cell already has occupied direction
                print("Overlapping directions!");
                return;
            }
            if(ActiveCells.ContainsKey(end)&& !ActiveCells[end].Intersection &&ActiveCells[end].GetOutbounds().ContainsKey(dir)){
                print("cannot have two way roads");
                return;
            }
            cB = ActiveCells[start];
        }
        else{
            cB = InstantiateCell(start);
        }
        throw new NotImplementedException();
            cB.addOutboundRoad(dir,debugArrow,end);
            if(cB.Intersection){
                
            }
    }
    */
    private void cellUpdateEmpty(Vector3Int cell, Direction from){
        Cell cB;
        if(ActiveCells.ContainsKey(cell)){
            cB = ActiveCells[cell];
        }
        else{
            cB = InstantiateCell(cell);
        }
    }
    private Cell InstantiateCell(Vector3Int cell){
            GameObject newCell = Instantiate(cellPrefab,grid.CellToWorld(cell),Quaternion.identity,transform);
            newCell.name = cell.ToString();
            Cell cB = newCell.GetComponent<Cell>();
            cB.Setup(cell);
            ActiveCells.Add(cell,cB);
            return cB;
    }
    /*
    public ReadOnlyDictionary<Vector3Int,CellBehavior> getCells(){
        return new ReadOnlyDictionary<Vector3Int, CellBehavior>(ActiveCells);
    }
    */
    public Cell getCell(Vector3Int input){
        if(ActiveCells.ContainsKey(input)){
            return ActiveCells[input];
        }
        return null;
    }
    public Vector3 getCellWorld(Vector3Int input){
        if(ActiveCells.ContainsKey(input)){
            return grid.CellToWorld(input);
        }    
        throw new InvalidOperationException("Input Vector3 does not exist");
    }
    public bool hasCell(Vector3Int input){
        if(ActiveCells.ContainsKey(input)){
            return true;
        }
        return false;
    }
    void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.green;
            Cell[] arr = ActiveCells.Values.ToArray();
            foreach(Cell cb in arr){
                var connections = cb.GetOutbounds();
                foreach(var connection in connections){
                        Gizmos.DrawLine(cb.transform.position,connection.Value.transform.position);
                }
            }
    }
}
