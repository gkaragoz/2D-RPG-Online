const entity = require("./entity");

exports = module.exports = Player;

function Player (socket) {
    entity.call(this, socket);
    this.socket = socket;
    this.Username = "Default";
    this.gold = 150;
    this.shipCount = 0;
    this.onShip = false;
    this.hasShip = false;
    this.inputs = [];
}

Player.prototype = Object.create(entity.prototype);

Player.prototype.validateInput = function(dt_time) {
    if (Math.abs(dt_time) > 0.2) {
        return false;
    }
    return true;
};

Player.prototype.add = function() {

};

Player.prototype.moveTo = function (data) {
    var dt_time = parseFloat(data.deltaTime);
    if(this.validateInput(dt_time)){

        var max_vector_poss = [0, -1, 1];
        //if it has an index about given input. it return > -1 index number.
        var x_is_valid = max_vector_poss.indexOf(data.vcX) > -1;
        var y_is_valid = max_vector_poss.indexOf(data.vcZ) > -1;
        var inputVc = new vector2(data.vcX, data.vcY);
        var charVc = new vector2(this.pos_x, this.pos_y);

        if(x_is_valid && y_is_valid)
        {
            if(inputVc.x !== 0 || inputVc.y !== 0)
            {
                //TODO:0.02 must be on game.yam
                this.pos_x += (inputVc.x * dt_time * this.state.baseSpeed);
                this.pos_y += (inputVc.y * dt_time * this.state.baseSpeed);
            }
        }
    }

};