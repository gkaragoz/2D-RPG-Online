const config = require('../config/game');
const THREE = require('three');
const sqlite3 = require('sqlite3').verbose();
let db = new sqlite3.Database(':memory:');

const socketioJwt = require("socketio-jwt");
const { JWT_SECRET } = require('../config/vars');

const YAML = require('yamljs');

const SupplyController = require("./controllers/supply.controller");
const PlayerController = require("./controllers/player.controller");
const ShipController = require("./controllers/ship.controller");
const EntityController = require("./controllers/entity.controller");

const flatbuffers = require('flatbuffers').flatbuffers;
const Packet = require("./fbs/message_generated").SailorIO;
const ClientInputPacket = require("./fbs/clientinput_generated").SailorIO;

exports = module.exports = ServerCore;

function ServerCore(socket, uws) {
    this.options = config;
    this.uws = uws;
    this.uws.binaryType = 'arraybuffer';
    this.uws.options = { binary: true };
    this.io = socket;
    this.lastUpdateTime = 0;
    this.updatePassTime = null;
    this.worldConfig = YAML.load(__base + "config/gamesettings/world.yaml");
    this.supplyConfig = YAML.load(__base + "config/gamesettings/supplies.yaml");
    this.shipConfig = YAML.load(__base + "config/gamesettings/ships.yaml");
    this.islandConfig = YAML.load(__base + "config/gamesettings/island.yaml");
    this.userSlots = range(this.worldConfig.maxUser);
    this.mainIsland = null;
    this.takenSlots = [];
    this.secureIdList = [];
    this.worldConfig.worldLeftX = this.worldConfig.offSetX - (this.worldConfig.width / 2);
    this.worldConfig.worldUpZ = (this.worldConfig.length / 2) - this.worldConfig.offSetZ;
    this.worldConfig.worldRightX = (this.worldConfig.width / 2) - this.worldConfig.offSetX;
    this.worldConfig.worldDownZ = this.worldConfig.offSetZ - (this.worldConfig.length / 2);
    this.supplyController = new SupplyController();
    this.playerController = new PlayerController();
    this.shipController = new ShipController(this.worldConfig);
    this.entityController = new EntityController();
}

ServerCore.prototype.broadcastState = function () {
    let builder = new flatbuffers.Builder(0);

    let serverTime = new Date().getTime() - this.startTime;
    this.updatePassTime = serverTime - this.lastUpdateTime;
    this.lastUpdateTime = serverTime;
    //let updateTime =  builder.createString((new Date().getTime() / 1000.00000);
    let updateTime =  new Date().getTime() / 1000.00000;
    let supplyList = Packet.Models.UpdateModel.createSupplyCratesVector(builder, this.supplyController.GetSupplies(Packet, builder));
    let shipModels = Packet.Models.UpdateModel.createShipModelsVector(builder, this.shipController.GetAllShips(Packet, builder));
    Packet.Models.UpdateModel.startUpdateModel(builder);
    Packet.Models.UpdateModel.addUpdatePassTime(builder, this.updatePassTime);
    Packet.Models.UpdateModel.addSupplyCrates(builder, supplyList);
    Packet.Models.UpdateModel.addShipModels(builder, shipModels);
    Packet.Models.UpdateModel.addEventType(builder, Packet.Models.EventTypes.UpdateModel);
    Packet.Models.UpdateModel.addUpdateTime(builder, updateTime);
    let updateModel = Packet.Models.UpdateModel.endUpdateModel(builder);
    builder.finish(updateModel);
    this.supplyController.supplyItems = this.supplyController.supplyItems.filter(function(el) { return el.isDeath === false });
    let buffer = builder.asUint8Array();
    console.log("world state update buffer len: " + buffer.length);
    this.uws.broadcast(buffer, this.uws.options);
};

ServerCore.prototype.removePlayer = function (socket) {
    //this.io.emit('removePlayer', id);
    this.playerController.remove(socket);
};
ServerCore.prototype.removeShip = function (socket) {
    //this.io.emit('removePlayer', id);
    this.shipController.remove(socket);
};
ServerCore.prototype.getPlayerShipId = function (socket) {
    return this.shipController.get(socket);
};
ServerCore.prototype.getPlayerEntity = function (socket) {
    return this.playerController.get(socket);
};
ServerCore.prototype.addPlayer = function (socket) {
    this.playerController.add(socket);
};

