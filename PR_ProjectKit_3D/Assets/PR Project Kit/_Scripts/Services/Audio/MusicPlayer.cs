using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	[SerializeField] private SoundBank musicBank;
	[SerializeField] private AnimationCurve easeIn;
	[SerializeField] private AnimationCurve easeOut;

	private AudioSource musicSource;
	private AudioSource oldMusicSource;

	private float easingTime = -1;

	private float MaxEasingTime => Mathf.Max(easeIn.keys[easeIn.length - 1].time, easeOut.keys[easeOut.length - 1].time);

	private void Update() {
		if (easingTime >= 0) {
			easingTime += GameTime.UnscaledDeltaTime;

			if (musicSource) {
				musicSource.volume = easeIn.Evaluate(easingTime);
			}

			if (oldMusicSource) {
				oldMusicSource.volume = easeOut.Evaluate(easingTime);
			}

			if (easingTime > MaxEasingTime) {
				easingTime = -1;

				if (oldMusicSource) {
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
				ServiceLocator.AudioManager.ResetAudioSource(oldMusicSource);
				oldMusicSource = null;
			}

			oldMusicSource = musicSource;
			musicSource = null;
		}

		if (index >= 0) musicSource = ServiceLocator.AudioManager.Play(musicBank.GetAt(index));

		easingTime = 0;
	}


}
