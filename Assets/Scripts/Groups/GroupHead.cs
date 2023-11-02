using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GroupHead : MonoBehaviour
{
    Dictionary<CellBehavior,GenericNodeGroup> ActiveGroups = new Dictionary<CellBehavior, GenericNodeGroup>();
    [SerializeField] CellHead cellLead;
    [SerializeField] Grid grid;
    Dictionary<Vector3Int,GameObject> errorCells = new Dictionary<Vector3Int,GameObject>();
    [SerializeField] GameObject errorCellFab;
    [SerializeField] GameObject intersectionFab;
    [SerializeField] GameObject roadFab;
    [SerializeField] GameObject buildingFab;
    public GenericNodeGroup GetGroup(CellBehavior input){
        if(ActiveGroups.ContainsKey(input)){
            return ActiveGroups[input];
        }
        return null;
    }
    public void UpdateRoad(CellBehavior cell){
        if(ActiveGroups.ContainsKey(cell)){
            return;
        }
        foreach(CellBehavior c in cell.GetInbounds()){
            if(ActiveGroups.ContainsKey(c) && ActiveGroups[c] is RoadGroup){
                ActiveGroups[c].AddCell(cell);
                return;
            }
        }
        RoadGroup newRoad = new RoadGroup();
        newRoad.AddCell(cell);
        ActiveGroups.Add(cell,newRoad);
    }
    public void UpdateIntersection(CellBehavior cell){
        if(ActiveGroups.ContainsKey(cell)){
            return;   
        }
        bool adjacentExists = false;
        IntersectionGroup lastIntersectionAddedTo = null;
        Vector3Int[] cardinals = cell.GetCardinals();
        foreach(Vector3Int cI in cardinals){
            CellBehavior card = cellLead.getCell(cI);
            if(card == null){
                continue;
            }
            bool activeGroupsContains = ActiveGroups.ContainsKey(card);
            if(activeGroupsContains && ActiveGroups[card] is IntersectionGroup intersect){
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
                ActiveGroups.Add(cell,intersect);
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
    public void UpdateBuilding(CellBehavior cell){
        
    }
    private void InstantiateIntersection(CellBehavior cell){
        var go = Instantiate(intersectionFab,cell.transform.position,Quaternion.identity,transform);
        IntersectionGroup inter = go.GetComponent<IntersectionGroup>();
        inter.Setup(cell,grid);
        ActiveGroups.Add(cell,inter);
        Debug.Log("Setup IntersectionGroup at position: " + cell.CellPosition.ToString());
    }
}
