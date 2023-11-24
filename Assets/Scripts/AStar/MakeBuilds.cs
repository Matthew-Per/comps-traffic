using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class MakeBuilds : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] GridFollower selector;
    [SerializeField] CellHead cellLead;
    [SerializeField] AStar aStar;
    Vector3Int? beginning;
    Vector3Int? destination;
    bool noDoubleOps = false;
    bool inProgress = false;
    int cState = 0;
    [SerializeField] GameObject carFab;
    [SerializeField] GameObject PossibleChoiceFab;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !inProgress)
        {
            inProgress = true;
            StartCoroutine(Assembly());
        }
        if (Input.GetMouseButtonDown(1) && inProgress && !noDoubleOps)
        {
            cState--;
            if (cState < 0)
            {
                StopAllCoroutines();
                cState = 0;
                inProgress = false;
            }
            noDoubleOps = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            noDoubleOps = false;
        }
    }
    IEnumerator Assembly()
    {
        //Home 0 -> Home entrance 1-> Home exit 2-> Dest 3-> dest entrance 4-> dest exit 5 -> finish call CellHead
        Vector3Int home = Vector3Int.zero;
        cState = 0;
        //I could do a switch but idk
        while (inProgress)
        {
            while (cState == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var selectorPos = selector.getGridPos();
                    if (!cellLead.hasCell(selectorPos))
                    {
                        home = selectorPos;
                        cState = 1;
                    }
                }
                yield return null;
            }
            Direction homeEntrance = null;
            Vector3Int homeEntranceV3I;
            List<GameObject> possibleEntrances = new List<GameObject>();
            Dictionary<Vector3Int, Direction> possibleEntranceDirections = new Dictionary<Vector3Int, Direction>();
            if (cState == 1)
            {
                selector.Stop();
                foreach (Direction d in Direction.Directions)
                {
                    var translatedCell = home + d.Translation;
                    if (cellLead.hasCell(translatedCell))
                    {
                        var vec3 = grid.CellToWorld(translatedCell);
                        var choice = Instantiate(PossibleChoiceFab, vec3, Quaternion.identity);
                        possibleEntrances.Add(choice);
                        possibleEntranceDirections.Add(translatedCell, d.Opposing());
                    }
                }
                while (cState == 1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var selectorPos = selector.OverrideStopForPos();
                        if (cellLead.hasCell(selectorPos) && possibleEntranceDirections.ContainsKey(selectorPos))
                        {
                            homeEntrance = possibleEntranceDirections[selectorPos];
                            homeEntranceV3I = selectorPos;
                            foreach (GameObject g in possibleEntrances)
                            {
                                Destroy(g);
                            }
                            cState = 2;
                        }
                    }
                    yield return null;
                }
            }
            while (cState == 2)
            {

                yield return null;
            }
            //reset everything if going back a step;
            selector.Start();
            foreach (GameObject g in possibleEntrances)
            {
                Destroy(g);
            }
            yield return null;
        }
        //TODO: end logic
    }
}
