using System;
using UnityEngine;

public class Direction{
    protected readonly string Name;
    public Vector3Int Translation{get; private set;}
    public readonly int Index;
    //protected static readonly Direction N,NE,E,SE,S,SW,W,NW;
    public static readonly Direction N = new Direction("N",new Vector3Int(0,0,1),0);
    public static readonly Direction NE = new Direction("NE",new Vector3Int(1,0,1),1);
    public static readonly Direction E = new Direction("E",new Vector3Int(1,0,0),2);
    public static readonly Direction SE = new Direction("SE",new Vector3Int(1,0,-1),3);
    public static readonly Direction S = new Direction("S",new Vector3Int(0,0,-1),4);
    public static readonly Direction SW = new Direction("SW",new Vector3Int(-1,0,-1),5);
    public static readonly Direction W = new Direction("W",new Vector3Int(-1,0,0),6);
    public static readonly Direction NW = new Direction("NW",new Vector3Int(-1,0,1),7);
    protected Direction(string name, Vector3Int translation, int index){
        Name = name;
        Translation = translation;
        Index = index;
    }
    public override string ToString()
    {
        return Name;
    }
    public static Direction intCast(int i){
        switch(i){
            case 0: return Direction.N;
            case 1: return Direction.NE;
            case 2: return Direction.E;
            case 3: return Direction.SE;
            case 4: return Direction.S;
            case 5: return Direction.SW;
            case 6: return Direction.W;
            case 7: return Direction.NW;
            default: throw new ArgumentOutOfRangeException(i.ToString() + " has no registered direction");
        }
    }

    public static implicit operator Index(Direction @enum)
    {
        return @enum.Index;
    }
    public Direction Opposing(){
            switch(Index){
            case 0: return Direction.S;
            case 1: return Direction.SW;
            case 2: return Direction.W;
            case 3: return Direction.NW;
            case 4: return Direction.N;
            case 5: return Direction.NE;
            case 6: return Direction.E;
            case 7: return Direction.SE;
            default: throw new ArgumentOutOfRangeException(Index.ToString() + " has no registered direction");
        }
    }
    public static implicit operator string(Direction @enum)
    {
        return @enum.Name;
    }
}