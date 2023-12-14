using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Video;

public class CarBehavior : MonoBehaviour
{
    [field: SerializeField]
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
    public bool stopped = false;
    public float RotationSpeed;
    int currentI = 0;
    public PathingCell Current;
    public PathingCell IntersectionEntranceCell;
    public PathingCell Next;
    float currentSpeed = 0;
    [SerializeField] private float initialAcceleration = 1f;
    [SerializeField] float speedReducer = 1;
    //public CarBehavior currentTarget;
    public CarBehavior currentTarget;
    public CarBehavior currentIgnore = null;
    public bool RightOfWay = false;
    public bool AwaitingRightOfWay = false;
    public bool OutsideOverride { get; set; } = false;

    AStar pathfinder;
    Building home;

    Transform body;
    Renderer bodyRend;
    bool redTicking = false;
    Coroutine GlowingRed;
    public const float RED_DELAY = 3f;
    Coroutine CurrentBrake = null;
    void Awake()
    {
        body = transform.GetChild(0);
        bodyRend = body.GetComponent<Renderer>();
        transform.Translate(new Vector3(0, YClipPrevention, 0));
        enabled = false;
    }
    public void Setup(AStar a, Building h)
    {
        pathfinder = a;
        home = h;
        this.enabled = true;
    }
    public void setPath(PathingCell[] path, float maxSpeed = ActualMaxSpeed)
    {
        if (enabled == false)
        {
            throw new Exception("Car needs AStar to pathfind autonomously!");
        }
        MaxSpeed = maxSpeed;
        this.path = path;
        StartCoroutine(Go());
        StartCoroutine(TargetSearch());
        //.25seconds is average human reaction
    }
    public IEnumerator setTarget(Vector3Int start, Vector3Int target, float maxSpeed = ActualMaxSpeed)
    {
        if (enabled == false)
        {
            throw new Exception("Car needs AStar to pathfind autonomously!");
        }
        PathingCell[] constructedPath = null;
        while (constructedPath == null)
        {
            constructedPath = pathfinder.CompleteCoAstar(start, target);
            yield return null;
        }
        MaxSpeed = maxSpeed;
        this.path = constructedPath;
        StartCoroutine(Go());
        StartCoroutine(TargetSearch());

    }    /*
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
        while (true)
        {
            if (!brake && !OutsideOverride)
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
                            if (RightOfWay)
                            {
                                if(!cc[i].AwaitingRightOfWay && !cc[i].RightOfWay){
                                    if (!cc[i].Equals(currentIgnore))
                                    {
                                        finder = cc[i];
                                    }
                                    break;
                                }
                                else if (!cc[i].AwaitingRightOfWay || cc[i].RightOfWay)
                                {
                                    if (!cc[i].Equals(currentIgnore))
                                    {
                                        finder = cc[i];
                                    }
                                    break;
                                }
                                else
                                {
                                    //keep going if in intersection and other car is not in intersection
                                }
                            }
                            else
                            {
                                if (!cc[i].Equals(currentIgnore))
                                {
                                    finder = cc[i];
                                    break;
                                }
                            }
                            //Debug.Log("Found target within cell");

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
                            if (RightOfWay)
                            {
                                if (!ccN[i].AwaitingRightOfWay || ccN[i].RightOfWay)
                                {
                                    if (!ccN[i].Equals(currentIgnore))
                                    {
                                        finder = ccN[i];
                                        break;
                                    }
                                }
                                else
                                {
                                    //see previous if
                                }
                            }
                            else
                            {
                                if (!ccN[i].Equals(currentIgnore))
                                {
                                    finder = ccN[i];
                                    break;
                                }
                            }
                            //Debug.Log("Found target within next cell");
                        }
                    }
                }
                if (finder != null)
                {
                    currentTarget = finder;
                    CurrentBrake = StartCoroutine(CrashPrevention(finder));
                    //Debug.Log("Starting Braking");
                }
            }
            yield return null;
        }
    }
    Vector3 frontCenter;
    Vector3 closestPoint;
    void StopBrake()
    {
        speedReducer = 1;
        brake = false;
        currentTarget = null;
        NoMoreRed();
    }
    public void AbsoluteRightOfWay()
    {
        RightOfWay = true;
        StopCoroutine(CurrentBrake);
        StopBrake();
        OutsideOverride = true;
    }
    IEnumerator CrashPrevention(CarBehavior target)
    {
        currentIgnore = null;
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
            Vector3 thisToTarget = target.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, thisToTarget.normalized);
            float distance = Vector3.Distance(frontCenter, closestPoint);
            if (dotProduct < 0)
            {
                StopBrake();
                currentIgnore = target;
                yield break;
            }
            //if they are targetting each other
            if (currentTarget.currentTarget != null && currentTarget.currentTarget.Equals(this))
            {
                StopBrake();
                currentIgnore = target;
                yield break;
            }
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
                stopped = true;
            }
            else
            {
                stopped = false;
                speedReducer = 1 - (distance / DriverViewDist);
            }
            if (!redTicking && Mathf.Approximately(speedReducer, 0))
            {
                GlowingRed = StartCoroutine(GetRed());
                redTicking = true;
            }
            else if (redTicking && !Mathf.Approximately(speedReducer, 0))
            {
                NoMoreRed();
            }
            yield return null;
        }
    }
    IEnumerator GetRed()
    {
        yield return new WaitForSeconds(RED_DELAY);
        bodyRend.material.color = Color.red;
    }
    void NoMoreRed()
    {
        if(GlowingRed != null){
            StopCoroutine(GlowingRed);
        }
        redTicking = false;
        bodyRend.material.color = Color.white;
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
                if (NextCell.groupType == GroupEnum.Road || NextCell.groupType == GroupEnum.Building)
                {
                    if (RightOfWay && Current.cell.groupType != GroupEnum.Intersection)
                    {
                        Debug.Log("Turning off Right of Way");
                        TurnOutofIntersection();

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
                        NextCell.intersect.AddCarToQueue(this);
                        IntersectionEntranceCell = Current;
                        while (!RightOfWay)
                        {
                            AwaitingRightOfWay = true;
                            yield return null;
                        }
                        AwaitingRightOfWay = false;
                        Debug.Log("Turning on Right of Way");
                    }
                }
            }
            //transform.position = path[i];
            t = 0;
            Current = Next;
        }
        home.CarReachedDestination(this);

    }
    void TurnOutofIntersection()
    {
        RightOfWay = false;
        IntersectionEntranceCell = null;
        OutsideOverride = false;
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
