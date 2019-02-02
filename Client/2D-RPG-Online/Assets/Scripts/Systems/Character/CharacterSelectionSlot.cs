using ManaShiftServer.Data.RestModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionSlot : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Toggle _toggle;
    [SerializeField]
    private Image _imageClassIcon;
    [SerializeField]
    private Image _imageClassAnimation;
    [SerializeField]
    private Image _imageExpFill;
    [SerializeField]
    private Image _imageExpBackground;
    [SerializeField]
    private Image _imageHealthBackground;
    [SerializeField]
    private Image _imageManaBackground;
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
    [SerializeField]
    private Color _expFillColor;
    [SerializeField]
    private Color _expBackgroundColor;
    [SerializeField]
    private Color _healthBgColor;
    [SerializeField]
    private Color _manaBgColor;
    [SerializeField]
    private Button _btnCreateCharacter;

    public void Initialize(CharacterModel character) {
        _imageClassIcon.color = Color.white;
        _imageClassAnimation.color = Color.white;
        _txtCharacterLevel.color = Color.white;
        _txtCharacterExperience.color = Color.white;
        _txtCharacterHealth.color = Color.white;
        _txtCharacterMana.color = Color.white;
        _imageExpFill.color = _expFillColor;
        _imageExpBackground.color = _expBackgroundColor;
        _imageExpFill.fillAmount = character.exp;
        _imageHealthBackground.color = _healthBgColor;
        _imageManaBackground.color = _manaBgColor;
        _btnCreateCharacter.gameObject.SetActive(false);

        _txtCharacterName.text = character.name;
        _txtCharacterLevel.text = character.level + " Lv.";
        _txtCharacterExperience.text = "Exp: " + character.exp;
        _txtCharacterHealth.text = "HP: " + character.stat.health;
        _txtCharacterMana.text = "MP: " + character.stat.mana;
    }

    public void SetInteractable(bool interactable) {
        _toggle.interactable = interactable;
    }

}
