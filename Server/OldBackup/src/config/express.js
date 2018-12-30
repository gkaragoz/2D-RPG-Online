const express = require('express');
const morgan = require('morgan');
const cors = require('cors');
const helmet = require('helmet');
const passport = require('passport');
const strategies = require('./passport');
const { logs } = require('./vars');
const routes = require('../game/routes/essential');
const bodyParser = require('body-parser');

/**
 * Express instance
 * @public
 */
const app = express();
// parse body params and attache them to req.body
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// request logging. dev: console | production: file
app.use(morgan(logs));
// secure apps by setting various HTTP headers
app.use(helmet());
// enable CORS - Cross Origin Resource Sharing
app.use(cors());
// mount api v1 routes
// enable authentication
app.use(passport.initialize());
passport.use('jwt', strategies.jwt);

app.use('/', routes);

module.exports = app;
