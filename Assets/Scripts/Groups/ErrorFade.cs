using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorFade : MonoBehaviour
{
    // Start is called before the first frame update
    const float fadeTime = 3f;
    [SerializeField] bool inOrOut = false;
    [SerializeField] float elapsedTime = 0;
    void Awake(){
        StartCoroutine(tillDeath());
    }
    IEnumerator tillDeath(){
        Renderer r = gameObject.GetComponent<Renderer>();
        Material m = r.material;
        Color faded = new Color(m.color.r,m.color.g,m.color.b,.5f);
        Color full = new Color(m.color.r,m.color.g,m.color.b,1f);
        Color init = m.color;
        while(true){
            if(!inOrOut){
                if(r.material.color.a <= faded.a){
                    inOrOut = true;
                    elapsedTime = 0;
                    init = r.material.color;
                }
                r.material.color = Color.Lerp(init, faded, elapsedTime/fadeTime);
                elapsedTime += Time.deltaTime;
            }   
            else{
                if(r.material.color.a >= full.a){
                    inOrOut = false;
                    elapsedTime = 0;
                    init = r.material.color;
                }
                r.material.color = Color.Lerp(init, full, elapsedTime/fadeTime);
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }
}
