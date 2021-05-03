using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject sardinePrefab;
    public GameObject whalePrefab;
    private float nextSardine;
    private float nextWhale;
    private float sardineTimer = 0;
    private float whaleTimer = 0;

    void Start()
    {
        // Populate area with initial fish
        int numSardines = Random.Range(120, 180);
        for (int i = 0; i < numSardines; ++i)
        {
            GameObject sardine = Instantiate(sardinePrefab, new Vector3(Random.Range(-300f, 300f), Random.Range(2f, 7f), Random.Range(-300f, 300f)), Quaternion.identity);
            sardine.transform.eulerAngles = Vector3.up * 180;
            Rigidbody rigidbody = sardine.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.back * Random.Range(4f, 6f);
        }

        int numWhales = Random.Range(15, 20);
        for (int i = 0; i < numWhales; ++i)
        {
            GameObject whale = Instantiate(whalePrefab, new Vector3(Random.Range(-60f, -24f), 7, Random.Range(-300f, 300f)), Quaternion.identity);
            Rigidbody rigidbody = whale.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.back * Random.Range(1f, 3f);
        }

        nextSardine = Random.Range(0.5f, 1.5f);
        nextWhale = Random.Range(5, 10);
    }

    void Update()
    {
        sardineTimer += Time.deltaTime;
        if (sardineTimer > nextSardine)
        {
            GameObject sardine = Instantiate(sardinePrefab, new Vector3(Random.Range(-300f, 300f), Random.Range(2f, 7f), 300), Quaternion.identity);
            sardine.transform.eulerAngles = Vector3.up * 180;
            Rigidbody rigidbody = sardine.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.back * Random.Range(4f, 6f);
            nextSardine = Random.Range(0.5f, 1.5f);
            sardineTimer = 0;
        }
        
        whaleTimer += Time.deltaTime;
        if (whaleTimer > nextWhale)
        {
            GameObject whale = Instantiate(whalePrefab, new Vector3(Random.Range(-60f, -24f), 7, 300), Quaternion.identity);
            Rigidbody rigidbody = whale.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.back * Random.Range(1f, 3f);
            nextWhale = Random.Range(5, 10);
            whaleTimer = 0;
        }
    }
}
