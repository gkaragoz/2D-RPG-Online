using ShiftServer.Proto.RestModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionSlot : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Toggle _toggle;
    [SerializeField]
    private Image _imageClassHighlight;
    [SerializeField]
    private Image _imageClassIcon;
    [SerializeField]
    private Image _imageClassAnimation;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterName;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterLevel;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterExperience;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterHealth;
    [SerializeField]
    private TextMeshProUGUI _txtCharacterMana;

    public void Initialize(CharacterModel character) {
        _imageClassIcon.color = Color.white;
        _imageClassAnimation.color = Color.white;

        if (character.class_index == 0) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Warrior).ClassFrameColor;
            _imageClassIcon.sprite = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Warrior).ClassIcon;
        } else if (character.class_index == 1) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Archer).ClassFrameColor;
            _imageClassIcon.sprite = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Archer).ClassIcon;
        } else if (character.class_index == 2) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Mage).ClassFrameColor;
            _imageClassIcon.sprite = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Mage).ClassIcon;
        } else if (character.class_index == 3) {
            _imageClassHighlight.color = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Priest).ClassFrameColor;
            _imageClassIcon.sprite = CharacterClassVisualization.instance.GetVisualizationProperties(CharacterClassVisualization.Classes.Priest).ClassIcon;
        }

        _txtCharacterName.text = character.name;
        _txtCharacterLevel.text = character.level + " Lv.";
        _txtCharacterExperience.text = "Exp: " + character.exp;
        //_txtCharacterHealth.text = "HP: " + character.hp;
        //_txtCharacterMana.text = "MP: " + character.mp;
    }

    public void SetInteractable(bool interactable) {
        _toggle.interactable = interactable;
    }

}
