using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GenericNodeGroup : MonoBehaviour
{
    public float Cost{get;protected set;}
    public List<CellBehavior> cells {get; protected set;}
    public List<CellBehavior> endCells{get; protected set;}
    public int CarLoad{get; set;}
    public abstract bool Contains(CellBehavior cell);
    public abstract void AddCell(CellBehavior cell);
    public abstract void RemoveCell(CellBehavior cell);

}
