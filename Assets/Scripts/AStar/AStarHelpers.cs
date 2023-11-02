using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarCoord{
    public CellBehavior cell;
    public Vector3Int pos{get{return cell.CellPosition;}}
    public Vector3 worldPos{get{return cell.transform.position;}}
    public float fCost = 0f;
    public float gCost = 0f;
    public float hCost = 0f;
    public AStarCoord parent;
    public Direction parentToThis;
    public AStarCoord(CellBehavior cell) {
        this.cell = cell;
        parent = null;
    }
    public bool Equals(AStarCoord other) {
        return pos.Equals(other.pos);
    }
    public bool Equals(Vector3Int other) {
        return pos.Equals(other);
    }

}
public class IgnoreCellCoord{
    public Vector3Int pos;
    public Vector3 worldPos;
    public float fCost = 0f;
    public float gCost = 0f;
    public float hCost = 0f;
    public IgnoreCellCoord parent;
    public Direction parentToThis;
    public IgnoreCellCoord(Vector3Int pos) {
        this.pos = pos;
        parent = null;
    }
    public bool Equals(AStarCoord other) {
        return pos.Equals(other.pos);
    }
    public bool Equals(Vector3Int other) {
        return pos.Equals(other);
    }

}