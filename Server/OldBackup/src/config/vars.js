const path = require('path');
const admin = require("firebase-admin");
// import .env variables
require('dotenv-safe').load({
    path: path.join(__dirname, '../../.env'),
    sample: path.join(__dirname, '../../.env.example'),
});

module.exports = {
    env: process.env.NODE_ENV,
    port: process.env.PORT,
    jwtSecret: process.env.JWT_SECRET,
    jwtExpirationInterval: process.env.JWT_EXPIRATION_MINUTES,
    servername: process.env.SERVER_NAME,
    mongo: {
        uri: process.env.NODE_ENV === 'test'
            ? process.env.MONGODB_URI_TESTS
            : process.env.MONGODB_URI,
    },
    redis: {
        uri: process.env.NODE_ENV === 'test'
            ? process.env.REDISDB_URI
            : process.env.REDISDB_URI_TESTS,
    },
    logs: process.env.NODE_ENV === 'production' ? 'combined' : 'dev',
};
