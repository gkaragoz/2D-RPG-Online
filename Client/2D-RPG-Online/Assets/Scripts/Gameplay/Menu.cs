using UnityEngine;

public abstract class Menu : MonoBehaviour {

    [Header("Initialization")]
    public GameObject container;
    public GameObject subContainer;

    public bool isOpen { get; set; }

    private void Awake() {
        if (container == null) {
            container = this.gameObject;
        }
    }

    public virtual void Show() {
        container.SetActive(true);
        isOpen = true;

        if (subContainer != null) {
            ShowSub();
        }
    }

    public virtual void Hide() {
        container.SetActive(false);
        isOpen = false;

        if (subContainer != null) {
            HideSub();
        }
    }

    public virtual void Toggle() {
        container.SetActive(!container.activeSelf);
        isOpen = container.activeSelf;
    }

    private void ShowSub() {
        subContainer.SetActive(true);
    }

    private void HideSub() {
        subContainer.SetActive(false);
    }

}