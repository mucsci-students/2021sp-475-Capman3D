using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public float xSpeed = 3.5f;
    public float zSpeed = 5f;
    private Vector3 movement;

    void Start()
    {
        movement = new Vector3(xSpeed, 0, zSpeed);
    }

    void Update()
    {
        transform.Translate(movement * Time.deltaTime);
    }
}
