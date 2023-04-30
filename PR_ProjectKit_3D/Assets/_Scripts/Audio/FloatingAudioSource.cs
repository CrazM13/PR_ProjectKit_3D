using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FloatingAudioSource : MonoBehaviour {

	[SerializeField] private AudioSource audioSource;

	private float timeUntilStop = -1;
	private AudioManager manager;

	void Update() {
		if (timeUntilStop > 0) {
			timeUntilStop -= Time.unscaledDeltaTime;
			if (timeUntilStop <= 0) {
				manager.RemoveFloatingAudioSource(gameObject);
			}
		}
	}

	public void Play(AudioManager manager, AudioClip clip, AudioMixerGroup mixerGroup) {
		audioSource.clip = clip;
		timeUntilStop = clip.length;
		audioSource.outputAudioMixerGroup = mixerGroup;
		audioSource.Play();

		this.manager = manager;
	}
}
