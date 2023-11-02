using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum UserMode{
    Road,
    Path
}

public class UserModeSelect : MonoBehaviour
{
    [SerializeField]
    UserMode mode = UserMode.Road;
    [SerializeField]
    GameObject roadDirector;
    [SerializeField]
    GameObject pathDirector;
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Material[] materials;
    public void setToRoad(){
        mode = UserMode.Road;
        pathDirector.SetActive(false);
        roadDirector.SetActive(true);
        selector.GetComponent<Renderer>().material = materials[0];
        
    }
    public void setToPath(){
        mode = UserMode.Path;
        roadDirector.SetActive(false);
        pathDirector.SetActive(true);
        selector.GetComponent<Renderer>().material = materials[1];
    }
}
