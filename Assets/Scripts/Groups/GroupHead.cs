using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GroupHead : MonoBehaviour
{
    List<Intersection> ActiveGroups = new List<Intersection>();
    [SerializeField] CellHead cellLead;
    [SerializeField] Grid grid;
    Dictionary<Vector3Int, GameObject> errorCells = new Dictionary<Vector3Int, GameObject>();
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
    public void UpdateIntersection(Cell cell)
    {
        if (ActiveGroups.Contains(cell.intersect))
        {
            return;
        }
        bool adjacentExists = false;
        Intersection lastIntersectionAddedTo = null;
        Vector3Int[] cardinals = cell.GetCardinals();
        foreach (Vector3Int cI in cardinals)
        {
            Cell card = cellLead.getCell(cI);
            if (card == null)
            {
                continue;
            }
            if (card.intersect != null && card.groupType == GroupEnum.Intersection)
            {
                var intersect = card.intersect;
                if (intersect.Equals(lastIntersectionAddedTo))
                {
                    continue;
                }
                else if (!intersect.Equals(lastIntersectionAddedTo) && lastIntersectionAddedTo != null)
                {
                    //TODO: merge intersections
                    //throw new NotImplementedException();
                    var intersect0 = lastIntersectionAddedTo;
                    var intersect1 = intersect;
                    Intersection Consumer, Consumed;
                    if (intersect0.cells.Count > intersect1.cells.Count)
                    {
                        Consumer = intersect0;
                        Consumed = intersect1;
                    }
                    else
                    {
                        Consumer = intersect1;
                        Consumed = intersect0;
                    }
                    foreach (Cell c in Consumed.cells)
                    {
                        Consumer.AddCell(c);
                        Vector3Int cellPos = c.CellPosition;
                        if (errorCells.ContainsKey(cellPos))
                        {
                            Destroy(errorCells[cellPos]);
                            errorCells.Remove(cellPos);
                            Consumer.removeError(cellPos);
                        }
                        else{
                            Consumer.removeError(cellPos);
                        }

                    }
                    var beforeRemoval = Consumed;
                    ActiveGroups.Remove(Consumed);
                    Destroy(Consumed.gameObject);
                    lastIntersectionAddedTo = Consumer;
                    if (Consumer.CellsUnfinished.Count > 0)
                    {
                        Debug.Log("Consumer Errors:");
                        foreach (Vector3Int vI in Consumer.CellsUnfinished)
                        {
                            Vector3 vec = grid.CellToWorld(vI);
                            if (!errorCells.ContainsKey(vI))
                            {
                                Debug.Log(vI);
                                var go = Instantiate(errorCellFab, vec, Quaternion.identity, transform);
                                errorCells.Add(vI, go);
                            }
                        }
                    }
                    //TODO: finish merging
                }
                else
                {
                    adjacentExists = true;
                    intersect.AddCell(cell);
                    lastIntersectionAddedTo = intersect;
                    Vector3Int cellPos = cell.CellPosition;
                    if (errorCells.ContainsKey(cellPos))
                    {
                        Destroy(errorCells[cellPos]);
                        errorCells.Remove(cellPos);
                        intersect.removeError(cellPos);
                    }
                    if (intersect.CellsUnfinished.Count > 0)
                    {
                        Debug.Log("Intrsect Errors:");
                        foreach (Vector3Int vI in intersect.CellsUnfinished)
                        {
                            Vector3 vec = grid.CellToWorld(vI);
                            if (!errorCells.ContainsKey(vI))
                            {
                                Debug.Log(vI);
                                var go = Instantiate(errorCellFab, vec, Quaternion.identity, transform);
                                errorCells.Add(vI, go);
                            }
                        }
                    }
                }

            }

        }
        if (!adjacentExists)
        {
            InstantiateIntersection(cell);
        }
    }
    public void UpdateBuilding(Cell cell)
    {

    }
    private void InstantiateIntersection(Cell cell)
    {
        var go = Instantiate(intersectionFab, cell.transform.position, Quaternion.identity, transform);
        Intersection inter = go.GetComponent<Intersection>();
        inter.IntersectionSetup(cell, grid);
        inter.specialization = GroupSpecialization.Yield;//TODO: remove debug
        ActiveGroups.Add(inter);
        Debug.Log("Setup IntersectionGroup at position: " + cell.CellPosition.ToString());
    }
}
