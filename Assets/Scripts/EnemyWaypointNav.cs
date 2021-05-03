using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaypointNav : MonoBehaviour
{
    public GameObject player;
    public float speed = 4.5f;
    public float realSpeed;
    public enum Color
    {
        Red,
        Blue,
        Yellow,
        Green
    }
    public Color color;
    public GameObject redSpider;
    public Image icon;

    private Rigidbody rigidbody;
    private float chaseTime;
    private float chaseTimer = -1;
    private float wanderTime;
    private float wanderTimer = -1;
    public bool chasing;
    private AudioSource audio;

    private Animator anim;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        chaseTime = Mathf.Min(20, 8 + (PlayerPrefs.GetInt("RoundNumber") * 2));
        wanderTime = Mathf.Max(0, 12 - (PlayerPrefs.GetInt("RoundNumber") * 2));
        speed += Mathf.Min(12, PlayerPrefs.GetInt("RoundNumber"));
        chaseTimer = Random.Range(chaseTime - 2, chaseTime + 2);
        realSpeed = speed;
        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (realSpeed > 0)
        {
            anim.speed = 1;
            realSpeed = speed;
            if (wanderTime > 0)
            {
                if (wanderTimer >= 0)
                {
                    chasing = false;
                    wanderTimer -= Time.deltaTime;
                    if (wanderTimer < 0)
                    {
                        wanderTimer = -1;
                        chaseTimer = Random.Range(chaseTime - 2, chaseTime + 2);
                    }
                }
                else if (chaseTimer >= 0)
                {
                    chasing = true;
                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer < 0)
                    {
                        chaseTimer = -1;
                        wanderTimer = Random.Range(wanderTime - 2, wanderTime + 2);
                    }
                }
            }
            else
            {
                // After a certain level, only chase
                chasing = true;
            }
        }
        else
        {
            anim.speed = 0;
        }

        rigidbody.velocity = transform.forward * realSpeed;
    }

    public void HitWaypoint(Transform waypoint)
    {
        Vector3 target = Vector3.zero;
        Vector3 originalDirection = transform.forward;
        if (color == Color.Red)
        {
            target = player.transform.position;
        }
        else if (color == Color.Green)
        {
            target = player.transform.position + player.transform.forward * 12;
        }
        else if (color == Color.Blue)
        {
            if (redSpider.transform.localScale.y > 0.1f)
            {
                target = redSpider.transform.position;
            }
            else
            {
                target = player.transform.position;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 24)
            {
                target = player.transform.position;
            }
            else
            {
                chasing = false;
            }
        }

        float bestRot = 0;
        float bestDist = Mathf.Infinity;
        RaycastHit hit;
        if (chasing)
        {
            // Check left
            Physics.Raycast(waypoint.position + transform.right * -1, transform.right * -1, out hit);
            if (hit.collider.gameObject.tag == "Player")
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 90, 0);
                transform.position = waypoint.position;
                return;
            }
            else if (hit.collider.gameObject.tag == "Waypoint")
            {
                float dist = Vector3.Distance(target, waypoint.position + (transform.right * -3));
                if (dist < bestDist)
                {
                    bestRot = -90;
                    bestDist = dist;
                }
            }

            // Check forward
            Physics.Raycast(waypoint.position + transform.forward, transform.forward, out hit);
            if (hit.collider.gameObject.tag == "Player")
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                transform.position = waypoint.position;
                return;
            }
            else if (hit.collider.gameObject.tag == "Waypoint")
            {
                float dist = Vector3.Distance(target, waypoint.position + (transform.forward * 3));
                if (dist < bestDist)
                {
                    bestRot = 0;
                    bestDist = dist;
                }
            }

            // Check right
            Physics.Raycast(waypoint.position + transform.right, transform.right, out hit);
            if (hit.collider.gameObject.tag == "Player")
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 90, 0);
                transform.position = waypoint.position;
                return;
            }
            else if (hit.collider.gameObject.tag == "Waypoint")
            {
                float dist = Vector3.Distance(target, waypoint.position + (transform.right * 3));
                if (dist < bestDist)
                {
                    bestRot = 90;
                    bestDist = dist;
                }
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + bestRot, 0);
        }
        else
        {
            float[] options = new float[3];
            int size = 0;
            // Check left
            Physics.Raycast(waypoint.position + transform.right * -1, transform.right * -1, out hit);
            if (hit.collider.gameObject.tag != "Wall")
            {
                options[size] = -90;
                ++size;
            }

            // Check forward
            Physics.Raycast(waypoint.position + transform.forward, transform.forward, out hit);
            if (hit.collider.gameObject.tag != "Wall")
            {
                options[size] = 0;
                ++size;
            }

            // Check right
            Physics.Raycast(waypoint.position + transform.right, transform.right, out hit);
            if (hit.collider.gameObject.tag != "Wall")
            {
                options[size] = 90;
                ++size;
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + options[Random.Range(0, size)], 0);
        }
        if (transform.forward != originalDirection)
        {
            transform.position = waypoint.position;
        }
    }

    public void Kill()
    {
        audio.Play();
        speed = realSpeed = 0;
        transform.localScale = new Vector3(transform.localScale.x, 0.05f, transform.localScale.z);
        foreach (Transform t in transform)
        {
            t.gameObject.tag = "Untagged";
        }
        icon.enabled = false;
    }
}
