using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    public GameObject MouseFollower;
    public GameObject RoadPrefab;
    GameObject cRoad;
    MouseFollowerBehavior mfB;
    Vector3 point1;
    Vector3 point2;
    bool updatedRoads = false;
    public bool RoadsHaveUpdated { get => updatedRoads;}

    public GameObject updatedRoad { get => cRoad;}

    // Start is called before the first frame update
    void Start()
    {
       mfB = MouseFollower.GetComponent<MouseFollowerBehavior>(); 

    }

    // Update is called once per frame
    void Update()
    {
      /*
      if(mfB.BuildARoad){
        point1 = mfB.point1;
        point2 = mfB.point2;
        //get center point between two lines
        Vector3 center = new Vector3(point1.x + point2.x, 0 ,point1.z + point2.z) /2f;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, point2 - point1);
        float scaleZ = Vector3.Distance(point1,point2);
        Vector3 scale = new Vector3(RoadPrefab.transform.localScale.x, RoadPrefab.transform.localScale.y,scaleZ);
        cRoad = Instantiate(RoadPrefab,center,rot);
        cRoad.transform.localScale = scale;
        mfB.BuildARoad = false;
        updatedRoads = true;
      }  
      */
    }
    public void UpdatedRoadOff(){
      updatedRoads = false;
    }
}
