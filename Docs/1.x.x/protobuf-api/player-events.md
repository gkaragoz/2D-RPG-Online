# 🔥 Player Events

::: warning
This features requires authorized client (session data must be valid).
Session data returns from JoinRequestSuccess server event
:::


enum Object > MSPlayerEvent
## .OnCreatePlayer

This event can be fired for creating player object to server world.

for now, you can pass same objects with MSServerEvent.JoinRequest

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

#### Server Response
```csharp
 in development
```
## .OnMove

This event can be fired before character move animation

Events like "Move", "Attack" cant have response. Server will register the player inputs and will return
in world update event (server)

#### Client Request

```csharp
ShiftServerData.Interaction.CurrentObject = new sGameObject();
ShiftServerData.Interaction.CurrentObject.PosX = 1;
ShiftServerData.Interaction.CurrentObject.PosY = 0;
ShiftServerData.Interaction.CurrentObject.PosZ = 0;
ShiftServerData.Interaction.CurrentObject.Guid = GameObject.Guid;
```
server validates object guid with socket client id

## .OnAttack

This event can be fired before character attack animation

#### Client Request
```csharp
on development
```

## .OnDead

When the player object state dead in client, the client must validate dead state with sending OnDead request to server

#### Client Request

```csharp
on development

```

## .OnUse

This event can be fired before character use event (any item use etc.)

#### Client Request


```csharp
on development

```