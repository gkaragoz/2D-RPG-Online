exports = module.exports = Entity;

function Entity (player) {
    this.colliders = [];
    this.pos_x = 0.00;
    this.pos_y = 0.00;
    this.pos_z = 0.00;
    this.maxHealth = 0;
    this.currentHealth = 0;
    this.movementSpeed = 0;
    this.rotationSpeed = 0;
    this.viewAngle = 0;
    this.lastProcessedInputSeqId = null;
    this.id = player.id;
    this.secureId = player.secureId;
}

Entity.prototype = {
    addState: function (state, executionTime) {
        this.addHistory(state, executionTime);
        this.state = state;
        this.sequence += 1;
    },
    addHistory: function (state, executionTime) {
        var minTime, spliceTo = 0, newHistory = new history(state, executionTime);

        this.stateHistory.push(newHistory);
        minTime = this.stateHistory[this.stateHistory.length - 1].executionTime - this.maxHistorySecondBuffer;

        for (var i = 0; i < this.stateHistory.length; i ++) {
            if (this.stateHistory[i].executionTime > minTime) {
                spliceTo = i - 1;
                break;
            }
        }
        if (spliceTo > 0) {
            this.stateHistory.splice(0, spliceTo);
        }
    },
    setRegion: function (region) {
        this.region = region;
    }
};