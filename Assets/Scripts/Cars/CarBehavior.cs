using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Unity.VisualScripting;

public class CarBehavior : MonoBehaviour
{
    [SerializeField]
    PathingCell[] path = null;
    const float ARBITRARY_MIN_DISTANCE = 0.35f;
    const float ActualMaxSpeed = 1f;
    [SerializeField] const float DriverViewDist = 1.25f;
    float MaxSpeed = 1f;
    [SerializeField] float absoulteStop = 1f;
    [SerializeField] bool brake = false;
    [SerializeField] bool eBrake = false;
    public float RotationSpeed;
    int currentI = 0;
    float currentSpeed = 0;
    [SerializeField] private float initialAcceleration = 1f;
    [SerializeField] float speedReducer = 1;
    //public CarBehavior currentTarget;
    public CarBehavior GizmoTarget;
    BoxCollider detector;
    void Awake()
    {
        absoulteStop = Mathf.Sqrt(DriverViewDist);
    }
    public void setPath(PathingCell[] path, float maxSpeed = ActualMaxSpeed)
    {
        MaxSpeed = maxSpeed;
        this.path = path;
        StartCoroutine(Go());
        StartCoroutine(TargetSearch());
        //.25seconds is average human reaction
    }
    /*
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Hitbox"))
        {
            if (brake)
            {   
                eBrake = true;
                speedReducer = 0;
                Debug.Log(gameObject.name +" has collided");
            }

        }
        else if (collider.CompareTag("ViewDistance"))
        {
            if (collider.gameObject.layer == 3)
            {
                StartCoroutine(CrashPrevention());
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Hitbox"))
        {
            eBrake = false;
        }
        else if (collider.CompareTag("ViewDistance"))
        {
            StopCoroutine(CrashPrevention());
            speedReducer = 1;
            brake = false;
        }
    }
    */
    IEnumerator TargetSearch(){
        CarBehavior finder = null;
        //TODO:
        var cc = path[currentI].cell.CurrentCars;
        int thisIndex = cc.IndexOf(this);
        if(thisIndex > 0){
            finder = cc[thisIndex--];
            Debug.Log("Found target within cell");
        }
        else if(path[currentI + 1] != null){
            var ccN = path[currentI + 1].cell.CurrentCars;
            for(int i = ccN.Count-1; i >= 0; i--){
                if(!ccN[i].Equals(this)){
                    finder = ccN[i];
                    Debug.Log("Found target within next cell");
                    break;
                }
            }
        }
        if(finder != null && !brake){
            GizmoTarget = finder;
            StartCoroutine(CrashPrevention(finder));
            Debug.Log("Starting Braking");
        }
        else{
            yield return null;
        }
    }
    IEnumerator CrashPrevention(CarBehavior target)
    {
        brake = true;
        BoxCollider otherBoxCollider = null;
        while (true)
        {
           /*if(eBrake){
                speedReducer = 0;
                yield return null;
            }*/
            Vector3 frontCenter = Vector3.zero;
            Vector3 closestPoint = Vector3.zero;
            try{
                otherBoxCollider = target.GetComponent<BoxCollider>();
                frontCenter = transform.TransformPoint(Vector3.forward * .8f);
                closestPoint = otherBoxCollider.ClosestPoint(frontCenter);
            }
            catch(NullReferenceException e){
                speedReducer = 1;
                brake = false;
                yield break;
            }
            float distance = Vector3.Distance(frontCenter, closestPoint);
            if (distance > DriverViewDist)
            {
                speedReducer = 1;
                brake = false;
                Debug.Log("EndingBraking");
                yield break;
            }
            if (distance < absoulteStop)
            {
                speedReducer = 0;
            }
            else{
                speedReducer = 1 - (distance / DriverViewDist);
            }
            yield return null;
        }
    }
    IEnumerator Go()
    {

        //values for internal use
        Quaternion _lookRotation;
        Vector3 _direction;
        float t = 0;
        for (int i = 0; i < path.Length; i++)
        {
            currentI = i;
            while (Vector3.Distance(transform.position, path[i].pos) > ARBITRARY_MIN_DISTANCE)
            {
                currentSpeed = MaxSpeed * speedReducer;
                //find the vector pointing from our position to the target
                _direction = (path[i].pos - transform.position).normalized;

                //create the rotation we need to be in to look at the target
                _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));

                //rotate us over time according to speed until we are in the required rotation
                //thisRigidbody.rotation = Quaternion.Slerp(Quaternion.Euler(0, thisRigidbody.rotation.eulerAngles.y, 0), _lookRotation, Time.deltaTime * RotationSpeed);
                //thisRigidbody.AddForce(transform.forward*Acceleration,ForceMode.Acceleration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, t * 2);
                transform.position = Vector3.Lerp(transform.position, path[i].pos, t);


                t += currentSpeed * Time.deltaTime;
                //yield return new WaitForSeconds(.1f);
                yield return null;
            }
            //transform.position = path[i];
            t = 0;
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);

    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, GizmoTarget.transform.position);
    }
}
