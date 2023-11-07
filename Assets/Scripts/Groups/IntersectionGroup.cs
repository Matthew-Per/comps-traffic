using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntersectionGroup : GenericNodeGroup
{
    int[] edges = new int[4];
    private int north{get {return edges[0];} set{edges[0] = value;}}
    private int east{get {return edges[1];} set{edges[1] = value;}}
    private int south{get {return edges[2];} set{edges[2] = value;}}
    private int west{get {return edges[3];} set{edges[3] = value;}}
    Dictionary<Vector3Int,GameObject> cellPoses = new  Dictionary<Vector3Int,GameObject>();
    [SerializeField] private List<Vector3Int> unfinishedCells;
    public List<Vector3Int> CellsUnfinished{get {return unfinishedCells;} private set {unfinishedCells = value;}}
    bool unfinished = false;
    public Grid grid;
    Transform theI;
    [SerializeField] GameObject intersectIndicFab;

    [SerializeField] List<Cell> DEBUG_ENDCELLS = new List<Cell>();
    [SerializeField] List<Cell> DEBUG_MYCELLS = new List<Cell>();
    
    void Awake(){
        enabled = false;
        theI = transform.GetChild(0);
    }
    public void Setup(Cell first, Grid grid){
        cells = new List<Cell>();
        endCells = new List<Cell>();
        north = south = first.CellPosition.z;
        east = west = first.CellPosition.x;
        this.grid = grid;
        AddCell(first);
        StartCoroutine(CellUpdateCheck());
        enabled = true;
    }
    IEnumerator CellUpdateCheck(){
        while(true){
            foreach(Cell c in cells){
                if(c.AlertGroup){
                    FindEndCells(c);
                }
                yield return null;
            }
            yield return new WaitForSeconds(5f);
        }
    }
    void Update(){
        if(unfinishedCells.Count > 0){
            Cost = -1;
            //-1 cost means untraversable
            unfinished = true;
        }
        else if(unfinished && unfinishedCells.Count == 0){
            CalculateBaseCost();
        }
        //TODO: remove
        DEBUG_MYCELLS = cells;
        DEBUG_ENDCELLS = endCells;
    }
    private void CalculateBaseCost()
    {
        float tempCost = 0;
        tempCost += cells.Count;
        
    }
    private void CalculateLegality(Vector3Int newCell){
        if(newCell.z > north){
            north = newCell.z;
            for(int i = west; i<=east; i++){
                Vector3Int checker = new Vector3Int(i,0,north);
                if(!cellPoses.ContainsKey(checker)){
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if(newCell.z < south){
            south = newCell.z;
            for(int i = west; i<=east; i++){
                Vector3Int checker = new Vector3Int(i,0,south);
                if(!cellPoses.ContainsKey(checker)){
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if(newCell.x > east){
            east = newCell.x;
            for(int i = south; i<=north; i++){
                Vector3Int checker = new Vector3Int(east,0,i);
                if(!cellPoses.ContainsKey(checker)){
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if(newCell.x < west){
            west = newCell.x;
            for(int i = south; i<=north; i++){
                Vector3Int checker = new Vector3Int(west,0,i);
                if(!cellPoses.ContainsKey(checker)){
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }

    }
    public override bool Contains(Cell cell)
    {
        if(cells.Contains(cell)){
            return true;
        }
        return false;
    }
    public override void AddCell(Cell cell){
        if(cells.Contains(cell)){
            return;
        }
        cells.Add(cell);
        var go = Instantiate(intersectIndicFab,cell.transform.position,Quaternion.identity,transform);
        cellPoses.Add(cell.CellPosition,go);
        FindEndCells(cell);
        CalculateBaseCost();
        CalculateLegality(cell.CellPosition);
        MoveCenter();
    }
    private void FindEndCells(Cell newCell){
        if(endCells.Contains(newCell)){
            endCells.Remove(newCell);
        }
        foreach(var kvp in newCell.GetOutbounds()){
            var cell = kvp.Value;
            if(!endCells.Contains(cell) && cell.group != Group.Intersection){
                endCells.Add(cell);
            }
        }
    }
    private void MoveCenter(){
        Vector3 northVec = grid.CellToWorld(new Vector3Int(0,0,north));
        Vector3 southVec = grid.CellToWorld(new Vector3Int(0,0,south));
        Vector3 westVec = grid.CellToWorld(new Vector3Int(west,0,0));
        Vector3 eastVec = grid.CellToWorld(new Vector3Int(east,0,0));
        theI.position = new Vector3((westVec.x + eastVec.x)/2,0,(northVec.z + southVec.z)/2);

    }
    public override void RemoveCell(Cell cell)
    {
        cells.Remove(cell);
        cellPoses.Remove(cell.CellPosition);
        CalculateBaseCost();
        //TODO: Legality check for deletion
        //CalculateLegality(cell.CellPosition);
        MoveCenter();
    }

    public void removeError(Vector3Int cellPos)
    {
        if(CellsUnfinished.Contains(cellPos)){
            CellsUnfinished.Remove(cellPos);
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.white;
        foreach(Cell cell in cells){
            Gizmos.DrawWireCube(cell.transform.position, grid.cellSize);
        }
    }
}
