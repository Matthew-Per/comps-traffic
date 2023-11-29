using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleCell : MonoBehaviour
{
    Renderer rend;
    Material mat;
    Color mouseOverIn = new Color(15f/255,30f/255,1);
    Color mouseOverOut = new Color(1,150f/255,0);
    Color mouseGONE = new Color(1,1,1,.5f);
    public bool isIn = true;
    void Start(){
        rend = GetComponent<Renderer>();
        mat = rend.material;
        mat.color = mouseGONE;
    }
    void OnMouseEnter(){
        if(isIn){
            mat.color = mouseOverIn;
        }
        else{
            mat.color = mouseOverOut;
        }
    }
    void OnMouseExit(){
        mat.color = mouseGONE;
    }
}
