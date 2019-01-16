using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    
    public bool HasInput {
        get {
            return (CurrentInput != Vector2.zero) ? true : false;
        }
    }

    public Vector2 CurrentInput { get; private set; }
    public Vector2 LastInput { get; private set; }

    [SerializeField]
    [Utils.ReadOnly]
    private float _xInput, _yInput;
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private Button _btnAttack;
    [SerializeField]
    private GameObject _shadowObject;

    private CharacterController _characterController;
    private PlayerHUD _playerHUD;
    private RoomPlayerInfo _playerInfo;

    public List<SPlayerInput> playerInputs = new List<SPlayerInput>();
    private int _nonAckInputIndex = 0;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _playerHUD = GetComponent<PlayerHUD>();
    }

    private void FixedUpdate() {
        LastInput = CurrentInput;

        _xInput = _joystick.Horizontal;
        _yInput = _joystick.Vertical;

        CurrentInput = new Vector2(_xInput, _yInput);

        if (HasInput && NetworkManager.mss != null) {
            ShiftServerData data = new ShiftServerData();

            data.PlayerInput = new SPlayerInput();
            data.PlayerInput.SequenceID = _nonAckInputIndex++;

            data.PlayerInput.PosX = CurrentInput.x;
            data.PlayerInput.PosY = CurrentInput.y;

            NetworkManager.mss.SendMessage(MSPlayerEvent.Move, data);

                Move();
                playerInputs.Add(data.PlayerInput);
          
        }
        if (!HasInput) {
            Stop();
        }
        //Debug.Log("NON-Ack Player Inputs: " + playerInputs.Count);

    }

    private void Update() {
        if (Input.GetButton("Fire1")) {
            Attack();
        }
    }

    public void Initialize(RoomPlayerInfo playerInfo) {
        this._playerInfo = playerInfo;

        _playerHUD.SetName(this._playerInfo.Username);
    }

    public void SetShadowPosition(Vector2 position) {
        _shadowObject.transform.position = position;

        if (_shadowObject.transform.position == this.transform.position) {
            _shadowObject.gameObject.SetActive(false);
        } else {
            _shadowObject.gameObject.SetActive(true);
        }
    }

    public Vector2 GetVectorByInput(int index) {
        return new Vector2(playerInputs[index].PosX, playerInputs[index].PosY);
    }

    public void ClearPlayerInputs() {
        playerInputs = new List<SPlayerInput>();
    }

    public void RemoveRange(int index, int count) {
        playerInputs.RemoveRange(index, count);
    }

    public int GetSequenceID(int index) {
        return playerInputs[index].SequenceID;
    }

    public void SetJoystick(FixedJoystick joystick) {
        this._joystick = joystick;
    }

    public void Attack() {
        _characterController.Attack();
    }

    public void Move() {
        _characterController.Move(CurrentInput);
    }

    public void Move(Vector2 input) {
        _characterController.Move(input);
    }

    public void Stop() {
        _characterController.Stop();
    }

}
