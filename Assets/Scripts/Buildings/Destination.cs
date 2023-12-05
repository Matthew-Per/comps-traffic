using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    public bool occupied = false;
    void OnTriggerEnter()
    {
        occupied = true;
    }
    void OnTriggerExit()
    {
        occupied = false;
    }
}
