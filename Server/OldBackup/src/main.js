Promise = require('bluebird'); // eslint-disable-line no-global-assign
global.__base = __dirname + '/';

const { port, env } = require('./config/vars');
const mongoose = require('./config/mongoose');
const ServerCore = require('./game/core');
const WebSocketServer = require('uws').Server;
// open mongoose connection
mongoose.connect();
const wss = new WebSocketServer({ port: port });
console.info(`Game server started on port ${port} (${env})`);
const server = new ServerCore(null, wss);
server.startUws();

/**
 * Exports express
 * @public
 */

module.exports = server;
