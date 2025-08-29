using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
	public List<AudioClip> audioClips;

	private int audioIndex = 0;

	private bool isMusicPlaying = false;
	private AudioSource audioSource;

	// Use this for initialization
	void Start ()
	{
		this.audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// If music should be playing at all.
		if (this.isMusicPlaying)
		{
			// Music has ended
			if (!this.audioSource.isPlaying)
			{
				// Start music.
				this.audioSource.clip = this.audioClips[this.audioIndex];
				this.audioSource.Play();

				// Switch to the next audio track.
				++this.audioIndex;

				if (this.audioIndex >= audioClips.Count)
				{
					this.audioIndex = 0;
				}
			}
		}

		// Hotkey for stopping music just in case :)
		if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftAlt))
		{
			if (this.isMusicPlaying)
			{
				this.StopMusic();
			}
			else
			{
				this.StartMusic();
			}
		}
	}

	public void StartMusic()
	{
		this.isMusicPlaying = true;
		this.audioSource.UnPause();
	}

	public void StopMusic()
	{
		this.isMusicPlaying = false;
		this.audioSource.Pause();
	}
}
