using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	[Header("Sound Bank")]
	[SerializeField] private SoundBank musicBank;

	[Header("Transition")]
	[SerializeField] private AnimationCurve easeIn;
	[SerializeField] private AnimationCurve easeOut;

	[Header("Settings")]
	[SerializeField, Tooltip("Sets as the main music player. Only one music player can be registered")] private bool registerToAudioManager = false;
	[SerializeField, Min(0)] private float volume = 1;

	private AudioSource musicSource;
	private AudioSource oldMusicSource;

	private float easeInTime = -1;
	private float easeOutTime = -1;

	private float MaxEaseInTime => easeIn.keys[easeIn.length - 1].time;
	private float MaxEaseOutTime => easeOut.keys[easeOut.length - 1].time;

	private void Awake() {
		if (registerToAudioManager) {
			ServiceLocator.AudioManager.MusicController = this;
		}
	}

	private void Update() {
		// Ease In
		if (easeInTime >= 0) {
			easeInTime += GameTime.UnscaledDeltaTime;

			if (musicSource) {
				musicSource.volume = easeIn.Evaluate(easeInTime) * volume;
			}

			if (easeInTime > MaxEaseInTime) {
				easeInTime = -1;
			}
		} else {
			if (musicSource) {
				musicSource.volume = volume;
			}
		}

		// Easde Out
		if (easeOutTime >= 0) {
			easeOutTime += GameTime.UnscaledDeltaTime;

			if (oldMusicSource) {
				oldMusicSource.volume = easeOut.Evaluate(easeOutTime) * volume;
			}

			if (easeOutTime > MaxEaseOutTime) {
				easeOutTime = -1;

				if (oldMusicSource) {
					oldMusicSource.Stop();
					ServiceLocator.AudioManager.ResetAudioSource(oldMusicSource);
					oldMusicSource = null;
				}
			}
		}
	}

	public void PlayMusic(int index) {
		if (musicSource) {
			// Force dispose of old Old Music
			if (oldMusicSource) {
				oldMusicSource.Stop();
				ServiceLocator.AudioManager.ResetAudioSource(oldMusicSource);
				oldMusicSource = null;
			}

			oldMusicSource = musicSource;
			musicSource = null;
			easeOutTime = 0;
		}

		if (index >= 0) {
			musicSource = ServiceLocator.AudioManager.Play(musicBank.GetAt(index));
			musicSource.volume = volume;
		}

		easeInTime = 0;
	}

}
