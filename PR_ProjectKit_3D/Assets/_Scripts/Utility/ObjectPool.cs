using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> {

	public UnityEvent<T> OnItemEnabled { get; private set; }
	public UnityEvent<T> OnItemDisabled { get; private set; }

	private List<T> activeObjects;
	private Queue<T> inactiveObjects;

	#region Properties
	public int Count => activeObjects.Count + inactiveObjects.Count;
	public int ActiveCount => activeObjects.Count;
	#endregion

	public ObjectPool() {
		OnItemEnabled = new UnityEvent<T>();
		OnItemDisabled = new UnityEvent<T>();

		activeObjects = new List<T>();
		inactiveObjects = new Queue<T>();
	}

	public T CreateNew() {
		if (inactiveObjects.Count == 0) return default;

		T item = inactiveObjects.Dequeue();
		activeObjects.Add(item);
		OnItemEnabled.Invoke(item);

		return item;
	}

	public void Destroy(T item) {
		activeObjects.Remove(item);
		inactiveObjects.Enqueue(item);
		OnItemDisabled.Invoke(item);
	}

	public void AddToPool(T item) {
		inactiveObjects.Enqueue(item);
	}
}
