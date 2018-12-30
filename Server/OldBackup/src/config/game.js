var config = {};

config.game = {};
config.game.interval = process.env.SERVER_INTERVAL || 15;
config.game.absolute_delta_time = process.env.ABSOLUTE_DT || 0.02;

module.exports = config;