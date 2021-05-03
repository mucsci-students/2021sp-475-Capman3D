using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
/*
 Transform tr_Player;
 float f_RotSpeed=3.0f,f_MoveSpeed = 3.0f;
 // Use this for initialization
 void Start () {
  tr_Player = GameObject.FindGameObjectWithTag ("Player").transform;
 }
 
 // Update is called once per frame
 void Update () {
  /* Look at Player
  transform.rotation = Quaternion.Slerp (transform.rotation
                                        , Quaternion.LookRotation (tr_Player.position - transform.position)
                                        , f_RotSpeed * Time.deltaTime);
  
  /* Move at Player
  transform.position += transform.forward * f_MoveSpeed * Time.deltaTime;
 }*/

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float sightRange;
    public bool playerInSightRange;
    //public static Enemy instance;

    private void Awake(){
        player = GameObject.FindGameObjectWithTag ("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        //instance = this;

    }

    private void Update(){
        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if(!playerInSightRange) Patroling();
        if(playerInSightRange) ChasePlayer();

    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f){
            walkPointSet = false;
        }

    }

    private void SearchWalkPoint(){
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)){
            walkPointSet = true;
        }
    }

    private void ChasePlayer(){
       agent.SetDestination(player.position);
      // agent.SetDestination(PlayerScript.instance.transform.position);
    }

}