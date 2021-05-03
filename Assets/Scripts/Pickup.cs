using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
	public int amt;
	public GameObject colliderToActivate;
	public EnemyWaypointNav spiderToAffect;
	public Image shield;
	public Image dot;
	GameManager gameManager;
	
	AudioSource audioSrc;
	MeshRenderer renderer;
	Collider collider;
	ParticleSystem particles;



	void Start()
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
		audioSrc = GetComponent<AudioSource>();
		renderer = GetComponent<MeshRenderer>();
		collider = GetComponent<Collider>();
		particles = GetComponent<ParticleSystem>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (Time.timeScale > 0)
		{
			audioSrc.Play();
			gameManager.AddScore(amt);
			renderer.enabled = collider.enabled = false;
			if (particles != null)
			{
				particles.enableEmission = false;
				if(tag == "Freeze")
				{
					gameManager.Freeze();
				}
			}
			else
			{
				gameManager.Collect(tag);
			}
			if (colliderToActivate != null)
			{
				colliderToActivate.SetActive(true);
			}
			if (spiderToAffect != null)
			{
				spiderToAffect.speed /= 2;
			}
			if (shield != null)
			{
				shield.enabled = false;
			}
			if (dot != null)
			{
				dot.enabled = false;
			}
		}
	}
}
