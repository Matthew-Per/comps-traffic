using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupEnum
{
    Road,
    Intersection,
    Building
}
public enum GroupSpecialization
{
    Yield,
    Stop,
    Light,
    Continue
}
public class Intersection : MonoBehaviour
{
    int[] edges = new int[4];
    private int north { get { return edges[0]; } set { edges[0] = value; } }
    private int east { get { return edges[1]; } set { edges[1] = value; } }
    private int south { get { return edges[2]; } set { edges[2] = value; } }
    private int west { get { return edges[3]; } set { edges[3] = value; } }
    HashSet<Vector3Int> cellPoses = new HashSet<Vector3Int>();
    public float Cost { get; protected set; }
    [field: SerializeField] public List<Cell> cells { get; protected set; } = new List<Cell>();
    public List<Cell> endCells { get; protected set; } = new List<Cell>();
    [SerializeField] private List<Vector3Int> unfinishedCells;
    public List<Vector3Int> CellsUnfinished { get { return unfinishedCells; } private set { unfinishedCells = value; } }
    bool unfinished = false;
    public Grid grid;
    Transform theI;
    [SerializeField] GameObject intersectIndicFab;
    public GroupEnum type { get; private set; }
    [field:SerializeField] public GroupSpecialization specialization { get; set; }
    Vector3 Center;
    BoxCollider Trigger;
    [SerializeField] List<CarBehavior> AwaitingCars = new List<CarBehavior>();
    [SerializeField] List<CarBehavior> currentCars = new List<CarBehavior>();
    Dictionary<Cell, Direction> Inbounds = new Dictionary<Cell, Direction>();
    public float lightTiming = 5f;
    bool Jammed = true;
    void Awake()
    {
        enabled = false;
        theI = transform.GetChild(0);
        Trigger = GetComponent<BoxCollider>();
        StartCoroutine(JamChecker());
    }
    public void IntersectionSetup(Cell first, Grid grid)
    {
        type = GroupEnum.Intersection;
        cells = new List<Cell>();
        endCells = new List<Cell>();
        north = south = first.CellPosition.z;
        east = west = first.CellPosition.x;
        this.grid = grid;
        this.type = type;
        AddCell(first);
        enabled = true;
    }
    void Update()
    {
        if (unfinishedCells.Count > 0)
        {
            Cost = -1;
            //-1 cost means untraversable
            unfinished = true;
        }
        else if (unfinished && unfinishedCells.Count == 0)
        {
            CalculateBaseCost();
        }
        if (AwaitingCars.Count > 0 || currentCars.Count > 0)
        {
            CarLogic();
        }
        //TODO: remove
    }
    IEnumerator JamChecker()
    {
        while (true)
        {
                for(int i = 0; i < cells.Count; i++)
                {
                    var c = cells[i];
                    if (c.CurrentCars.Count > 1)
                    {
                        bool Jammed = true;
                        foreach (CarBehavior car in c.CurrentCars)
                        {
                            if (!car.stopped)
                            {
                                Jammed = false;
                                break;
                            }
                        }
                        if (Jammed)
                        {
                            CarBehavior firstCar = c.CurrentCars[0];
                            firstCar.AbsoluteRightOfWay();
                            Debug.Log("Absolute used");
                        }
                    }
                    yield return null;
                }
            yield return null;
        }
    }
    private void CalculateBaseCost()
    {
        float tempCost = 0;
        tempCost += cells.Count;

    }
    private void CalculateLegality(Vector3Int newCell)
    {
        if (newCell.z > north)
        {
            north = newCell.z;
            for (int i = west; i <= east; i++)
            {
                Vector3Int checker = new Vector3Int(i, 0, north);
                if (!cellPoses.Contains(checker))
                {
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if (newCell.z < south)
        {
            south = newCell.z;
            for (int i = west; i <= east; i++)
            {
                Vector3Int checker = new Vector3Int(i, 0, south);
                if (!cellPoses.Contains(checker))
                {
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if (newCell.x > east)
        {
            east = newCell.x;
            for (int i = south; i <= north; i++)
            {
                Vector3Int checker = new Vector3Int(east, 0, i);
                if (!cellPoses.Contains(checker))
                {
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }
        if (newCell.x < west)
        {
            west = newCell.x;
            for (int i = south; i <= north; i++)
            {
                Vector3Int checker = new Vector3Int(west, 0, i);
                if (!cellPoses.Contains(checker))
                {
                    unfinished = true;
                    unfinishedCells.Add(checker);
                }
            }
        }

    }
    public bool Contains(Cell cell)
    {
        if (cells.Contains(cell))
        {
            return true;
        }
        return false;
    }
    public void AddCell(Cell cell)
    {
        if (cells.Contains(cell))
        {
            return;
        }
        cells.Add(cell);
        var go = Instantiate(intersectIndicFab, cell.transform.position, Quaternion.identity, transform);
        cellPoses.Add(cell.CellPosition);
        cell.intersect = this;
        CorrectInbounds(cell);
        FindEndCells(cell);
        CalculateBaseCost();
        CalculateLegality(cell.CellPosition);
        MoveCenter();
        ResizeTrigger();
    }
    private void CorrectInbounds(Cell newCell)
    {
        if (Inbounds.ContainsKey(newCell))
        {
            Inbounds.Remove(newCell);
        }
        var cellInbounds = newCell.GetInbounds();
        foreach (var kvp in cellInbounds)
        {
            if (kvp.Value.groupType != GroupEnum.Intersection)
            {
                Inbounds.Add(kvp.Value, kvp.Key);
            }
        }
    }
    private void FindEndCells(Cell newCell)
    {
        if (endCells.Contains(newCell))
        {
            endCells.Remove(newCell);
        }
        foreach (var kvp in newCell.GetOutbounds())
        {
            var cell = kvp.Value;
            if (!endCells.Contains(cell) && cell.groupType != GroupEnum.Intersection)
            {
                endCells.Add(cell);
            }
        }
    }
    private void MoveCenter()
    {
        Vector3 northVec = grid.CellToWorld(new Vector3Int(0, 0, north));
        Vector3 southVec = grid.CellToWorld(new Vector3Int(0, 0, south));
        Vector3 westVec = grid.CellToWorld(new Vector3Int(west, 0, 0));
        Vector3 eastVec = grid.CellToWorld(new Vector3Int(east, 0, 0));
        Center = new Vector3((westVec.x + eastVec.x) / 2, 0, (northVec.z + southVec.z) / 2);
        theI.position = Center;

    }
    public void RemoveCell(Cell cell)
    {
        cells.Remove(cell);
        cellPoses.Remove(cell.CellPosition);
        CalculateBaseCost();
        //TODO: Legality check for deletion
        //CalculateLegality(cell.CellPosition);
        MoveCenter();
    }

    public void removeError(Vector3Int cellPos)
    {
        if (CellsUnfinished.Contains(cellPos))
        {
            CellsUnfinished.Remove(cellPos);
        }
    }
    public void UpdateOuts(Cell c)
    {
        //endCells = new List<Cell>();
        var GetReal = c.GetOutbounds();
        endCells.RemoveAll(cell => cell is null);
        foreach (var i in GetReal)
        {
            if (!endCells.Contains(i.Value))
            {
                var iCell = i.Value;
                if (iCell.intersect == null || !iCell.intersect.Equals(this))
                {
                    endCells.Add(iCell);
                }
            }
        }
    }
    void ResizeTrigger()
    {
        Trigger.center = transform.InverseTransformPoint(Center);
        Vector3 cSize = Trigger.size;
        cSize.x = Math.Abs(east + 1 - west) * grid.cellSize.x;
        cSize.z = Math.Abs(north + 1 - south) * grid.cellSize.z;
        if (cSize.x == 0) cSize.x = 2.5f;
        if (cSize.z == 0) cSize.z = 2.5f;
        Trigger.size = cSize;
    }
    void OnTriggerEnter(Collider other)
    {

    }
    void OnTriggerExit(Collider other)
    {
        var car = other.GetComponent<CarBehavior>();
        if (car != null)
        {
            currentCars.Remove(car);
        }
    }
    public void AddCarToQueue(CarBehavior car)
    {
        if (car != null)
        {
            AwaitingCars.Add(car);
        }
    }
    void CarLogic()
    {
        currentCars.RemoveAll(item => item == null);
        switch (specialization)
        {
            case GroupSpecialization.Stop:
                StopLogic();
                break;
            case GroupSpecialization.Yield:
                YieldLogic();
                break;
            case GroupSpecialization.Light:
                LightLogic();
                break;

        }
    }
    void StopLogic()
    {
        if (AwaitingCars.Count > 0)
        {
            if (currentCars.Count == 0)
            {
                var nextCar = AwaitingCars[0];
                currentCars.Add(nextCar);
                nextCar.RightOfWay = true;
                AwaitingCars.Remove(nextCar);
            }
        }
    }
    void YieldLogic()
    {
        if (AwaitingCars.Count > 0)
        {
            if (currentCars.Count == 0)
            {
                var nextCar = AwaitingCars[0];
                currentCars.Add(nextCar);
                nextCar.RightOfWay = true;
                AwaitingCars.Remove(nextCar);
            }
            else
            {
                var AwaitingCarsSnapshot = AwaitingCars.ToArray();
                var CurrentCarsSnapshot = currentCars.ToArray();
                foreach (var potential in AwaitingCarsSnapshot)
                {
                    if (CheckCarsRightOfWay(potential, CurrentCarsSnapshot))
                    {
                        currentCars.Add(potential);
                        potential.RightOfWay = true;
                        AwaitingCars.Remove(potential);
                    }
                }
            }
        }
    }
    float elapsedTime = 0;
    int lightCycle = 0;
    [SerializeField] Direction currentLight = null;
    List<Direction> lights = new List<Direction>();
    bool changeLights = true;
    void LightLogic()
    {
        if (changeLights)
        {
            foreach (var kvp in Inbounds)
            {
                if (!lights.Contains(kvp.Value))
                    lights.Add(kvp.Value);
            }
            currentLight = lights[lightCycle];
            lightCycle++;
            if (lightCycle >= lights.Count)
            {
                lightCycle = 0;
            }
            changeLights = false;
        }
        if (AwaitingCars.Count > 0)
        {
            for (int i = 0; i < AwaitingCars.Count;)
            {
                var possible = AwaitingCars[i];
                if (possible.IntersectionEntranceCell.NextDirection.Equals(currentLight) ||
                possible.IntersectionEntranceCell.NextDirection.Equals(currentLight.Opposite))
                {
                    currentCars.Add(possible);
                    possible.RightOfWay = true;
                    AwaitingCars.Remove(possible);
                }
                else
                {
                    i++;
                }
            }
        }
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lightTiming)
        {
            elapsedTime = 0;
            changeLights = true;
        }

    }
    bool CheckCarsRightOfWay(CarBehavior behind, CarBehavior[] forwardCars)
    {
        bool noOpposers = true;
        foreach (var forward in forwardCars)
        {
            //if a car ahead entered the intersection through a direction differing from the current car
            if (forward.IntersectionEntranceCell != null &&
                !behind.IntersectionEntranceCell.NextDirection.Equals(forward.IntersectionEntranceCell.NextDirection))
            {
                noOpposers = false;
            }
        }
        return noOpposers;
    }
    void AddCar(CarBehavior car)
    {
        currentCars.Add(car);
        car.RightOfWay = true;
        AwaitingCars.Remove(car);

    }
    void OnDrawGizmosSelected()
    {
        /*Gizmos.color = Color.white;
        foreach(Cell cell in cells){
            Gizmos.DrawWireCube(cell.transform.position, grid.cellSize);
        }*/
        Gizmos.color = Color.yellow;
        foreach (Cell cell in endCells)
        {
            Gizmos.DrawWireCube(cell.transform.position, grid.cellSize);
        }
        Gizmos.color = Color.gray;
        foreach (var kvp in Inbounds)
        {
            Gizmos.DrawWireCube(kvp.Key.transform.position, grid.cellSize);
        }
    }
}
