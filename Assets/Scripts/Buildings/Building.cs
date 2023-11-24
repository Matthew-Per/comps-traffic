using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float delay = 2f;
    public float pathfindDelay = 60f;
    public int maxCars = 0;
    int carsOut = 0;
    public float carSpeed = 0;
    bool occupied = false;
    Cell home;
    Cell dest;
    [SerializeField] AStar pathfinder;
    PathingCell[] currentPath;
    [SerializeField] GameObject carP;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCars());
        StartCoroutine(Pathfinding());
    }
    public void Setup(AStar a, Cell s, Cell d)
    {
        this.pathfinder = a;
        home = s;
        dest = d;
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
                GameObject c = Instantiate(carP, StartCell.pos, Quaternion.identity);
                float initRot = StartCell.NextDirection.Index * 45f;
                c.transform.eulerAngles = new Vector3(0, initRot, 0);
                CarBehavior carB = c.GetComponent<CarBehavior>();
                carB.Setup(pathfinder);
                carB.setPath(currentPath, carSpeed);
                carsOut++;
                occupied = true;
            }
            yield return null;
        }
    }
    IEnumerator Pathfinding(){
        bool findingPath = true;
        while(true){
            findingPath = true;
            while(findingPath){
                var pathInQuestion = pathfinder.CompleteCoAstar(home.CellPosition,dest.CellPosition);
                if(pathInQuestion != null){
                    currentPath = pathInQuestion;
                    findingPath = false;
                }
                yield return null;
            }
            yield return new WaitForSeconds(pathfindDelay);
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
