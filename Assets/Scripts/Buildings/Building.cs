using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float delay = 0f;
    public float destDelay = 3f;
    public float pathfindDelay = 60f;
    public int maxCars = 0;
    int carsOut = 0;
    int WantToReturn = 0;
    public float carSpeed = 0;
    [SerializeField] bool occupied = false;
    Cell home;
    Cell dest;
    [SerializeField] AStar pathfinder;
    PathingCell[] currentPath;
    PathingCell[] currentReturn;
    [SerializeField] GameObject carP;
    public GameObject destinationObject;
    Destination destScript;
    // Start is called before the first frame update
    void Start()
    {
        destScript = destinationObject.GetComponent<Destination>();
        StartCoroutine(SpawnCars());
        StartCoroutine(SpawnCarToHome());
        StartCoroutine(Pathfinding());
        StartCoroutine(reversePathfinding());
    }
    public void Setup(AStar a, Cell s, Cell d, GameObject destOb)
    {
        this.pathfinder = a;
        home = s;
        home.build = this;
        dest = d;
        dest.build = this;
        destinationObject = destOb;
        enabled = true;
    }

    float elapsedT = 0;
    IEnumerator SpawnCars()
    {
        while (true)
        {
            while (currentPath == null)
            {
                //handled by other function
                yield return null;
            }
            PathingCell StartCell = currentPath[0];
            if (!occupied && carsOut < maxCars)
            {
                yield return new WaitForSeconds(delay);
                GameObject c = Instantiate(carP, StartCell.pos, Quaternion.identity);
                float initRot = StartCell.NextDirection.Index * 45f;
                c.transform.eulerAngles = new Vector3(0, initRot, 0);
                CarBehavior carB = c.GetComponent<CarBehavior>();
                carB.Setup(pathfinder, this);
                carB.setPath(currentPath, carSpeed);
                carsOut++;
                occupied = true;
            }
            yield return null;
        }
    }
    IEnumerator SpawnCarToHome()
    {
        while (true)
        {
            while (currentReturn == null)
            {
                yield return null;
            }
            PathingCell StartCell = currentReturn[0];
            //TODO: destination occupying
            bool destOccupy = destScript.occupied;
            if (!destOccupy && WantToReturn > 0)
            {
                yield return new WaitForSeconds(delay);
                GameObject c = Instantiate(carP, StartCell.pos, Quaternion.identity);
                float initRot = StartCell.NextDirection.Index * 45f;
                c.transform.eulerAngles = new Vector3(0, initRot, 0);
                CarBehavior carB = c.GetComponent<CarBehavior>();
                carB.Setup(pathfinder, this);
                carB.setPath(currentReturn, carSpeed);
                WantToReturn--;
                //occupied = true;

            }
            yield return null;
        }
    }
    IEnumerator Pathfinding()
    {
        bool findingPath = true;
        while (true)
        {
            findingPath = true;
            while (findingPath)
            {
                var pathInQuestion = pathfinder.CompleteCoAstar(home.CellPosition, dest.CellPosition);
                if (pathInQuestion != null)
                {
                    if (pathInQuestion == AStar.NoPathIndicator)
                    {
                        Debug.Log("No path to dest");
                        yield return new WaitForSeconds(pathfindDelay / 10);
                    }
                    else
                    {
                        currentPath = pathInQuestion;
                        findingPath = false;
                    }
                }
                yield return null;
            }
            yield return new WaitForSeconds(pathfindDelay);
        }
    }
    IEnumerator reversePathfinding()
    {
        bool findingReturn = true;
        while (true)
        {
            findingReturn = true;
            while (findingReturn)
            {
                var pathInQuestion = pathfinder.CompleteCoAstar(dest.CellPosition, home.CellPosition);
                if (pathInQuestion != null)
                {
                    if (pathInQuestion == AStar.NoPathIndicator)
                    {
                        Debug.Log("No path to home");
                        yield return new WaitForSeconds(pathfindDelay / 10);
                    }
                    else
                    {
                        currentReturn = pathInQuestion;
                        findingReturn = false;
                    }

                }
                yield return null;
            }
            yield return new WaitForSeconds(pathfindDelay);
        }
    }
    public void CarReachedDestination(CarBehavior carInQuestion)
    {
        var destinationCell = carInQuestion.Current.cell;
        Destroy(carInQuestion.gameObject);
        if (destinationCell.Equals(home))
        {
            carsOut--;
            occupied = false;
            //TODO: figure out how to respawn cars when home
        }
        else if (destinationCell.Equals(dest))
        {
            WantToReturn++;
            destScript.occupied = false;
        }
        else
        {
            throw new System.Exception("????wtf????");
        }
    }
    void OnTriggerEnter()
    {
        occupied = true;
    }
    void OnTriggerExit()
    {
        occupied = false;
    }
}
