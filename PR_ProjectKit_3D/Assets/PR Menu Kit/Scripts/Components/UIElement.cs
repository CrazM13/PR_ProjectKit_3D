using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	#region Events
	public UnityEvent OnBeginHover { get; set; } = new UnityEvent();
	public UnityEvent OnEndHover { get; set; } = new UnityEvent();
	#endregion

	#region Properties
	public float TimeAlive { get; private set; } = 0;
	public float UnscaledTimeAlive { get; private set; } = 0;
	public bool IsHovering { get; set; } = false;
	public bool IsSelected {
		get => EventSystem.current.currentSelectedGameObject == gameObject;
		set {
			if (value) {
				EventSystem.current.SetSelectedGameObject(gameObject);
			} else if (IsSelected) {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}
	#endregion

	#region Unity Event Handlers
	public void OnPointerEnter(PointerEventData eventData) {
		IsHovering = true;
		OnBeginHover.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData) {
		IsHovering = false;
		OnEndHover.Invoke();
	}

	private void Update() {
		TimeAlive += Time.deltaTime;
		UnscaledTimeAlive += Time.unscaledDeltaTime;
	}
	#endregion

}
