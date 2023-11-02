using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class RoadBehavior : MonoBehaviour
{
    public bool CurrentlyZFighting;
    [SerializeField] private Collider[] ZFighting;
    public LayerMask m_LayerMask;
    public Renderer thisRenderer;
    public Mesh myMesh;
    public Vector3[] verts;
    Vector3 TopLeft;
    Vector3 TopRight;
    Vector3 BottomLeft;
    Vector3 BottomRight;
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
        UpdateCollisions();
        SortVerts();
        //TODO: get most northeast northwest southeast southwest verts into method
    }

    private void SortVerts()
    {
        //ignore y, get the 4 verts that matter
        /*
        Vector3 one, two, three, four;
        one = verts[0];
        int currentOccupy = 1;
        foreach(Vector3 v in verts){
            
            if(currentOccupy == 1 && !v.Equals(one)){
                currentOccupy++;
                two = v;
            }
            elseif(currentOccupy == 2 && !v.Equals(one)&& !v.Equals(two)){
                currentOccupy++;
                two = v;
            }
        }
        */
        
        
    }

    // Update is called once per frame
    public void UpdateCollisions(){
        gameObject.layer = LayerMask.NameToLayer("ToIgnore");
        ZFighting = Physics.OverlapBox(transform.position, transform.localScale / 2, transform.rotation, m_LayerMask);
        if(ZFighting.Length > 0){
            CurrentlyZFighting = true;
        }
        else{
            CurrentlyZFighting = false;
        }
        gameObject.layer = LayerMask.NameToLayer("Road");
    } 
    public RoadBehavior[] getCollisionScripts(){
        List<RoadBehavior> objs = new List<RoadBehavior>();
        foreach(Collider c in ZFighting){
            objs.Add(c.gameObject.GetComponent<RoadBehavior>());
        }
        return objs.ToArray();
    }
    public GameObject[] getCollisionObjs(){
        List<GameObject> objs = new List<GameObject>();
        foreach(Collider c in ZFighting){
            objs.Add(c.gameObject);
        }
        return objs.ToArray();
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero,Vector3.one);
    }
}
