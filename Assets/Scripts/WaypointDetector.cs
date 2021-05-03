using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointDetector : MonoBehaviour
{
    public EnemyWaypointNav spider;
    public Transform lastWaypoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint" && other.transform != lastWaypoint)
        {
            spider.HitWaypoint(other.transform);
            lastWaypoint = other.transform;
        }
    }
}
