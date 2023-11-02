using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MouseFollowerBehavior : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    public GameObject Splinefab;
    public GameObject point;
    GameObject cSplinefab;
    SplineContainer cSplineCon;
    Spline cSpline;
    bool FirstPlaced = false;
    bool CantHoldM1 = false;
    public Vector3 point1;
    public Vector3 point2;
    bool BuildRoad = false;

    public bool BuildARoad { get => BuildRoad; set => BuildRoad = value; }

    void Start()
    {
    }
    void FixedUpdate(){
        cSpline.Knots.ToArray();
    }
    void Update()
    {
        cSpline.Knots.ToArray();
        if(Input.GetMouseButtonDown(0) && !FirstPlaced && !CantHoldM1){
            cSplinefab = Instantiate(Splinefab,transform.position,quaternion.identity);
            cSplineCon = cSplinefab.AddComponent<SplineContainer>();
            cSpline = cSplineCon.AddSpline();
            float3 f = new float3 (transform.position.x, transform.position.y,transform.position.z);
            cSpline.Add(new BezierKnot(f),TangentMode.AutoSmooth);
            FirstPlaced = true;
            CantHoldM1 = true;
        }
        if(Input.GetMouseButtonUp(0)){
            CantHoldM1 = false;
        }
        if(Input.GetMouseButtonDown(1) && FirstPlaced == true){
            //Destroy(cSplinefab);
            FirstPlaced = false;
        }
        if(Input.GetMouseButtonDown(0) && !CantHoldM1 && FirstPlaced){
            float3 f = new float3 (transform.position.x, transform.position.y,transform.position.z);
            cSpline.Add(new BezierKnot(f),TangentMode.AutoSmooth);
            
            Instantiate(Splinefab,transform.position,quaternion.identity,cSplinefab.transform);
            CantHoldM1 = true;
        }
        if(Input.GetMouseButtonDown(0) && !MarkerPlaced && !CantHoldM1){
            MarkerPlaced = true;
            cMarker = Instantiate(Marker,transform.position, Quaternion.identity);
            cMarker.transform.Rotate(90,0,0);
            point1 = cMarker.transform.position;
            CantHoldM1 = true;
        }
        if(Input.GetMouseButtonUp(0)){
            CantHoldM1 = false;
        }
        if(Input.GetMouseButtonDown(1) && MarkerPlaced == true){
            point1 = Vector3.zero;
            MarkerPlaced = false;
            Destroy(cMarker);
            cMarker = null;
        }
        if(Input.GetMouseButtonDown(0) && !CantHoldM1 && MarkerPlaced){
            Vector3 MousePosInScreen = new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y);
            point2 = Camera.main.ScreenToWorldPoint(MousePosInScreen);
            MarkerPlaced = false;
            Destroy(cMarker);
            cMarker = null;
            print(point1);
            print(point2);
            BuildRoad = true;
        }
        
    }
    */
}
