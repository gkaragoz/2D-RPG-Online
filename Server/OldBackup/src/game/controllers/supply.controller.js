const Supply = require("../entities/supply");
const entityController = require("./entity.controller");

exports = module.exports = SupplyController;

function SupplyController () {
    entityController.call(this);
    this.lastSupplySpawnTime = null;
    this.supplyEntities = [];
    this.supplyItems = [];
}

SupplyController.prototype.spawnOneSupply = function (worldConfig, supplyConfig) {
    let self = this;
    let nowDt = new Date().getTime();
    if (nowDt - self.lastSupplySpawnTime > 1000.000 * worldConfig.supplyRespawnSec)
    {
        let i = getRandomInt(0, supplyConfig.length - 1 );
        let supplyItem = supplyConfig[i];
        if (self.supplyItems.length < supplyItem.maxSupplyCount)  {
            let newSupplyCrate = new Supply(self.supplyItems.length ,supplyItem, worldConfig);
            self.supplyItems.push(newSupplyCrate);
            self.lastSupplySpawnTime = new Date().getTime();
            return null;
        }
    }
};

SupplyController.prototype.spawnOneSupplyWithInterval = function (worldConfig, supplyConfig) {
    let self = this;
        let i = getRandomInt(0, supplyConfig.length - 1 );
        let supplyItem = supplyConfig[i];
        if (self.supplyItems.length < supplyItem.maxSupplyCount)  {
            let newSupplyCrate = new Supply(self.supplyItems.length, supplyItem, worldConfig);
            console.log("CRATE SPAWNED --> :"+newSupplyCrate.supplyName + " // len: "+ self.supplyItems.length );
            self.supplyItems.push(newSupplyCrate);
            return null;
        }
};
SupplyController.prototype.GetAllSupplies = function (Packet, builder) {
    let self = this;
    let supplyList = [];
    self.supplyItems.forEach(function (supplyCrate, i) {
        if (!supplyCrate.isNew && !supplyCrate.isDeath)
        {
            Packet.Models.Supply.startSupply(builder);
            Packet.Models.Supply.addPos(builder,
                Packet.Models.Vec3.createVec3(builder,
                    supplyCrate.pos_x,
                    supplyCrate.pos_y,
                    supplyCrate.pos_z,
                ));

            Packet.Models.Supply.addAssetId(builder, Packet.Models.SupplyTypes[supplyCrate.assetName]);

            let supply = Packet.Models.Supply.endSupply(builder);
            supplyList.push(supply);
        }
    });
    console.log("World Info Supplies: "+supplyList.length);
    return supplyList;
}
SupplyController.prototype.GetSupplies = function (Packet, builder) {
    let self = this;
    let supplyList = [];
    self.supplyItems.forEach(function (supplyCrate, i) {
        if (supplyCrate.isNew || supplyCrate.isDeath)
        {
            Packet.Models.Supply.startSupply(builder);
            Packet.Models.Supply.addPos(builder,
                Packet.Models.Vec3.createVec3(builder,
                    supplyCrate.pos_x,
                    supplyCrate.pos_y,
                    supplyCrate.pos_z,
                ));

            Packet.Models.Supply.addAssetId(builder, Packet.Models.SupplyTypes[supplyCrate.assetName]);

            if (supplyCrate.isNew)
            {
                Packet.Models.Supply.addIsNew(builder, supplyCrate.isNew);
            }

            if (supplyCrate.isDeath)
            {
                Packet.Models.Supply.addIsDeath(builder, supplyCrate.isDeath);
                self.supplyItems.splice(i, 1);
            }

            let supply = Packet.Models.Supply.endSupply(builder);
            supplyList.push(supply);
            supplyCrate.isNew = false;
            //this.supplyItems[i].isNew = false;
        }
    });
    return supplyList;
};
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}