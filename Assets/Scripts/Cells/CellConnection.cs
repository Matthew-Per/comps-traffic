using UnityEngine;

public class CellConnection{
    public Cell Cell{get; private set;}
    public readonly bool Inbound;
    public CellConnection(Cell affected, bool inbound){
        Inbound = inbound;
        Cell = affected;
    }
    //if its inbound Affected is from
    //if its outbound Affect is to
}
public enum Group{
    Road,
    Intersection,
    Building,
    NULL
}