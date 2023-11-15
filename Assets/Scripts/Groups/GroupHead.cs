using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GroupHead : MonoBehaviour
{
    List<Group> ActiveGroups = new List<Group>();
    [SerializeField] CellHead cellLead;
    [SerializeField] Grid grid;
    Dictionary<Vector3Int,GameObject> errorCells = new Dictionary<Vector3Int,GameObject>();
    [SerializeField] GameObject errorCellFab;
    [SerializeField] GameObject intersectionFab;
    [SerializeField] GameObject roadFab;
    [SerializeField] GameObject buildingFab;
    /*
    public void UpdateRoad(Cell cell){
        if(ActiveGroups.ContainsKey(cell)){
            return;
        }
        foreach(Cell c in cell.GetInbounds()){
            if(ActiveGroups.ContainsKey(c) && ActiveGroups[c] is RoadGroup){
                ActiveGroups[c].AddCell(cell);
                return;
            }
        }
        RoadGroup newRoad = new RoadGroup();
        newRoad.AddCell(cell);
        ActiveGroups.Add(cell,newRoad);
    }
    */
    public void UpdateIntersection(Cell cell){
        if(ActiveGroups.Contains(cell.group)){
            return;   
        }
        bool adjacentExists = false;
        Group lastIntersectionAddedTo = null;
        Vector3Int[] cardinals = cell.GetCardinals();
        foreach(Vector3Int cI in cardinals){
            Cell card = cellLead.getCell(cI);
            if(card == null){
                continue;
            }
            if(card.group != null && card.groupType == GroupEnum.Intersection){
                var intersect = card.group;
                if(intersect.Equals(lastIntersectionAddedTo)){
                    continue;
                }
                else if(!intersect.Equals(lastIntersectionAddedTo) && lastIntersectionAddedTo != null){
                    //TODO: merge intersections
                    throw new NotImplementedException();
                }
                adjacentExists = true;
                intersect.AddCell(cell);
                lastIntersectionAddedTo = intersect;
                Vector3Int cellPos = cell.CellPosition;
                if(errorCells.ContainsKey(cellPos)){
                    Destroy(errorCells[cellPos]);
                    errorCells.Remove(cellPos);
                    intersect.removeError(cellPos);
                }
                if(intersect.CellsUnfinished.Count > 0){
                    foreach(Vector3Int vI in intersect.CellsUnfinished){
                        Vector3 vec = grid.CellToWorld(vI);
                        if(!errorCells.ContainsKey(vI)){
                            var go = Instantiate(errorCellFab,vec,Quaternion.identity,transform);
                            errorCells.Add(vI,go);
                        }
                    }
                }
            }
            
        }
        if(!adjacentExists){
            InstantiateIntersection(cell);
        }
    }
    public void UpdateBuilding(Cell cell){
        
    }
    private void InstantiateIntersection(Cell cell){
        var go = Instantiate(intersectionFab,cell.transform.position,Quaternion.identity,transform);
        Group inter = go.GetComponent<Group>();
        inter.IntersectionSetup(cell,grid);
        ActiveGroups.Add(inter);
        Debug.Log("Setup IntersectionGroup at position: " + cell.CellPosition.ToString());
    }
}
