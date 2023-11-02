using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineToMouse : MonoBehaviour
{
    LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseCamPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
        Vector3 mouse = Camera.main.ScreenToWorldPoint(mouseCamPos);
        line.SetPosition(0, transform.position);
        line.SetPosition(1,new Vector3(mouse.x,transform.position.y,mouse.z));  
    }
}
