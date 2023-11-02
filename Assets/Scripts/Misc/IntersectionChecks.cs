using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
[RequireComponent(typeof(RoadBuilder))]
public class IntersectionChecks : MonoBehaviour
{
    // Start is called before the first frame update
    RoadBuilder rB;
    GameObject updatedRoad;
    BoxCollider boxCollider;
    List<RoadBehavior> roads = new List<RoadBehavior>();

    public GameObject debugMarker;
    
    void Start()
    {
        rB = gameObject.GetComponent<RoadBuilder>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rB.RoadsHaveUpdated){
            RoadBehavior newRoad = rB.updatedRoad.GetComponent<RoadBehavior>();
            RoadBehavior[] CollScripts = newRoad.getCollisionScripts();
            roads.AddRange(CollScripts);
            foreach(RoadBehavior coll in CollScripts){
                Instantiate(debugMarker,coll.verts[0],Quaternion.identity);
            }
            //Vector3 wack = new Vector3(newRoad.thisRenderer.bounds.max.x,newRoad.transform.position.y,newRoad.thisRenderer.bounds.max.z);
            


            newRoad.UpdateCollisions();
            rB.UpdatedRoadOff();
        }
    }
    void collectRoadScripts(){
        List<GameObject> objs = GameObject.FindGameObjectsWithTag("Road").ToList<GameObject>();
        foreach (GameObject ob in objs){
            roads.Add(ob.GetComponent<RoadBehavior>());
        }
    }
}
