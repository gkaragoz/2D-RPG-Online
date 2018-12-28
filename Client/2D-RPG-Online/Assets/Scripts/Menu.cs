using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Menu : MonoBehaviour {

    public bool isOpen { get; set; }

    public virtual void Show() {
        gameObject.SetActive(true);
        isOpen = true;
    }

    public virtual void Show(GameObject container) {
        container.SetActive(true);
        isOpen = true;
    }

    public virtual void Hide() {
        gameObject.SetActive(false);
        isOpen = false;
    }

    public virtual void Hide(GameObject container) {
        container.SetActive(false);
        isOpen = false;
    }

    public virtual void Toggle() {
        gameObject.SetActive(!gameObject.activeSelf);
        isOpen = gameObject.activeSelf;
    }

    public virtual void Toggle(GameObject container) {
        container.SetActive(!container.activeSelf);
        isOpen = container.activeSelf;
    }

}