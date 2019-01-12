using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Menu : MonoBehaviour {

    [Header("Initialization")]
    public GameObject container;

    public bool isOpen { get; set; }

    private void Awake() {
        if (container == null) {
            container = this.gameObject;
        }
    }

    public virtual void Show() {
        container.SetActive(true);
        isOpen = true;
    }

    public virtual void Hide() {
        container.SetActive(false);
        isOpen = false;
    }

    public virtual void Toggle() {
        container.SetActive(!container.activeSelf);
        isOpen = container.activeSelf;
    }

}