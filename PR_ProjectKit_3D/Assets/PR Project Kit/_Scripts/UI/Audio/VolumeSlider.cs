using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

	private const string WARNING_NO_MIXER = "[Volume Slider] Warning! No Audio Mixer assigned!";
	private const string WARNING_NO_SLIDER = "[Volume Slider] Warning! No Slider assigned!";

	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private Slider slider;
	[SerializeField] private string propertyName;
	[SerializeField] private bool decibleConversion;

	// Start is called before the first frame update
	void Start() {
		if (!slider) {
			Debug.LogWarning(WARNING_NO_SLIDER);
			return;
		}

		slider.onValueChanged.AddListener(OnSliderValueChanged);
		audioMixer.GetFloat(propertyName, out float decibles);

		if (decibleConversion) {
			slider.minValue = 0.0001f;
			slider.maxValue = 1f;
			slider.value = DecibleToValue(decibles);
		} else {
			slider.value = decibles;
		}
	}

	private void OnSliderValueChanged(float value) {
		if (!audioMixer) {
			Debug.LogWarning(WARNING_NO_MIXER);
			return;
		}

		float newDecibles = 0;

		if (decibleConversion) {
			newDecibles = ValueToDecible(value);
		} else {
			newDecibles = value;
		}

		audioMixer.SetFloat(propertyName, newDecibles);
	}

	private float ValueToDecible(float value) {
		return Mathf.Log10(value) * 20;
	}

	private float DecibleToValue(float decible) {
		return Mathf.Pow(10, decible / 20);
	}

}
