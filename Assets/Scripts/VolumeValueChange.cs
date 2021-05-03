using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeValueChange : MonoBehaviour
{
	private AudioSource[] audioSrc;
	public float musicVolume = 1f;
    public bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponents<AudioSource>();
        foreach(AudioSource audio in audioSrc)
        {
            audio.volume = musicVolume;
        }
    }
	
	public void SetVolume(float vol)
	{
		musicVolume = vol;
        foreach(AudioSource audio in audioSrc)
        {
            audio.volume = musicVolume;
        }
	}

    public void Play(int track)
    {
        audioSrc[track].Play();
        playing = true;
    }

    public void Stop()
    {
        foreach(AudioSource audio in audioSrc)
        {
            audio.Stop();
        }
        playing = false;
    }
}