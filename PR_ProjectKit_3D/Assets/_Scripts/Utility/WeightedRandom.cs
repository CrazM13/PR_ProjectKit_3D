using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom {

	public static int GetRandomWeightedIndex(int[] weights) {
		int maxWeights = 0;
		for (int i = 0; i < weights.Length; ++i) {
			maxWeights += weights[i];
		}

		int lastIndex = weights.Length - 1;
		for (int index = 0;  index < lastIndex; index++) {
			int randomNumber = Random.Range(0, maxWeights);
			if (randomNumber < weights[index]) {
				return index;
			}

			index++;
			maxWeights -= weights[index];
		}

		return lastIndex;
	}

	public static int GetRandomWeightedIndex<T>(params WeightedItem<T>[] list) {
		int maxWeights = 0;
		for (int i = 0; i < list.Length; ++i) {
			maxWeights += list[i].weight;
		}

		int lastIndex = list.Length - 1;
		for (int index = 0; index < lastIndex; index++) {
			int randomNumber = Random.Range(0, maxWeights);
			if (randomNumber < list[index].weight) {
				return index;
			}

			index++;
			maxWeights -= list[index].weight;
		}

		return lastIndex;
	}

	public static T GetRandomWeightedItem<T>(params WeightedItem<T>[] list) {
		int maxWeights = 0;
		for (int i = 0; i < list.Length; ++i) {
			maxWeights += list[i].weight;
		}

		int lastIndex = list.Length - 1;
		for (int index = 0; index < lastIndex; index++) {
			int randomNumber = Random.Range(0, maxWeights);
			if (randomNumber < list[index].weight) {
				return list[index].item;
			}

			index++;
			maxWeights -= list[index].weight;
		}

		return list[lastIndex].item;
	}

	public class WeightedItem<T> {
		[SerializeField] public T item;
		[SerializeField, Min(1)] public int weight = 1;
	}

}
