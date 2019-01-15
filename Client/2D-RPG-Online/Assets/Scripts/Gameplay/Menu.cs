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

    public virtual void Show(bool sub = false) {
        container.SetActive(true);
        isOpen = true;

        if (sub) {
            ShowSub();
        }
    }

    public virtual void Hide(bool sub = false) {
        container.SetActive(false);
        isOpen = false;

        if (sub) {
            HideSub();
        }
    }

    public virtual void ShowSub() {
        subContainer.SetActive(true);
    }

    public virtual void HideSub() {
        subContainer.SetActive(false);
    }

    public virtual void Toggle() {
        container.SetActive(!container.activeSelf);
        isOpen = container.activeSelf;
    }

}