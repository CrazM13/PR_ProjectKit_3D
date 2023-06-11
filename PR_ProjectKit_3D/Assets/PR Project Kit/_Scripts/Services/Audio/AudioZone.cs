using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Zone))]
public class AudioZone : MonoBehaviour {

	[Header("Tracks")]
	[SerializeField] private int enterTrackIndex;
	[SerializeField] private int exitTrackIndex;

	[Header("Settings")]
	[SerializeField] private MusicPlayer overrideMusicPlayer;
	[SerializeField] private bool muteOnCooldown;
	[SerializeField] private float cooldownToChange;

	private int currentTrack = -1;

	private int changeTotrack = -1;
	private float changeTimer = -1;

	private void OnZoneEnter(Zone e) {
		changeTotrack = enterTrackIndex;

		changeTimer = 0;

		if (muteOnCooldown) {
			PlayTrack(-1);
		}
	}

	private void OnZoneExit(Zone e) {
		changeTotrack = exitTrackIndex;

		changeTimer = 0;

		if (muteOnCooldown) {
			PlayTrack(-1);
		}
	}

	private void Update() {
		if (changeTimer >= 0) {
			changeTimer += GameTime.UnscaledDeltaTime;

			if (changeTimer >= cooldownToChange) {
				changeTimer = -1;

				PlayTrack(changeTotrack);
				currentTrack = changeTotrack;
			}

		}
	}

	private void PlayTrack(int track) {
		if (track == currentTrack) return;

		if (overrideMusicPlayer) {
			overrideMusicPlayer.PlayMusic(track);
		} else {
			PRAudioManager audioManager = ServiceLocator.AudioManager;

			if (audioManager && audioManager.MusicController) {
				audioManager.MusicController.PlayMusic(track); ;
			}
		}
	}

}
