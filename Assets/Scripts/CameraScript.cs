using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public bool isTracking = true;
    public float xDist = 14;
    public float yDist = 10;
    public float zDist = 0;
    private Vector3 dist;

    // Start is called before the first frame update
    void Start()
    {
        dist = new Vector3(xDist, yDist, zDist);
    }

    void FixedUpdate()
    {
        if (isTracking)
        {
            transform.position = player.transform.position + dist;
        }
    }
}
