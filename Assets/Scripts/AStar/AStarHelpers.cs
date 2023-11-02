using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarCoord{
    public Cell cell;
    public Vector3Int pos{get{return cell.CellPosition;}}
    public Vector3 worldPos{get{return cell.transform.position;}}
    public float fCost = 0f;
    public float gCost = 0f;
    public float hCost = 0f;
    public AStarCoord parent;
    public Direction parentToThis;
    public AStarCoord(Cell cell) {
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
public class PathingCell{
    public Cell cell{get; private set;}
    public Vector3Int cPos{get{return cell.CellPosition;}}
    public Vector3 pos{get{return cell.transform.position;}}
    public Direction NextDirection{get; set;}
    public PathingCell(Cell c){
        cell = c;
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