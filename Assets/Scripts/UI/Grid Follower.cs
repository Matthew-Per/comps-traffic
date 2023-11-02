using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFollower : MonoBehaviour
{ 
    Vector3 mouse;
    float GridSize = 2.5f;
    Vector3 GridOffset = new Vector3(1.25f,.05f,1.25f);
    Vector3Int gridPos;

    [SerializeField]
    Grid grid;
    // Update is called once per frame
    void FixedUpdate()
    {
        mouse = new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.y-1);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse = new Vector3(mouse.x + GridOffset.x,0,mouse.z+GridOffset.z);
        gridPos = grid.WorldToCell(mouse);
        transform.position = grid.CellToWorld(gridPos);
    }
    public Vector3Int getGridPos(){
        return gridPos;
    }
}
