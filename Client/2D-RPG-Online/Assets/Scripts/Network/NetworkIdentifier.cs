using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkIdentifier {

    public Action OnPlayerDataChanged;

    public class PositionEntry {
        public Vector3 vector3;
        public double updateTime;
        public int inputSequenceID;

        public PositionEntry(double updateTime, Vector3 vector3, int inputSequenceID) {
            this.updateTime = updateTime;
            this.vector3 = vector3;
            this.inputSequenceID = inputSequenceID;
        }
    }

    public PlayerObject PlayerObject {
        get { return _playerData; }
        set {
            _playerData = value;
            OnPlayerDataChanged?.Invoke();
        }
    }

    public string PlayerName { get { return _playerData.Name; } }

    public List<SPlayerInput> PlayerInputs { get { return _playerInputs; } }

    public int Oid { get { return _playerData.Oid; } }

    public List<PositionEntry> PositionBuffer { get { return _positionBuffer; } set { _positionBuffer = value; } }

    public int LastProcessedInputSequenceID { get { return _lastProcessedInputSequenceID; } set { _lastProcessedInputSequenceID = value; } }

    private PlayerObject _playerData;
    private List<PositionEntry> _positionBuffer = new List<PositionEntry>();
    private List<SPlayerInput> _playerInputs = new List<SPlayerInput>();
    private int _lastProcessedInputSequenceID;
    private int _nonAckInputIndex = 0;

    public NetworkIdentifier(PlayerObject playerData) {
        this._playerData = playerData;
    }

    public void SendInputData(Vector3 input) {
        ShiftServerData data = new ShiftServerData();

        data.PlayerInput = new SPlayerInput();
        data.PlayerInput.SequenceID = _nonAckInputIndex++;

        data.PlayerInput.PosX = input.x;
        data.PlayerInput.PosZ = input.z;
        data.PlayerInput.PressTime = Time.fixedDeltaTime;

        NetworkManager.mss.SendMessage(MSPlayerEvent.Move, data);

        PlayerInputs.Add(data.PlayerInput);
    }

    public void AddPositionToBuffer(double timestamp, Vector3 position, int inputSequenceID) {
        _positionBuffer.Add(new PositionEntry(timestamp, position, inputSequenceID));
    }

    public Vector2 GetVectorByInput(int index) {
        return new Vector2(PlayerInputs[index].PosX, PlayerInputs[index].PosY);
    }

    public void ClearPlayerInputs() {
        _playerInputs = new List<SPlayerInput>();
    }

    public void RemoveRange(int index, int count) {
        _playerInputs.RemoveRange(index, count);
    }

    public int GetSequenceID(int index) {
        return PlayerInputs[index].SequenceID;
    }

}
