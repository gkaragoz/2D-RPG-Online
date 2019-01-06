using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FriendRow : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private TextMeshProUGUI _txtUsername;
    [SerializeField]
    private TextMeshProUGUI _txtUserStatus;
    [SerializeField]
    private Image _imgClassIcon;

    [SerializeField]
    private Button _btnSendMessage;
    [SerializeField]
    private Button _btnDeleteFriend;

    [Header("Settings")]
    [SerializeField]
    private Color _onlineUIColor;
    [SerializeField]
    private Color _offlineUIColor;

    public bool IsOnline { get; set; }

    public void UpdateUI() {

    }

    public void SetSendMessageButtonOnClickAction(FriendManager.SendMessageDelegate sendMessageDelegate) {
        _btnSendMessage.onClick.AddListener(() => sendMessageDelegate());
    }

    public void SetDeleteFriendButtonOnClickAction(FriendManager.DeleteFriendDelegate deleteFriendDelegate) {
        _btnDeleteFriend.onClick.AddListener(() => deleteFriendDelegate());
    }

    public void SetOnline() {
        _txtUsername.color = _onlineUIColor;
        _txtUserStatus.color = _onlineUIColor;

        _imgClassIcon.color = Color.white;
        _btnSendMessage.interactable = true;
        _btnDeleteFriend.interactable = true;
    }

    public void SetOffline() {
        _txtUsername.color = _offlineUIColor;
        _txtUserStatus.color = _offlineUIColor;

        _imgClassIcon.color = _offlineUIColor;
        _btnSendMessage.interactable = false;
        _btnDeleteFriend.interactable = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FriendRow))]
    public class ExampleScriptEditor : Editor {
        public FriendRow friendRow;

        public void OnEnable() {
            friendRow = (FriendRow)target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            bool is_interactive_target = friendRow.IsOnline;
            GUI.backgroundColor = (is_interactive_target) ? Color.green : Color.red;
            GUILayout.Space(10f);
            GUILayout.Label("UI Representation");
            if (GUILayout.Button("IsOnline: " + friendRow.IsOnline + " (Click to make " + !is_interactive_target + ")")) {
                friendRow.IsOnline = !is_interactive_target;

                if (friendRow.IsOnline) {
                    friendRow.SetOnline();
                } else {
                    friendRow.SetOffline();
                }
            }
            GUI.backgroundColor = Color.white;
            SceneView.RepaintAll();
        }
    }

#endif

}
