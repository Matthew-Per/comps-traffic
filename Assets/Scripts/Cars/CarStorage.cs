using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarStorage : MonoBehaviour
{
    public int Cars;
    public int CurrentCars;
    public float CarSpeed;
    bool Occupied = false;
    [SerializeField] GameObject carP;
    public PathingCell[] Path {get;private set;} 
    PathingCell StartCell;
    List<CarBehavior> CarList = new List<CarBehavior>();

    // Start is called before the first frame update
    void Start()
    {
    }
    public void Setup(int CarCount, PathingCell[] path, float carSpeed){
        enabled = true;
        Cars = CarCount;
        UpdatePath(path);
        CarSpeed = carSpeed;
    }
    public void UpdatePath(PathingCell[] newPath){
        Path = newPath;
        StartCell = newPath[0];
    }
    void Update(){
        if(!Occupied && CurrentCars < Cars){
            GameObject c = Instantiate(carP,StartCell.pos,Quaternion.identity);
            float initRot = StartCell.NextDirection.Index * 45f;
            c.transform.eulerAngles = new Vector3(0,initRot,0);
            CarBehavior carB = c.GetComponent<CarBehavior>();
            carB.setPath(Path,CarSpeed);
            CarList.Add(carB);
            CurrentCars++;
            Occupied = true;
        }
        for(int i = 0; i < CarList.Count;i++){
            if(CarList[i] == null){
                CarList.RemoveAt(i);
                CurrentCars--;
            }
        }
    }
    void OnTriggerEnter(){
        Occupied = true;
    }
    void OnTriggerExit(){
        Occupied = false;
    }
}
