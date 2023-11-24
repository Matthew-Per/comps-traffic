using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleCell : MonoBehaviour
{
    Renderer rend;
    Material mat;
    Color mouseOver = new Color(1, 0, 1);
    Color mouseGONE = new Color(1,110f/255,1);
    void Start(){
        rend = GetComponent<Renderer>();
        mat = rend.material;
    }
    void OnMouseEnter(){
        mat.color = mouseOver;
    }
    void OnMouseExit(){
        mat.color = mouseGONE;
    }
}
