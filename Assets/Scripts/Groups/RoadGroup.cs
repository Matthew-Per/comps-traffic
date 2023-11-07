using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGroup : GenericNodeGroup
{
    Cell RoadsEnd {get{return cells[cells.Count-1];} set{endCells[cells.Count-1]=value;}}
    Grid grid;
    void Start(){
        cells = new List<Cell>();
        endCells = new List<Cell>();
    }

    public override void AddCell(Cell cell)
    {
        cells.Add(cell);
        RoadsEnd = cell;
    }
    public override bool Contains(Cell cell)
    {
        if(cells.Contains(cell)){
            return true;
        }
        return false;
    }

    public override void RemoveCell(Cell cell)
    {
        cells.Remove(cell);
        //RoadsEnd = cells[cells.Count-1];
    }
    
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.white;
        foreach(Cell cell in cells){
            Gizmos.DrawWireCube(cell.transform.position, grid.cellSize);
        }
    }
}
