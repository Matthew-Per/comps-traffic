using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
public enum UserMode
{
    Road,
    Path,
    UI
}

public class UserModeSelect : MonoBehaviour
{
    [SerializeField]
    UserMode mode = UserMode.Road;
    [SerializeField]
    GameObject roadDirector;
    [SerializeField] GameObject pathDirector;
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Material[] materials;
    bool pathState = false;
    bool roadState = true;
    bool tuneState = false;
    UserMode prevMode;
    public void setToRoad()
    {
        pathState = false;
        tuneState = false;
        roadState = true;
        selector.GetComponent<Renderer>().material = materials[0];
        prevMode = UserMode.Road;
        if(mode != UserMode.UI){
            mode = UserMode.Road;
            pathDirector.SetActive(pathState);
            roadDirector.SetActive(roadState);
        }


    }
    public void setToPath()
    {
        pathState = true;
        roadState = false;
        tuneState = false;
        prevMode = UserMode.Path;
        selector.GetComponent<Renderer>().material = materials[1];
        if (mode != UserMode.UI)
        {
            mode = UserMode.Path;
            roadDirector.SetActive(roadState);
            pathDirector.SetActive(pathState);
        }

    }
    public void setToTune(){
        
    }
    public void OnUIHover()
    {
        prevMode = mode;
        mode = UserMode.UI;
        roadDirector.SetActive(false);
        pathDirector.SetActive(false);
    }
    public void OnUIExit()
    {
        mode = prevMode;
        roadDirector.SetActive(roadState);
        pathDirector.SetActive(pathState);
    }
}
