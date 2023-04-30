using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Group", menuName = "Audio/Audio Group", order = 0)]
public class AudioGroup : ScriptableObject {

	[Header("Settings")]
	[SerializeField] private AudioMixerGroup group;

	[Header("Clips")]
	[SerializeField] private AudioGroupItem[] audioClips;

	private int lastItem = -1;

	/// <summary>
	/// Get random clip from audio group
	/// </summary>
	/// <returns>Random clip from weighted list</returns>
	public AudioClip GetRandomClip() {
		if (audioClips.Length == 1) return audioClips[0].item;
		else if (audioClips.Length == 0) return null;

		return WeightedRandom.GetRandomWeightedItem(audioClips);
	}

	public AudioClip GetRandomClipWithoutRepeating() {
		if (audioClips.Length == 1) return audioClips[0].item;
		else if (audioClips.Length == 0) return null;

		int index = WeightedRandom.GetRandomWeightedIndex(audioClips);
		if (index == lastItem) index = (index + 1) % audioClips.Length;
		lastItem = index;

		return audioClips[index].item;
	}

	/// <summary>
	/// Retreve audio clip from group with the file name <paramref name="name"/>
	/// </summary>
	/// <param name="name">The name of the file</param>
	/// <returns>AudioClip if found. NULL if not found.</returns>
	public AudioClip GetClipByName(string name) {
		for (int i = 0; i < audioClips.Length; i++) {
			if (audioClips[i].item.name == name) return audioClips[i].item;
		}

		return null;
	}

	public AudioMixerGroup GetAudioGroup() {
		return group;
	}

	[System.Serializable]
	public class AudioGroupItem : WeightedRandom.WeightedItem<AudioClip> { /*MT*/ }
}