ServerCore.prototype.createIsland = function () {
    let geo = new THREE.Geometry();
    let startingPoint = new THREE.Vector3(0, 1, 0);
    geo.vertices.push(startingPoint);
    for (i = 0; i < this.islandConfig.length; i++) {
        let v1 = new THREE.Vector3(this.islandConfig[i].x, this.islandConfig[i].y, this.islandConfig[i].z);
        geo.vertices.push(v1);
        geo.faces.push( new THREE.Face3( i, i + 1) );
    }

    this.mainIsland = new THREE.Mesh( geo, new THREE.MeshNormalMaterial() );
    console.log(this.mainIsland);
};
ServerCore.prototype.addShip = function (selectedShip, socket) {
    let currentPlayer, playerFound;

    if (selectedShip !== undefined)
    {
        this.playerController.entities.some(function (player, i) {
            if (player.id === socket.id) {
                currentPlayer = player;
                player.hasShip = true;
                player.shipCount++;
                playerFound = true;
                return true;
            }
        });
        if (!currentPlayer.onShip && currentPlayer.hasShip)
        {
            this.shipController.add(currentPlayer, selectedShip);
            console.log("New ship created for: " + socket.id);
        }

    }
};

ServerCore.prototype.feedShip = function (feedModel) {
    let supplyIndex = this.supplyController.supplyItems.findIndex(x => x.supplyId === feedModel.supplyId);
    let shipIndex = this.shipController.entities.findIndex(x => x.id ===  feedModel.shipId);
    if (supplyIndex > -1 && shipIndex > -1 )
    {
        let supplyVc = new THREE.Vector2(
            this.supplyController.supplyItems[supplyIndex].pos_x,
            this.supplyController.supplyItems[supplyIndex].pos_z );

        let shipVc = new THREE.Vector2(
            this.shipController.entities[shipIndex].pos_x,
            this.shipController.entities[shipIndex].pos_z );

        let distance = supplyVc.distanceTo( shipVc );
        if (distance < 10) {
            console.log("Ship eating the supply ! > shipid: "+ this.shipController.entities[shipIndex].id);
            // ship.sailors.forEach(function (sailor) {
            //     //TODO: ADD INCOME TO SAILORS
            // })
            this.supplyController.supplyItems[supplyIndex].isDeath = true;
        }
    }

};

