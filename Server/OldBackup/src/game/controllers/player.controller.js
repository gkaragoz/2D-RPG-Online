const entityController = require("./entity.controller");
const player = require("../entities/player");

exports = module.exports = PlayerController;

function PlayerController () {
    entityController.call(this);
}

PlayerController.prototype = Object.create(entityController.prototype);

PlayerController.prototype.add = function (socket) {
    let self = this;
    let newPlayer, playerFound = false;

    this.entities.some(function (player) {
        if (player.id === socket.id) {
            newPlayer = player;
            playerFound = true;
            return true;
        }
    });

    if (!playerFound) {
        newPlayer = new player(socket);
        this.entities.push(newPlayer);
    }
    return newPlayer;
};

PlayerController.prototype.remove = function (socket) {
    this.entities = this.entities.filter(function(item) {
        return item.id !== socket.id && item.secureId !== socket.secureId
    });
};
PlayerController.prototype.get = function (socket) {
    return this.entities.findIndex(x => x.id === socket.id && x.secureId === socket.secureId);
};

PlayerController.prototype.addInput = function (id, inputName, input, time) {
    this.entities.some(function (player) {
        if (player.id === id) {
            input["time"] = time;
            player.inputs.push(input);
            return true;
        }
    });
};