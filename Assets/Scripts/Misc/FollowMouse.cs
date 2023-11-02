using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y);
        transform.position = Camera.main.ScreenToWorldPoint(Position);
    }
}
