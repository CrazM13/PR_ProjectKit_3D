using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Sound Bank", menuName = "PR Kit/Audio/Sound Bank", order = 0)]
public class SoundBank : ScriptableObject {

	[SerializeField] private AudioMixerGroup audioGroup;
	[SerializeField] private AudioClip[] sounds;

	public AudioMixerGroup AudioGroup => audioGroup;

	public AudioClip GetAt(int index) {
		return sounds[index];
	}

	public AudioClip GetRandom() {
		int index = Random.Range(0, sounds.Length);
		return GetAt(index);
	}

}
