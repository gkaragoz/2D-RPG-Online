"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""
from warrant import Cognito
from flask import Flask
from flask import request
from flask import jsonify
from datetime import datetime
from flask_pymongo import PyMongo
from apiclient import discovery
import httplib2
from oauth2client import client
from google.oauth2 import id_token
from google.auth.transport import requests
import uuid
import boto3

from manaconfig import SECRET_HASH, ACCESS_ID

app = Flask(__name__)
app.config["MONGO_URI"] = "mongodb://localhost:27017/ManaShiftGameDB"
mongo = PyMongo(app)
accounts = mongo.db.Accounts
account_chars = mongo.db.AccountCharacters
sessions = mongo.db.AccountSessions
# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app


@app.route('/api/auth/status', methods=['GET'])
def status():
    resp = {"GameServer": "ONLINE"}
    return jsonify(resp)


@app.route('/api/auth/changepass', methods=['POST'])
def changepass():
    resp = {"Status": "CHANGED"}
    content = request.get_json()
    print(content)
    return jsonify(resp)

@app.route('/api/user/account', methods=['POST'])
def get_accountdata():
    content = request.get_json()
    print(content)
    success = False
    if "session_id" not in content:
        abort(403)

    session_obj = sessions.find_one({'session_id': content["session_id"]})

    if session_obj is None:
        abort(403)

    account = accounts.find_one({'email': session_obj["email"]})

    if account is None:
        abort(403)

    accountChars = list(account_chars.find({'account_email': account["email"]}))
    chars = []

    try:
       for char in accountChars:
            charObj = {
                        "account_id": str(char["account_id"]),
                        "account_email": char["account_email"],
                        "name": char["name"],
                        "class_id" : char["class"],
                        "level":  char["level"],
                        "exp":  char["exp"],
            }

            chars.append(charObj)
    except BaseException as err:
        print(err)
 
    success = True
    resp = {
            "success": success,
            "gem": account["gem"],
            "gold": account["gold"],
            "chars": chars
        }

    return jsonify(resp)

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', '192.168.1.2')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555

    app.run("192.168.1.2", PORT)
