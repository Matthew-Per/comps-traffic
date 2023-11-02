using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class CarBehavior : MonoBehaviour
{
    [SerializeField]
    Vector3[] path = null;
    const float ARBITRARY_MIN_DISTANCE = 0.35f;
    const float ActualMaxSpeed = 1f;
    [SerializeField] float DriverViewDist = 0;
    float MaxSpeed = 1f;
    [SerializeField] float absoulteStop = 1f;
    [SerializeField] bool brake = false;
    [SerializeField] bool eBrake = false;
    public float RotationSpeed;
    Rigidbody thisRigidbody;
    int debugI = 0;
    float currentSpeed = 0;
    [SerializeField] private float initialAcceleration = 1f;
    float speedReducer = 1;
    int layerMask = 1 << 3;
    BoxCollider detector;
    void Awake()
    {
        detector = transform.GetChild(0).GetComponent<BoxCollider>();
        DriverViewDist = detector.size.z;
        absoulteStop = Mathf.Sqrt(DriverViewDist);
    }
    public void setPath(Vector3[] path, float maxSpeed = ActualMaxSpeed)
    {
        MaxSpeed = maxSpeed;
        this.path = path;
        thisRigidbody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(Go());
        //StartCoroutine(CrashPrevention());
        StartCoroutine(SpeedControl());
        //.25seconds is average human reaction
    }
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
                StartCoroutine(CrashPrevention(collider));
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
            StopCoroutine(CrashPrevention(collider));
            speedReducer = 1;
            brake = false;
        }
    }
    IEnumerator CrashPrevention(Collider target)
    {
        brake = true;
        BoxCollider otherBoxCollider = target.GetComponent<BoxCollider>();
        if (otherBoxCollider == null)
        {
            yield break;
        }
        while (true)
        {
            if(eBrake){
                speedReducer = 0;
                yield return null;
            }
            if (target == null)
            {
                speedReducer = 1;
                brake = false;
                yield break;
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
                yield break;
            }
            if (distance < absoulteStop)
            {
                speedReducer = 0;
            }
            yield return null;
        }
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
            debugI = i;
            while (Vector3.Distance(transform.position, path[i]) > ARBITRARY_MIN_DISTANCE)
            {
                //find the vector pointing from our position to the target
                _direction = (path[i] - transform.position).normalized;

                //create the rotation we need to be in to look at the target
                _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));

                //rotate us over time according to speed until we are in the required rotation
                //thisRigidbody.rotation = Quaternion.Slerp(Quaternion.Euler(0, thisRigidbody.rotation.eulerAngles.y, 0), _lookRotation, Time.deltaTime * RotationSpeed);
                //thisRigidbody.AddForce(transform.forward*Acceleration,ForceMode.Acceleration);
                transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, t * 2);
                transform.position = Vector3.Lerp(transform.position, path[i], t);


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
    IEnumerator GoRigid()
    {
        Quaternion _lookRotation;
        Vector3 _direction;

        float forceMagnitude = 10.0f; // Adjust this value as needed
        float maxSpeed = 5.0f;
        float t = 0;
        for (int i = 0; i < path.Length; i++)
        {
            debugI = i;
            while (Vector3.Distance(transform.position, path[i]) > ARBITRARY_MIN_DISTANCE)
            {
                Vector3 direction = (path[i] - transform.position).normalized;

                float currentSpeed = Mathf.Min(maxSpeed, Vector3.Distance(transform.position, path[i]) / 2.0f);

                // Create a force vector in the direction of movement
                Vector3 force = direction * forceMagnitude;

                // Apply the force to the rigidbody
                thisRigidbody.AddForce(force, ForceMode.Force);

                // Rotate the object to face the target direction over time
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, t);

                // Increase the 't' parameter for smooth interpolation
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
