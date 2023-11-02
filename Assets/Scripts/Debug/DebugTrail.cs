using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTrail : MonoBehaviour
{
    // Start is called before the first frame update
    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Transform lastChild = null;
        foreach(Transform child in transform){
            if(lastChild != null){
                Gizmos.DrawLine(lastChild.position, child.position);
            }
            lastChild = child;
        }
    }
}
