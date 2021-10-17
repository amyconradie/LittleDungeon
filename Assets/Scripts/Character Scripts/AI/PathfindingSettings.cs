using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingSettings : MonoBehaviour
{
    private CharacterStats cStats;
    private AIDestinationSetter destinationSetter;
    private AIPath path;
    public Transform target;

    void Start()
    {

        destinationSetter = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        cStats = GetComponent<CharacterStats>();
        path.maxSpeed = cStats.moveSpeed;
        
        // Scans all graphs
        AstarPath.active.Scan();
    }

    void Update()
    {
        destinationSetter.target = target;

        //// Update Graphs
        //AstarPath.active.Scan();
    }
}
