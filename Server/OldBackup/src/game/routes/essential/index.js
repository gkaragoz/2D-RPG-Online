const express = require('express');
const { admin, JWT_SECRET } = require('../../../config/vars');
const authRoutes = require('./auth.route');
const userRoutes = require('./user.route');
const { authorize, ADMIN, LOGGED_USER } = require('../../middlewares/auth');

const router = express.Router();

/**
 * GET v1/docs
 */
router.use('/docs', express.static('docs'));
router.use('/users', userRoutes);
router.use('/auth', authRoutes);


module.exports = router;
