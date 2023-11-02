using UnityEngine;

public class CellConnection{
    public CellBehavior Cell{get; private set;}
    public readonly bool Inbound;
    public CellConnection(CellBehavior affected, bool inbound){
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