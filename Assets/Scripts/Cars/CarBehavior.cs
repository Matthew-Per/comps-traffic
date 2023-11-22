using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

public class CarBehavior : MonoBehaviour
{
    [SerializeField]
    PathingCell[] path = null;
    const float ARBITRARY_MIN_DISTANCE = 0.35f;
    const float ActualMaxSpeed = 1f;
    const float CellSize = 2.5f;
    [SerializeField] const float DriverViewDist = 3f;
    const float YClipPrevention = .25f;
    float MaxSpeed = 1f;
    const float absoulteStop = .25f;
    [SerializeField] bool brake = false;
    [SerializeField] bool eBrake = false;
    public float RotationSpeed;
    int currentI = 0;
    PathingCell Current;
    PathingCell Next;
    float currentSpeed = 0;
    [SerializeField] private float initialAcceleration = 1f;
    [SerializeField] float speedReducer = 1;
    //public CarBehavior currentTarget;
    public CarBehavior GizmoTarget;
    public bool RightOfWay = false;
    bool IntersectIgnore = false;
    void Awake()
    {
        transform.Translate(new Vector3(0, YClipPrevention, 0));
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
    IEnumerator TargetSearch()
    {
        Coroutine CurrentBrake = null;
        while (true)
        {
            if (!brake || !IntersectIgnore)
            {
                CarBehavior finder = null;
                var cc = Current.cell.CurrentCars;//should be current cell
                int thisIndex = cc.IndexOf(this);
                if (thisIndex > 0)
                {
                    for (int i = thisIndex - 1; i >= 0; i--)
                    {
                        if (!cc[i].Equals(this))
                        {
                            finder = cc[i];
                            //Debug.Log("Found target within cell");
                            break;
                        }
                    }

                }
                else if (path[currentI] != null)
                {
                    var ccN = path[currentI].cell.CurrentCars;
                    for (int i = ccN.Count - 1; i >= 0; i--)
                    {
                        if (!ccN[i].Equals(this))
                        {
                            finder = ccN[i];
                            //Debug.Log("Found target within next cell");
                            break;
                        }
                    }
                }
                if (finder != null)
                {
                    GizmoTarget = finder;
                    CurrentBrake = StartCoroutine(CrashPrevention(finder));
                    //Debug.Log("Starting Braking");
                }
            }
            else if(IntersectIgnore && CurrentBrake != null){
                StopCoroutine(CurrentBrake);
                StopBrake();
            }
            yield return null;
        }
    }
    Vector3 frontCenter;
    Vector3 closestPoint;
    void StopBrake(){
                speedReducer = 1;
                brake = false;
                GizmoTarget = null;
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

            try
            {
                otherBoxCollider = target.GetComponent<BoxCollider>();
                frontCenter = transform.TransformPoint(Vector3.forward * .8f);
                closestPoint = otherBoxCollider.ClosestPoint(frontCenter);
            }
            catch (Exception e)
            {
                StopBrake();
                yield break;
            }
            float distance = Vector3.Distance(frontCenter, closestPoint);
            //TODO
            /*var angle = Mathf.Abs(Vector3.SignedAngle(frontCenter, closestPoint,transform.position));
            Debug.Log(angle);
            if(angle > 90f){
               
                
            }
            */
            if (distance >= DriverViewDist)
            {
                StopBrake();
                yield break;
            }
            if (distance <= absoulteStop)
            {
                speedReducer = 0;
            }
            else
            {
                speedReducer = 1 - (distance / DriverViewDist);
            }
            yield return null;
        }
    }
    IEnumerator Go()
    {
        float t = 0;
        Current = path[0];
        for (int i = 0; i < path.Length; i++)
        {
            currentI = i;
            Next = path[i];
            var NextPos = new Vector3(path[i].pos.x, path[i].pos.y + YClipPrevention, path[i].pos.z);
            var NextCell = path[i].cell;
            var CurrentCell = Current.cell;
            Vector3 init = transform.position;
            if (CurrentCell.groupType == GroupEnum.Intersection)
            {
                    while (RightOfWay && Vector3.Distance(transform.position, NextPos) > ARBITRARY_MIN_DISTANCE)
                    {
                        DriveTo(init, NextPos, ref t);
                        yield return null;
                    }
            }
            else
            {
                IntersectIgnore = false;
                if (NextCell.groupType == GroupEnum.Road)
                {
                    if (RightOfWay && Current.cell.groupType != GroupEnum.Intersection)
                    {
                        Debug.Log("Turning off Right of Way");
                        RightOfWay = false;
                    }
                    while (Vector3.Distance(transform.position, NextPos) > ARBITRARY_MIN_DISTANCE)
                    {
                        DriveTo(init, NextPos, ref t);
                        yield return null;
                    }
                }
                else if (NextCell.groupType == GroupEnum.Intersection)
                {
                    if (!RightOfWay)
                    {
                        Vector3 v = Current.NextDirection.Translation;
                        NextPos -= (CellSize / 2) * v;
                        var initDist = Vector3.Distance(transform.position, NextPos);
                        while (Vector3.Distance(transform.position, NextPos) > ARBITRARY_MIN_DISTANCE)
                        {
                            DriveTo(init, NextPos, ref t);
                            yield return null;
                        }
                        //TODO: continue if intersection
                        //yield return new WaitForSeconds(.5f);
                        NextCell.group.AddCarToQueue(this);
                        IntersectIgnore = true;
                        while (!RightOfWay)
                        {
                            yield return null;
                        }
                        Debug.Log("Turning on Right of Way");
                    }
                }
            }
            //transform.position = path[i];
            t = 0;
            Current = Next;
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);

    }
    void DriveTo(Vector3 initial, Vector3 NextPos, ref float t)
    {
        Quaternion _lookRotation;
        Vector3 _direction;
        currentSpeed = MaxSpeed * speedReducer;
        _direction = (NextPos - transform.position).normalized;
        _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, t * 2);
        transform.position = Vector3.Lerp(initial, NextPos, t);
        t += currentSpeed * Time.deltaTime;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(frontCenter, closestPoint);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Current.pos, new Vector3(2.5f, 0.1f, 2.5f));
        Gizmos.DrawCube(Next.pos, new Vector3(2.5f, 0.1f, 2.5f));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, Next.pos);
    }
}
