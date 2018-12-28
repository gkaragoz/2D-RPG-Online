const Entity = require("./entity");
const uuidv4 = require('uuid/v4');
const config = require('../../config/game');
const THREE = require('three');
exports = module.exports = Ship;

function Ship (shipListLen, player, shipConfig) {
    this.currentCrewMembersIds = [];
    this.shipName = player.Username+ "'s Ship";
    //using for making unique object
    Entity.call(this, player);
    this.assetName = shipConfig.assetName;
    this.captainUserId = player.id;
    this.captain = player;
    this.slopeSpeed = shipConfig.slopeSpeed;
    this.currentSuppliesCount = 0;
    this.currentSailorsCount = 0;
    this.marketPrice = shipConfig.marketPrice;
    this.currentHealth = shipConfig.maxHealth;
    this.rotationSpeed = shipConfig.rotationSpeed;
    this.movementSpeed = shipConfig.movementSpeed;
    this.absSpeed = shipConfig.absSpeed;
    this.pos_x = 0;
    this.pos_y = 0;
    this.pos_z = 0;
    this.isSail = true;
    this.sailors = [];
    this.inputs = [];
    this.sailors.push(player);
}

Ship.prototype = Object.create(Entity.prototype);

Ship.prototype.validateInput = function(dt_time) {
    if (Math.abs(dt_time) > 0.2) {
        return false;
    }
    return true;
};

Ship.prototype.moveForward = function (worldConfig) {
    //TODO:0.02 must be on game.yaml
    if(this.isSail)
    {
        //collision detect
        if(this.worldCollisionDetect(worldConfig))
        {
            this.pos_z -= (this.absSpeed * config.game.absolute_delta_time * this.movementSpeed / config.game.interval);

        }
        else
        {
            this.pos_z += (this.absSpeed * config.game.absolute_delta_time * this.movementSpeed / config.game.interval);
        }
    }
};
Ship.prototype.worldCollisionDetect = function (worldConfig) {
    return (this.pos_z >= worldConfig.worldUpZ ||
        this.pos_z <= worldConfig.worldDownZ ||
        this.pos_x >= worldConfig.worldRightX ||
        this.pos_x <= worldConfig.worldLeftX)
};


// Ship.prototype.moveTo = function (data) {
//     var dt_time = parseFloat(data.deltaTime);
//     if(this.validateInput(dt_time)){
//
//         var max_vector_poss = [0, -1, 1];
//         //if it has an index about given input. it return > -1 index number.
//         var x_is_valid = max_vector_poss.indexOf(data.vcX) > -1;
//         var y_is_valid = max_vector_poss.indexOf(data.vcZ) > -1;
//         var inputVc = new vector2(data.vcX, data.vcY);
//         var charVc = new vector2(this.pos_x, this.pos_y);
//
//         if(x_is_valid && y_is_valid)
//         {
//             if(inputVc.x !== 0 || inputVc.y !== 0)
//             {
//                 //TODO:0.02 must be on game.yam
//                 this.pos_x += (inputVc.x * dt_time * this.state.baseSpeed);
//                 this.pos_y += (inputVc.y * dt_time * this.state.baseSpeed);
//             }
//         }
//     }
// };