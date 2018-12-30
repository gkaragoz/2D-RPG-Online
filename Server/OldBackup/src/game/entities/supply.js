exports = module.exports = Supply;
const uuidv4 = require('uuid/v4');
const config = require('../../config/game');
const Chance = require('chance');
const chance = new Chance();

function Supply (supplyListLength, supplyData, worldConfig) {
    this.pos_x = 0;
    this.pos_y = 0;
    this.pos_z = 0;
    this.supplyConfigData = supplyData;
    this.supplyName = supplyData.name;
    this.supplyIncome = supplyData.income;
    this.supplyId = "";
    this.isDeath = false;
    this.isNew = true;
    this.looterShipId = "";
    this.assetName = supplyData.assetName;
    this.SetRandomPosition(worldConfig);
}

Supply.prototype.SetRandomPosition = function(worldConfig) {
    let self = this;
    self.pos_x = chance.floating({min: worldConfig.worldLeftX, max:  worldConfig.worldRightX});
    self.pos_z = chance.floating({min: worldConfig.worldDownZ, max: worldConfig.worldUpZ});
    self.pos_y = worldConfig.height;
    self.supplyId = self.pos_x.toString()+self.pos_y.toString()+self.pos_z.toString();
};

