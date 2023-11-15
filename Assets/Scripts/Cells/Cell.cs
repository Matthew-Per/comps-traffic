using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
public class Cell : MonoBehaviour
{
    //[SerializeField] GameObject debugIntersectionTell;
    public bool Intersection { get; private set; }
    //roads
    //RoadType[] roads = new RoadType[8]{RoadType.NULL,RoadType.NULL,RoadType.NULL,RoadType.NULL,RoadType.NULL,RoadType.NULL,RoadType.NULL,RoadType.NULL};
    //objs
    public Vector3Int CellPosition { get; private set; }
    Dictionary<Direction, GameObject> roadObjects = new Dictionary<Direction, GameObject>();
    //neighbors
    Dictionary<Direction, CellConnection> AllConnections = new Dictionary<Direction, CellConnection>();
    const float clockwiseRotation = 45f;
    const float indicOffset = 1.25f;
    public Group group;
    public GroupEnum groupType{get{if(group == null){return GroupEnum.Road;}else{return group.type;}}}
    public bool AlertGroup = false;

    [field: SerializeField] public List<CarBehavior> CurrentCars { get; private set; }

    [SerializeField] public int CarCount;
    const int CarLayer = 3;
    //if directions >2 become intersection
    //int DivergingRoads = 0;
    void Start()
    {
        CurrentCars = new List<CarBehavior>();
        //enabled = false;
    }
    private void InitiateIntersection()
    {
        Intersection = true;
        //Group gets assigned by GroupHead
         //Instantiate(debugIntersectionTell,new Vector3(transform.position.x,transform.position.y + indicOffset,transform.position.z),debugIntersectionTell.transform.rotation,transform);
    }
    public void Setup(Vector3Int position)
    {
        this.CellPosition = position;
        enabled = true;
    }
    //Outbound should handle the gameobject
    public void addOutboundRoad(Direction d, GameObject indicator, Cell newNeighbor)
    {
        Vector3 offset = offsetFinder(d);
        roadObjects[d] = Instantiate(indicator, transform.position + offset, indicator.transform.rotation, transform);
        roadObjects[d].name = d.ToString();
        roadObjects[d].transform.Rotate(new Vector3(0, d.Index * clockwiseRotation, 0));
        ChangeOutbound(d, newNeighbor);
        /*
        if(!AllConnections.ContainsKey(d.Opposing())){
            DivergingRoads++;
        }
        else if(AllConnections.ContainsKey(d.Opposing()) && !AllConnections[d.Opposing()].Inbound){
            DivergingRoads++;
        }
        if(DivergingRoads > 1){
            InitiateIntersection();
        }*/
        if (checkIfIntersection())
        {
            InitiateIntersection();
        }
    }
    public void addInboundRoad(Direction d, Cell newNeighbor)
    {
        ChangeInbound(d, newNeighbor);
        if (checkIfIntersection())
        {
            InitiateIntersection();
        }
    }
    private bool checkIfIntersection()
    {
        if (AllConnections.Count >= 2 && !Intersection)
        {
            if (AllConnections.Count > 2)
            {
                return true;
            }
            //should only have two elements if it reached this point
            var arrC = AllConnections.ToArray<KeyValuePair<Direction, CellConnection>>();
            if (arrC[0].Value.Inbound == !arrC[1].Value.Inbound)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
    public void removeRoad(Direction d, GameObject indicator)
    {
        throw new NotImplementedException();
    }
    private void ChangeOutbound(Direction d, Cell newConnection)
    {
        CellConnection newOut = new CellConnection(newConnection, false);
        AllConnections[d] = newOut;
        AlertGroup = true;
    }
    private void ChangeInbound(Direction d, Cell newConnection)
    {
        CellConnection newIn = new CellConnection(newConnection, true);
        AllConnections[d] = newIn;
        AlertGroup = true;
    }
    public ReadOnlyDictionary<Direction, CellConnection> GetConnections()
    {
        ReadOnlyDictionary<Direction, CellConnection> readOnlyDictionary = new ReadOnlyDictionary<Direction, CellConnection>(AllConnections);
        return readOnlyDictionary;
    }
    /// <summary>
    /// Returns all outbound connections scripts, used specificlly for A Star, does not give direction
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<Direction, Cell>[] GetOutbounds()
    {
        List<KeyValuePair<Direction, Cell>> returnee = new List<KeyValuePair<Direction, Cell>>();
        foreach (KeyValuePair<Direction, CellConnection> kvp in AllConnections)
        {
            if (!kvp.Value.Inbound)
            {
                returnee.Add(new KeyValuePair<Direction, Cell>(kvp.Key, kvp.Value.Cell));
            }
        }
        return returnee.ToArray();
    }
    public KeyValuePair<Direction, Cell>[] GetInbounds()
    {
        List<KeyValuePair<Direction, Cell>> returnee = new List<KeyValuePair<Direction, Cell>>();
        foreach (KeyValuePair<Direction, CellConnection> kvp in AllConnections)
        {
            if (kvp.Value.Inbound)
            {
                returnee.Add(new KeyValuePair<Direction, Cell>(kvp.Key, kvp.Value.Cell));
            }
        }
        return returnee.ToArray();
    }
    public Vector3Int[] GetCardinals()
    {
        Vector3Int[] returnee = new Vector3Int[4];
        returnee[0] = CellPosition + Direction.N.Translation;
        returnee[1] = CellPosition + Direction.E.Translation;
        returnee[2] = CellPosition + Direction.S.Translation;
        returnee[3] = CellPosition + Direction.W.Translation;
        return returnee;
    }

    /*public Dictionary<Direction, Vector3Int> getConnections(){
        Dictionary<Direction, Vector3Int> validConnections = new Dictionary<Direction, Vector3Int>();
        int i = 0;
        foreach(Vector3Int? connector in connections){
            if(connector.HasValue){
                validConnections.Add(Direction.intToDirection(i),connector.Value);
            }
            i++;
        }
        return validConnections;
    }
    */
    private Vector3 offsetFinder(Direction d)
    {
        Vector3 vector3 = d.Translation;
        return vector3 * indicOffset;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == CarLayer)
        {
            CarCount++;
            CurrentCars.Add(other.GetComponent<CarBehavior>());
            //Debug.Log("NowContains car: " + other.gameObject.GetInstanceID());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == CarLayer)
        {
            CarCount--;
            CurrentCars.Remove(other.GetComponent<CarBehavior>());
        }
    }

}
