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

import boto3

from manaconfig import SECRET_HASH, ACCESS_ID

app = Flask(__name__)
app.config["MONGO_URI"] = "mongodb://localhost:27017/ManaShiftGameDB"
mongo = PyMongo(app)
accounts = mongo.db.Accounts
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

@app.route('/api/auth/login', methods=['POST'])
def login():
    content = request.get_json()
    print(content)

    resp = {
        "Success": False,
        "Session": "",
        "AccessToken": "",
        "Code": "",
        "ErrorMessage": "",
        "RefreshToken": "",
        "IdToken": "",
        "ExpireIn": 0,     
    } 

    try:
     
  

       resp["AccessToken"] = u.access_token
       resp["RefreshToken"] = u.refresh_token
       resp["IdToken"] = u.id_token

       account = accounts.find_one({'username': content["username"]})
       if account is None:
            resp["ErrorMessage"] = "Cant find account"
            resp["Code"] = "AccountDoesNotExist"
            return jsonify(resp)

       session = {
               "username": str(u.username),
               "session_id": u.access_token,
               "expire_in": 3600,
               "created_at": datetime.utcnow(),
               "updated_at": datetime.utcnow()
       }

       session_id = sessions.update_one({"username": str(u.username)}, {"$set":session}, upsert = True)
       resp["Success"] = True
       print(session)
    except BaseException as err:
        print(err)
        resp["ErrorMessage"] = err.response["Error"]["Message"]
        resp["Code"] = err.response["Error"]["Code"]

    return jsonify(resp)

@app.route('/api/auth/signup', methods=['POST'])
def sign_up():
    content = request.get_json()
    print(content)
    resp = {
        "Success": False,
        "Session": "",
        "AccessToken": "",
        "Code": "",
        "ErrorMessage": "",
        "RefreshToken": "",
        "IdToken": "",
        "ExpireIn": 0,     
    } 

    u = Cognito('eu-central-1_Csbkzqg5d','1nasds8i6jci7vlntp03r0si8', "eu-central-1",)

    try:
        if "email" not in content:
           content["email"] = ""    
        elif  len(content["password"]) < 8:
           resp["ErrorMessage"] = "Provide a valid Password"
           resp["Code"] = "InvalidPasswordLength"

           return jsonify(resp)

        account = accounts.find_one({'email': content["email"]})
        if account is not None:
            resp["ErrorMessage"] = "Email already exist in our database"
            resp["Code"] = "EmailAlreadyExist"
            return jsonify(resp)


        u.add_base_attributes(email=content["email"])
        cognitoData = u.register(content["username"], content["password"])
        userUsername = u.username
        userEmail = u.email
        resp["Success"] = True


        AccountObject = {
            'username': userUsername,
            'email': userEmail,
            'created_at': datetime.utcnow(),
            'updated_at': datetime.utcnow()
        }

        user_id = accounts.insert_one(AccountObject).inserted_id
        print("New Account Document Inserted")
    except BaseException as err:
        print(err)
        resp["ErrorMessage"] = err.response["Error"]["Message"]
        resp["Code"] = err.response["Error"]["Code"]

    return jsonify(resp)

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', '192.168.1.2')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555

    app.run("192.168.1.2", PORT)
