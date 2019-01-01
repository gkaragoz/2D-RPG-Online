# ⚡  Server Events

::: warning
This features are key events to communicate with server
:::


enum Object > MSServerEvent

## .WorldUpdate

This event must be only `listened` by client

#### Server Response
RepeatedFiled == List. Foreach and update all game objects with server's game objects
```csharp
    public RepeatedField<sGameObject> UpdatedGameObjects { get; }
```
sGameObject
```csharp
    public string Guid { get; set; }
    public float PosX { get; set; }
    public float PosZ { get; set; }
    public float PosY { get; set; }
```
## .PingRequest

This event is `key`. all clients must register this event without any excuse.
Also, the client must ping every second to approve the connection health.
you can pass null data object. there is no need to construct any sub object inside this event.

## .ConnectOK

This event must be only `listened` by client to approve connect() method from server.


Server Response just have this event id. other sub objects are null.
#### Server Response

an example to this event listener
```csharp
    public void OnConnected(ShiftServerData data)
    {
        StartCoroutine(SendPing());
        Debug.Log("Connected To Server");
    }
```


## .JoinRequest

This event must be fired for entering server world

#### Client Request

- `ClientData.Loginname`  is username of the approved client user
- `ClientData.Guid` new Guid
- `ClientData.MachineId` SystemInfo.deviceUniqueIdentifier
- `ClientData.MachineName` SystemInfo.deviceName

```csharp
clientInfo = new ClientData();
clientInfo.Guid = Guid.NewGuid().ToString();
clientInfo.Loginname = "Test"; // get from client user
clientInfo.MachineId = SystemInfo.deviceUniqueIdentifier;
clientInfo.MachineName = SystemInfo.deviceName;
```

```csharp
ShiftServerData data = new ShiftServerData();
data.ClData = this.clientInfo;
NetworkManager.client.SendMessage(MSServerEvent.JoinRequest, data);
```
## .JoinRequestSuccess

This event must be only `listened` by client to approve `.JoinRequest` event from client.

#### Server Response
Server will respond with `Session` object, you need to set this session object ``IN ALL SEND EVENTS``

```csharp
ShiftServerData.Session
```
Session object has one property that has session id (string)
```csharp
ShiftServerData.Session.sid
```

## .JoinRequestFailed

This event must be only `listened` by client to approve `.JoinRequest` event from client.

#### Server Response

```csharp
ShiftServerData errorData = new ShiftServerData();
errorData.ErrorReason = ShiftServerError.[ERROR TYPE];
```
```csharp

public enum ShiftServerError {

  OldClientVersion = 0,
  NoSession = 1,
  SocketAbuse = 2,
  WrongCredentials = 3,
}

```

## .Diagnostic

Server log download event

#### Server Response

```csharp
in development
```