ServerCore.prototype.messageHandler = function (ws, message) {
    let buffer = Buffer.from(message);
    let buf = new flatbuffers.ByteBuffer(buffer);
    let userInput = ClientInputPacket.ClientInputModel.ClientInput.getRootAsClientInput(buf);
    let eventType = userInput.EventType();

    if (eventType === ClientInputPacket.ClientInputModel.ClientEventTypes.GetWorldInfo ) {
        console.log("[GetWorldInfo] EVENT TRIGGERED");
        let builder = new flatbuffers.Builder(0);
        Packet.Models.WorldInfoTable.startWorldInfoTable(builder);
        Packet.Models.WorldInfoTable.addUserSlotId(builder, ws.id);
        Packet.Models.WorldInfoTable.addHeight(builder, this.worldConfig.height);
        Packet.Models.WorldInfoTable.addWidth(builder, this.worldConfig.width);
        Packet.Models.WorldInfoTable.addLength(builder, this.worldConfig.length);
        Packet.Models.WorldInfoTable.addOffSetX(builder, this.worldConfig.offSetX);
        Packet.Models.WorldInfoTable.addOffSetY(builder, this.worldConfig.offSetY);
        Packet.Models.WorldInfoTable.addOffSetZ(builder, this.worldConfig.offSetZ);
        let worldInfo = Packet.Models.WorldInfoTable.endWorldInfoTable(builder);
        let supplyList = Packet.Models.UpdateModel.createSupplyCratesVector(builder, this.supplyController.GetAllSupplies(Packet, builder));
        Packet.Models.UpdateModel.startUpdateModel(builder);
        Packet.Models.UpdateModel.addSupplyCrates(builder, supplyList);
        Packet.Models.UpdateModel.addEventType(builder, Packet.Models.EventTypes.WorldInfoUpdate);
        Packet.Models.UpdateModel.addWorldInfo(builder, worldInfo);

        let updateModel = Packet.Models.UpdateModel.endUpdateModel(builder);
        builder.finish(updateModel);

        let buffer = builder.asUint8Array();

        console.log("World INFO LEN: "+ buffer.length);
        ws.send(buffer ,this.uws.options)
    }
    else if ( eventType === ClientInputPacket.ClientInputModel.ClientEventTypes.NewPlayer )
    {
        console.log("[NewPlayer] EVENT TRIGGERED");
        this.addPlayer(ws);

        let builder = new flatbuffers.Builder(0);
        Packet.Models.UpdateModel.startUpdateModel(builder);
        Packet.Models.UpdateModel.addEventType(builder, Packet.Models.EventTypes.NewPlayer);
        let updateModel = Packet.Models.UpdateModel.endUpdateModel(builder);
        builder.finish(updateModel);
        let buffer = builder.asUint8Array();
        db.run();
        abc.run().run();
        abc.abc2().run();
        ws.send(buffer ,this.uws.options)

    }
    else if ( eventType === ClientInputPacket.ClientInputModel.ClientEventTypes.BuyNewShip ) {
        console.log("[BuyNewShip] EVENT TRIGGERED");
        let shipType = userInput.Event(new ClientInputPacket.ClientInputModel.BuyNewShip()).ship();
        if (shipType === ClientInputPacket.ClientInputModel.ShipTypes.RAFT1)
        {
            let shipData = this.shipConfig.RAFT1;
            let currentPlayerIndex = this.playerController.get(ws);
            if ( currentPlayerIndex !==  -1 )
            {
                if (this.playerController.entities[currentPlayerIndex].gold >= shipData.marketPrice)
                {
                    //SELL APPROVED
                    this.playerController.entities[currentPlayerIndex].gold -= shipData.marketPrice;
                    if (this.playerController.entities[currentPlayerIndex].shipCount === 0)
                    {
                        this.addShip(shipData, ws);
                        let builder = new flatbuffers.Builder(0);
                        Packet.Models.UpdateModel.startUpdateModel(builder);
                        Packet.Models.UpdateModel.addEventType(builder, Packet.Models.EventTypes.BuyNewShip);
                        let updateModel = Packet.Models.UpdateModel.endUpdateModel(builder);
                        builder.finish(updateModel);
                        let buffer = builder.asUint8Array();

                        ws.send(buffer ,this.uws.options)
                    }
                }
            }
        }
        else
        {
            //there is no raft with given type
        }
    }
    else if ( eventType === ClientInputPacket.ClientInputModel.ClientEventTypes.FeedShip ) {
        let shipId = userInput.Event(new ClientInputPacket.ClientInputModel.FeedShip()).shipId();
        let supplyPosX = userInput.Event(new ClientInputPacket.ClientInputModel.FeedShip()).supplyPos().x();
        let supplyPosY = userInput.Event(new ClientInputPacket.ClientInputModel.FeedShip()).supplyPos().y();
        let supplyPosZ = userInput.Event(new ClientInputPacket.ClientInputModel.FeedShip()).supplyPos().z();
        let supplyId = supplyPosX.toFixed(4)+supplyPosY+supplyPosZ.toFixed(4);

        let shipEntityIdx = this.shipController.entities.findIndex(x => x.id === shipId && x.secureId === ws.secureId);
        let supplyEntityIdx = this.supplyController.supplyItems.findIndex(x => x.supplyId === supplyId);
        if (shipEntityIdx > -1 && supplyEntityIdx > -1)
        {
            let supplyVc = new THREE.Vector2(
                this.supplyController.supplyItems[supplyEntityIdx].pos_x,
                this.supplyController.supplyItems[supplyEntityIdx].pos_z );

            let shipVc = new THREE.Vector2(
                this.shipController.entities[shipEntityIdx].pos_x,
                this.shipController.entities[shipEntityIdx].pos_z );

            let distance = supplyVc.distanceTo( shipVc );
            if (distance < 10) {
                // ship.sailors.forEach(function (sailor) {
                //     //TODO: ADD INCOME TO SAILORS
                // })
                this.supplyController.supplyItems[supplyEntityIdx].isDeath = true;
            }
        }
        console.log("[FeedShip] EVENT TRIGGERED");

    }
    else {
        //TODO: Posible client side error.
        ws.close();
    }

};
ServerCore.prototype.getUniquePlayerId = function (wsSecureId) {
    let randomNum = Math.floor(Math.random()*this.userSlots.length);
    let slotId = this.userSlots[randomNum];
    this.userSlots.splice(randomNum, 1);

    this.takenSlots.push({slotId: slotId, secureId: wsSecureId});
    return slotId;
};
ServerCore.prototype.clientCloseHandler = function (ws) {
    let takenTokenIndex = this.takenSlots.findIndex(x => x.slotId === ws.id && x.secureId === ws.secureId);

    if (takenTokenIndex > -1)
    {
        this.removePlayer(ws);
        this.removeShip(ws);
        let takenSlot = this.takenSlots[takenTokenIndex];
        this.userSlots.push(takenSlot.slotId);
        this.takenSlots.splice(takenTokenIndex, 1);

        let builder = new flatbuffers.Builder(0);
        Packet.Models.RemovePlayerInfoTable.startRemovePlayerInfoTable(builder);
        Packet.Models.RemovePlayerInfoTable.addUserSlotId(builder, takenSlot.slotId);
        let removePlayerInfo = Packet.Models.RemovePlayerInfoTable.endRemovePlayerInfoTable(builder);
        Packet.Models.UpdateModel.startUpdateModel(builder);
        Packet.Models.UpdateModel.addEventType(builder, Packet.Models.EventTypes.RemovePlayer);
        Packet.Models.UpdateModel.addRemovePlayerInfo(builder, removePlayerInfo);
        let updateModel = Packet.Models.UpdateModel.endUpdateModel(builder);
        builder.finish(updateModel);
        let buffer = builder.asUint8Array();

        this.uws.broadcast(buffer, this.uws.options);
        console.log("[CLIENT DISCONNECTED] > " + ws.id)
    }
};
ServerCore.prototype.startUws = function () {
    this.startTime = new Date().getTime();
    this.createIsland();

    function noop() {}

    function heartbeat() {
        this.isAlive = true;
    }

    let self = this;

    function onMessage(message) {
        self.messageHandler(this, message);
    }
   function onClose() {
        self.clientCloseHandler(this);
    }

    self.uws.on('connection', function(ws) {
        //TODO: CHECK JWT INPUT - IF NOT AUTHORIZED>> ws.terminate();


        if (self.takenSlots.length >= 500)
        {
            ws.terminate();
        }
        ws.secureId = ws.upgradeReq.headers["sec-websocket-key"];
        ws.id = self.getUniquePlayerId(ws.secureId);
        ws.isAlive = true;
        console.log('['+ws.upgradeReq.headers['x-forwarded-for']+'] '+'PLAYER CONNECTED > '+ ws.id);

        ws.on('pong', heartbeat);
        ws.on('message', onMessage);
        ws.on('close', onClose);
    });

    self.stateIntervalId = setInterval(function () { self.broadcastState(); }, 1000 / this.options.game.interval);
    self.supplyIntervalId = setInterval(function () { self.supplyController.spawnOneSupplyWithInterval(self.worldConfig, self.supplyConfig);}, 1000 / self.worldConfig.supplyRespawnSec);
    self.pingInterval = setInterval(function ping() {
        self.uws.clients.forEach(function each(ws) {
            if (ws.isAlive === false) return ws.close();

            ws.isAlive = false;
            ws.ping(noop);
        });
    }, 30000);
};

