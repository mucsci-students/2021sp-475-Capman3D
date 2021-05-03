using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float runSpeed = 5;
    public float jumpAmount = 8;
    public float recoveryTime = 1;
    public GameManager gameManager;
    public GameObject hands;
    private SkinnedMeshRenderer handsRenderer;
    public GameObject head;
    private SkinnedMeshRenderer headRenderer;
    public GameObject shoes;
    private SkinnedMeshRenderer shoesRenderer;
    public GameObject torso;
    private SkinnedMeshRenderer torsoRenderer;
    public GameObject pants;
    private SkinnedMeshRenderer pantsRenderer;

    public ParticleSystem bubbles;

    private bool isAlive = true;

    private Rigidbody rigidbody;
    private Collider collider;
    private bool isJumping;
    private float justJumpedTimer = -1;
    private Animator anim;
    private float recoveryTimer = -1;
    private AudioSource audio;

    //public static PlayerScript instance;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        //instance = this;

        handsRenderer = hands.GetComponent<SkinnedMeshRenderer>();
        headRenderer = head.GetComponent<SkinnedMeshRenderer>();
        shoesRenderer = shoes.GetComponent<SkinnedMeshRenderer>();
        torsoRenderer = torso.GetComponent<SkinnedMeshRenderer>();
        pantsRenderer = pants.GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (isAlive && Time.timeScale > 0)
        {
            if (justJumpedTimer >= 0)
            {
                justJumpedTimer += Time.deltaTime;
                if (justJumpedTimer > 0.1f)
                {
                    justJumpedTimer = -1;
                }
            }

            if (recoveryTimer >= 0)
            {
                recoveryTimer += Time.deltaTime;
                SetVisible(recoveryTimer % 0.1f < 0.05f);
                if (recoveryTimer > recoveryTime)
                {
                    recoveryTimer = -1;
                    SetVisible(true);
                }
            }

            // Handle directions
            float yRot = transform.eulerAngles.y;
            if (Input.GetKeyDown(KeyCode.D))
            {
                yRot = 0;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                yRot = 90;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                yRot = 180;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                yRot = 270;
            }
            transform.eulerAngles = Vector3.up * yRot;

            // Handle movement
            float xSpeed = 0;
            float zSpeed = 0;
            if (transform.eulerAngles.y == 0)
            {
                zSpeed = runSpeed;
            }
            else if (transform.eulerAngles.y == 90)
            {
                xSpeed = runSpeed;
            }
            else if (transform.eulerAngles.y == 180)
            {
                zSpeed = -runSpeed;
            }
            else
            {
                xSpeed = -runSpeed;
            }
            rigidbody.velocity = new Vector3(xSpeed, rigidbody.velocity.y, zSpeed);
            
            // Handle jump
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                rigidbody.velocity += Vector3.up * jumpAmount;
                isJumping = true;
                justJumpedTimer = 0;
                audio.Play();
            }

            anim.speed = isJumping ? 0.1f : 1;
        }
        else
        {
            
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (justJumpedTimer == -1 && other.gameObject.tag == "Platform")
        {
            isJumping = false;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag == "Platform")
        {
            isJumping = true;
        }
    }

    public void Hit()
    {
        recoveryTimer = 0;
    }

    void SetVisible(bool visible)
    {
        handsRenderer.enabled =
            headRenderer.enabled = 
            shoesRenderer.enabled = 
            torsoRenderer.enabled = 
            pantsRenderer.enabled = visible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crush" && rigidbody.velocity.y < -0.1f)
        {
            other.gameObject.transform.parent.gameObject.BroadcastMessage("Kill");
        }
        else if (other.tag == "Hurt" && recoveryTimer == -1)
        {
            Hit();
            gameManager.Death();
        }
    }

    public void Kill()
    {
        isAlive = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        SetVisible(false);
        recoveryTimer = -1;
        collider.enabled = false;

        if (bubbles != null)
        {
            bubbles.enableEmission = false;
        }
    }
}