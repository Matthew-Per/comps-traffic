using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMarker : MonoBehaviour
{
    public GameObject marker;
    bool onePerClick;
    // Start is called before the first frame update
    void Start()
    {
        onePerClick =true;
    }

    // Update is called once per frame
    void Update()
    {
     if(Input.GetMouseButtonDown(0) && onePerClick){
        Instantiate(marker,transform.position,Quaternion.identity);
        onePerClick = false;
     }   
     if(Input.GetMouseButtonUp(0)){
        onePerClick = true;
     }
    }
}