ServerCore.prototype.start = function () {
    this.startTime = new Date().getTime();
    let self = this;
    self.io.on('connection', function(socket){
        console.log("connection has been made");

        socket.on('getWorldInfo', function () {
            console.log("user wants to know world info > "+socket.client.id);
            socket.emit('getWorldInfo', self.worldConfig);
        });

        socket.on('newShip', function(data){
            console.log("new ship request from: " + socket.client.id);
            //data.playerId
            let selectedShip = self.shipConfig[data.shipType];
            self.addShip(selectedShip, socket)
        });
        socket.on('summonShip', function(data){
            //data.playerId
            //TODO: Get user's ship from database if needed.
        });
        socket.on('playerNew', function(){
            console.log("new player request from: " + socket.client.id);
            //data.playerId
            self.addPlayer(socket);
            console.log('new user created id: ' + socket.client.id);
        });
        socket.on('playerMove', function(data){
            let inputtime = new Date().getTime();
            //self.addPlayerInput(socket.client.id, 'playerMove', data, inputtime);
        });
        socket.on('feedShip', function(data){
            let inputtime = new Date().getTime();
            self.feedShip(data)
            //self.addPlayerInput(socket.client.id, 'playerMove', data, inputtime);
        });
        socket.on('disconnect', function(){
            let dcStatusModel = {userId: socket.client.id, shipId: self.getPlayerShipId(socket)};
            self.removePlayer(socket);
            self.removeShip(socket);
            self.broadcastUserStatus(dcStatusModel);
            console.log('user+ship deleted');
        });


    });

    //update per second
    self.supplyIntervalId = setInterval(function () { self.supplyController.spawnOneSupplyWithInterval(self.worldConfig, self.supplyConfig);}, 1000 / self.worldConfig.supplyRespawnSec);

};

function range(max) {
    let numList = [];

    for (let i = 0; i < max; i++)
    {
        numList.push(i);
    }

    return numList;
}

