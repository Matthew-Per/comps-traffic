using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
public struct PossibilitiesContainer
{
    public Vector3Int pos;
    public Direction directionTowards;
    public PossibleCell script;
    public PossibilitiesContainer(Vector3Int pos, Direction directionTowards, PossibleCell script)
    {
        this.pos = pos;
        this.directionTowards = directionTowards;
        this.script = script;
    }
}
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
    [SerializeField] GameObject homeFab;
    [SerializeField] GameObject destFab;
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
        GameObject home = null;
        Vector3Int homeVec = Vector3Int.zero;
        cState = 0;
        //I could do a switch but idk
        if (cState == 0) selector.Follow();
        while (cState == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.getGridPos();
                if (!cellLead.hasCell(selectorPos))
                {
                    homeVec = selectorPos;
                    home = Instantiate(homeFab, selector.transform.position, Quaternion.identity);
                    home.transform.Translate(0, -.05f, 0);
                    cState = 1;
                }
            }
            yield return null;
        }
        Direction homeEntrance = null;
        Vector3Int homeEntranceV3I;
        List<PossibilitiesContainer> possibles = new List<PossibilitiesContainer>();
        if (cState == 1)
        {
            selector.Halt();
            foreach (Direction d in Direction.Directions)
            {
                var translatedCell = homeVec + d.Translation;
                if (cellLead.hasCell(translatedCell))
                {
                    var vec3 = grid.CellToWorld(translatedCell);
                    var choice = Instantiate(PossibleChoiceFab, vec3, Quaternion.identity);
                    var choiceScript = choice.GetComponent<PossibleCell>();
                    possibles.Add(new PossibilitiesContainer(translatedCell, d.Opposite, choiceScript));
                }
            }
        }
        while (cState == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.OverrideStopForPos();

                if (cellLead.hasCell(selectorPos))
                {
                    int i = possibles.FindIndex(item => item.pos == selectorPos);
                    if (i > -1)
                    {
                        var found = possibles[i];
                        homeEntrance = found.directionTowards;
                        homeEntranceV3I = selectorPos;

                        Destroy(found.script.gameObject);
                        possibles.Remove(found);
                        foreach (PossibilitiesContainer g in possibles)
                        {
                            g.script.isIn = false;
                        }
                        cState = 2;
                    }
                }
            }
            yield return null;
        }

        Direction homeExitDirection = null;
        Vector3Int homeExitV3;
        if (cState == 2)
        {
            selector.Halt();
        }
        while (cState == 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.OverrideStopForPos();

                if (cellLead.hasCell(selectorPos))
                {
                    int i = possibles.FindIndex(item => item.pos == selectorPos);
                    if (i > -1)
                    {
                        var found = possibles[i];
                        homeExitDirection = found.directionTowards.Opposite;//entrance is inwards to home exit is outwards to street
                        homeExitV3 = selectorPos;

                        Destroy(found.script.gameObject);
                        possibles.Remove(found);
                        foreach (PossibilitiesContainer g in possibles)
                        {
                            Destroy(g.script.gameObject);
                        }
                        possibles.Clear();
                        cState = 3;
                    }
                }
            }
            yield return null;
        }
        GameObject Dest = null;
        Vector3Int DestVec = Vector3Int.zero;
        if (cState == 3) selector.Follow();
        while (cState == 3)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.getGridPos();
                if (!cellLead.hasCell(selectorPos))
                {
                    DestVec = selectorPos;
                    Dest = Instantiate(destFab, selector.transform.position, Quaternion.identity);
                    Dest.transform.Translate(0, -.05f, 0);
                    cState = 4;
                }
            }
            yield return null;
        }
        Direction destEntrance = null;
        Vector3Int destEntranceV3I;
        List<PossibilitiesContainer> destpossibles = new List<PossibilitiesContainer>();
        if (cState == 4)
        {
            selector.Halt();
            foreach (Direction d in Direction.Directions)
            {
                var translatedCell = DestVec + d.Translation;
                if (cellLead.hasCell(translatedCell))
                {
                    var vec3 = grid.CellToWorld(translatedCell);
                    var choice = Instantiate(PossibleChoiceFab, vec3, Quaternion.identity);
                    var choiceScript = choice.GetComponent<PossibleCell>();
                    destpossibles.Add(new PossibilitiesContainer(translatedCell, d.Opposite, choiceScript));
                }
            }
        }
        while (cState == 4)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.OverrideStopForPos();

                if (cellLead.hasCell(selectorPos))
                {
                    int i = destpossibles.FindIndex(item => item.pos == selectorPos);
                    if (i > -1)
                    {
                        var found = destpossibles[i];
                        destEntrance = found.directionTowards;
                        destEntranceV3I = selectorPos;

                        Destroy(found.script.gameObject);
                        destpossibles.Remove(found);
                        foreach (PossibilitiesContainer g in destpossibles)
                        {
                            g.script.isIn = false;
                        }
                        cState = 5;
                    }
                }
            }
            yield return null;
        }

        Direction destExitDirection = null;
        Vector3Int destExitV3;
        if (cState == 5)
        {
            selector.Halt();
        }
        while (cState == 5)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var selectorPos = selector.OverrideStopForPos();

                if (cellLead.hasCell(selectorPos))
                {
                    int i = destpossibles.FindIndex(item => item.pos == selectorPos);
                    if (i > -1)
                    {
                        var found = destpossibles[i];
                        destExitDirection = found.directionTowards.Opposite;//entrance is inwards to home exit is outwards to street
                        destExitV3 = selectorPos;

                        Destroy(found.script.gameObject);
                        destpossibles.Remove(found);
                        foreach (PossibilitiesContainer g in destpossibles)
                        {
                            g.script.gameObject.SetActive(false);
                        }
                        cState = 6;
                    }
                }
            }
            yield return null;
        }
        //reset everything if going back a step;
        //nvm im too fixated on this
        foreach (PossibilitiesContainer g in possibles)
        {
            Destroy(g.script.gameObject);
        }
        possibles.Clear();
        //just clear all if not finished
        if (cState != 6)
        {
            if (home != null)
            {
                Destroy(home);
            }
            cState = 0;
            inProgress = false;
            selector.Follow();
            yield break;
        }
        else
        {
            //return logic
            Building build = home.GetComponent<Building>();
            cellLead.CreateBuiding(homeVec, DestVec, homeExitDirection, homeEntrance, destEntrance, destExitDirection, build, aStar);
            //build.Setup(aStar,)
            cState = 0;
            inProgress = false;
            selector.Follow();
            yield break;
        }

        //TODO: end logic
    }
}
