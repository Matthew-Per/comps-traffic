using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugOperator : MonoBehaviour
{
    [SerializeField] CarBehavior car; 
    // Start is called before the first frame update
    void Start()
    {
        Vector3[] path = new Vector3[9]{new Vector3(-11.25f,0f,1.25f),new Vector3(-8.75f,0f,1.25f),new Vector3(-6.25f,0f,1.25f),new Vector3(-3.75f,0f,1.25f)
        ,new Vector3(-1.25f,0f,1.25f),new Vector3(1.25f,0f,1.25f),new Vector3(3.75f,0f,1.25f),new Vector3(6.25f,0f,1.25f),new Vector3(8.75f,0f,1.25f)};
        //car.setPath(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
