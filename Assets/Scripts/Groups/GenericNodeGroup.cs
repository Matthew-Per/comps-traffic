using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GenericNodeGroup : MonoBehaviour
{
    public float Cost{get;protected set;}
    public List<Cell> cells {get; protected set;}
    public List<Cell> endCells{get; protected set;}
    public int CarLoad{get; set;}
    public abstract bool Contains(Cell cell);
    public abstract void AddCell(Cell cell);
    public abstract void RemoveCell(Cell cell);

}
