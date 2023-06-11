using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRAudioManager : MonoBehaviour {

	public MusicPlayer MusicController { get; set; } = null;

	private Queue<AudioSource> inactiveAudioSources = new Queue<AudioSource>();
	private List<AudioSource> activeAudioSources = new List<AudioSource>();

	// Update is called once per frame
	void Update() {
		for (int i = activeAudioSources.Count - 1; i >= 0; i--) {
			if (!activeAudioSources[i].clip || (!activeAudioSources[i].loop && activeAudioSources[i].time >= activeAudioSources[i].clip.length)) {
				DeleteAudioSource(i);
			}
		}
	}

	public AudioSource Play(AudioClip clip, Vector3 position) {
		AudioSource newAudioSource = CreateAudioSource();

		newAudioSource.transform.position = position;

		newAudioSource.clip = clip;
		newAudioSource.Play();

		return newAudioSource;
	}

	public AudioSource Play(AudioClip clip) {
		return Play(clip, Vector3.zero);
	}

	public AudioSource PlayRandom(SoundBank soundBank, Vector3 position) {
		AudioSource newAudioSource = CreateAudioSource();

		newAudioSource.transform.position = position;

		newAudioSource.clip = soundBank.GetRandom();
		newAudioSource.outputAudioMixerGroup = soundBank.AudioGroup;
		newAudioSource.Play();

		return newAudioSource;
	}

	public AudioSource PlayRandom(SoundBank soundBank) {
		return PlayRandom(soundBank, Vector3.zero);
	}

	public AudioSource CreateAudioSource() {
		AudioSource newAudioSource;

		if (inactiveAudioSources.Count > 0) {
			newAudioSource = inactiveAudioSources.Dequeue();
			newAudioSource.gameObject.SetActive(true);
		} else {
			GameObject audioObject = new GameObject($"{name}.NewAudioSource");
			audioObject.transform.SetParent(transform);
			newAudioSource = audioObject.AddComponent<AudioSource>();
		}

		activeAudioSources.Add(newAudioSource);
		return newAudioSource;
	}

	private void DeleteAudioSource(int index) {
		activeAudioSources[index].gameObject.SetActive(false);
		ResetAudioSource(activeAudioSources[index]);

		inactiveAudioSources.Enqueue(activeAudioSources[index]);
		activeAudioSources.RemoveAt(index);
	}

	public void ResetAudioSource(AudioSource audioSource) {
		audioSource.clip = null;
		audioSource.outputAudioMixerGroup = null;
		audioSource.mute = false;
		audioSource.bypassEffects = false;
		audioSource.bypassListenerEffects = false;
		audioSource.bypassReverbZones = false;
		audioSource.playOnAwake = true;
		audioSource.loop = false;
		audioSource.priority = 128;
		audioSource.volume = 1;
		audioSource.pitch = 1;
		audioSource.panStereo = 0;
		audioSource.spatialBlend = 0;
		audioSource.reverbZoneMix = 1;
	}

}
