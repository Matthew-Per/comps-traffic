using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGroup : GenericNodeGroup
{
    CellBehavior RoadsEnd {get{return cells[cells.Count-1];} set{endCells[cells.Count-1]=value;}}
    void Start(){
        cells = new List<CellBehavior>();
        endCells = new List<CellBehavior>();
    }

    public override void AddCell(CellBehavior cell)
    {
        cells.Add(cell);
        RoadsEnd = cell;
    }
    public override bool Contains(CellBehavior cell)
    {
        if(cells.Contains(cell)){
            return true;
        }
        return false;
    }

    public override void RemoveCell(CellBehavior cell)
    {
        cells.Remove(cell);
        //RoadsEnd = cells[cells.Count-1];
    }
}
