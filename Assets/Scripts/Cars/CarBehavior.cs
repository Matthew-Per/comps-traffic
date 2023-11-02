using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class CarBehavior : MonoBehaviour
{
    [SerializeField]
    PathingCell[] path = null;
    const float ARBITRARY_MIN_DISTANCE = 0.35f;
    const float ActualMaxSpeed = 1f;
    [SerializeField] const float DriverViewDist = 4f;
    float MaxSpeed = 1f;
    [SerializeField] float absoulteStop = 1f;
    [SerializeField] bool brake = false;
    [SerializeField] bool eBrake = false;
    public float RotationSpeed;
    Rigidbody thisRigidbody;
    int currentI = 0;
    float currentSpeed = 0;
    [SerializeField] private float initialAcceleration = 1f;
    float speedReducer = 1;
    int layerMask = 1 << 3;
    public CarBehavior currentTarget;
    BoxCollider detector;
    void Awake()
    {
        absoulteStop = Mathf.Sqrt(DriverViewDist);
    }
    public void setPath(PathingCell[] path, float maxSpeed = ActualMaxSpeed)
    {
        MaxSpeed = maxSpeed;
        this.path = path;
        thisRigidbody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(Go());
        StartCoroutine(CrashPrevention());
        StartCoroutine(SpeedControl());
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
    IEnumerator CrashPrevention()
    {
        brake = true;
        BoxCollider otherBoxCollider = null;
        while (true)
        {
           /*if(eBrake){
                speedReducer = 0;
                yield return null;
            }*/
            if (currentTarget == null)
            {
                if(!FindNewTarget()){
                    speedReducer = 1;
                    brake = false;
                    yield return new WaitForSeconds(5);
                }
            }
            else{
                otherBoxCollider = currentTarget.GetComponent<BoxCollider>();
            }
            if (otherBoxCollider == null)
            {
                if(!FindNewTarget()){
                    speedReducer = 1;
                    brake = false;
                    yield return new WaitForSeconds(5);
                }
            }
            Vector3 frontCenter = transform.position + transform.forward * (detector.size.z / 2);
            Vector3 closestPoint = otherBoxCollider.ClosestPoint(frontCenter);
            float distance = Vector3.Distance(frontCenter, closestPoint);
            brake = true;
            speedReducer = 1 - (distance / DriverViewDist);
            if (distance > DriverViewDist)
            {
                speedReducer = 1;
                brake = false;
                yield return new WaitForSeconds(5);
            }
            if (distance < absoulteStop)
            {
                speedReducer = 0;
            }
            yield return null;
        }
    }
    bool FindNewTarget(){
        currentTarget = null;
        PathingCell next = null;
        if(path.Count() > currentI +1){
            next = path[currentI+1];
        } 
        if(path[currentI].cell.CurrentCars.Count > 1){
            var cc = path[currentI].cell.CurrentCars;
            int indexOfThis = cc.IndexOf(this);
            if(cc.IndexOf(this) == 0 && next != null){
                findNextTargetOnNextCell(path[currentI+1].cell.CurrentCars);
            }
            else{
                for(int i = cc.Count()-1; i >= 0; i--){
                    if(indexOfThis > i){
                        currentTarget = cc[i];
                        break;
                    }
                }
            }
        }
        else if (next != null){
            currentTarget = findNextTargetOnNextCell(path[currentI+1].cell.CurrentCars);
        }
        if(currentTarget != null){
            return true;
        }
        return false;
    }
    CarBehavior findNextTargetOnNextCell(List<CarBehavior> cc){
            for(int i = cc.Count()-1; i >= 0; i--){
                if(!cc[i].Equals(this)){
                    return cc[i];
                }
            }
            //TODO: uh oh
            return null;
    }
    IEnumerator SpeedControl()
    {
        while (true)
        {
            currentSpeed = MaxSpeed * speedReducer;
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
}
