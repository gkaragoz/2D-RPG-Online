using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class is reponsible to open and close any panels.
/// </summary>
public abstract class Menu : MonoBehaviour {

    /// <summary>
    /// If this class is derived from a Manager class, that class is going to need a container object to open close the this panel.
    /// </summary>
    [Header("Initialization")]
    public GameObject container;

    /// <summary>
    /// A status for this panel is opened or not.
    /// </summary>
    public bool isOpen { get; set; }

    /// <summary>
    /// If container doesn't set as any object, than container is that panel itself.
    /// </summary>
    private void Start() {
        if (container == null) {
            container = this.gameObject;
        }
    }

    /// <summary>
    /// Show the panel.
    /// </summary>
    public virtual void Show() {
        container.SetActive(true);
        isOpen = true;
    }

    /// <summary>
    /// Hide the panel.
    /// </summary>
    public virtual void Hide() {
        container.SetActive(false);
        isOpen = false;
    }

    /// <summary>
    /// Toggle the panel. Basically if it's opened, than it's going to be closed.
    /// </summary>
    public virtual void Toggle() {
        container.SetActive(!container.activeSelf);
        isOpen = container.activeSelf;
    }

}