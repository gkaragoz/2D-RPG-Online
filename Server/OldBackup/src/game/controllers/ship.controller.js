const entityController = require("./entity.controller");
const Ship = require("../entities/ship");

exports = module.exports = ShipController;

function ShipController (worldConfig) {
    entityController.call(this);
    this.worldConfig = worldConfig;
}

ShipController.prototype = Object.create(entityController.prototype);
ShipController.prototype = {
    add: function (player, shipConfig) {
        let newShip = new Ship(this.entities.length, player, shipConfig);
        this.entities.push(newShip);
    },
    remove: function (socket) {
        this.entities = this.entities.filter(function(item) {
            return item.captainUserId !== socket.id
        });
    },
    get: function (socket) {
         let ship = this.entities.filter(function(item) {
            return item.captainUserId === socket.client.id
        })[0];
        return ship.id;
    }
};


ShipController.prototype.addInput = function (id, inputName, input, time) {
    this.entities.some(function (ship) {
        if (ship.id === id) {
            input["time"] = time;
            ship.inputs.push(input);
            return true;
        }
    });
};

//ONLY ON BROADCAST !!!!!!!!!!!!!!
ShipController.prototype.GetAllShips = function (Packet, builder) {
    let self = this;
    let shipList = [];
    self.entities.forEach(function (entity) {
        entity.moveForward(self.worldConfig);
        Packet.Models.Ship.startShip(builder);
        Packet.Models.Ship.addId(builder, entity.id);
        Packet.Models.Ship.addAssetType(builder, Packet.Models.ShipTypes[this.assetName]);
        Packet.Models.Ship.addCaptainUserId(builder, entity.captainUserId);
        Packet.Models.Ship.addPos(builder,
            Packet.Models.Vec3.createVec3(builder,
                entity.pos_x,
                entity.pos_y,
                entity.pos_z,
            ));
        Packet.Models.Ship.addViewAngle(builder, entity.viewAngle);
        Packet.Models.Ship.addCurrentSuppliesCount(builder, entity.currentSuppliesCount);
        Packet.Models.Ship.addCurrentSailorsCount(builder, entity.currentSailorsCount);
        Packet.Models.Ship.addCurrentHealth(builder, entity.currentHealth);
        Packet.Models.Ship.addSlopeSpeed(builder, entity.slopeSpeed);
        Packet.Models.Ship.addRotationSpeed(builder, entity.rotationSpeed);
        Packet.Models.Ship.addMovementSpeed(builder, entity.movementSpeed);
        let ship = Packet.Models.Ship.endShip(builder);
        shipList.push(ship);
    });
    return shipList;
};