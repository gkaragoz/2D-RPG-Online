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
                        "account_id": char["account_id"],
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

@app.route('/api/char/add', methods=['POST'])
def add_character():
    resp = {
        "error_message": "",
        "success": False,
        "character": {
            
        }
    }
    content = request.get_json()
    print(content)

    if "session_id" not in content:
        abort(403)

    if "char_class" not in content:
        abort(403)

    if "char_name" not in content:
        abort(403)

    session_obj = sessions.find_one({'session_id': content["session_id"]})

    if session_obj is None:
        abort(403)

    account = accounts.find_one({'email': session_obj["email"]})

    if account is None:
        abort(403)

    char_in_db = account_chars.find_one({'name': content["char_name"]})

    if char_in_db is not None:
        resp["error_message"] = "Character Name Already Exist"
        return jsonify(resp)
    else:


        accountChars = list(account_chars.find({'account_email': account["email"]}))
        chars = []

        for char in accountChars:
            charObj = {
                        "account_id": char["account_id"],
                        "account_email": char["account_email"],
                        "name": char["name"],
                        "class_id" : char["class"],
                        "level":  char["level"],
                        "exp":  char["exp"],
            }

            chars.append(charObj)


        if len(chars) == 4:
            resp["error_message"] = "Already has 4 characters"
            resp["success"] = False
            return jsonify(resp)

        if int(content["char_class"]) > 3:
            resp["error_message"] = "Malformed data"
            resp["success"] = False
            return jsonify(resp)
 
        if int(content["char_name"]) > 14:
            resp["error_message"] = "Character username is too long. Must be lower than 14 chars"
            resp["success"] = False
            return jsonify(resp)

        CharObject = {
             "account_id": account.get('_id'),
             "account_email":  account["email"],
             "name": content["char_name"],
             "class": content["char_class"],
             "level": 1,
             "exp": 0,
        }
        char_inserted_id = account_chars.insert_one(CharObject).inserted_id
        resp["success"] = True
        resp["character"] = CharObject
        print(char_inserted_id)

    return jsonify(resp)



@app.route('/api/auth/guestlogin', methods=['POST'])
def guestlogin():
    content = request.get_json()
    print(content)
    resp = {
                "success": False,
                "session_id": "",
    }
    if "guest_id" not in content:
       abort(403)
    else:
       token = content["guest_id"]
    
    if len(token) == 0 or token == "":
        token = str(uuid.uuid4())
    
    is_guest = True

    CLIENT_ID = "374468244948-1vrf3t9ol7so72uo1as7nbo1icrmjbvb.apps.googleusercontent.com"
    ## If this request does not have `X-Requested-With` header, this could be a CSRF
    #if not request.headers.get('X-Requested-With'):
    #    abort(403)

 
    # (Receive token by HTTPS POST)
    # ...

    try:

        userid = token
        email = ""

        print("userid:" + userid + "email:" + email)
    
        AccountObject = {
            'userid': userid,
            'email': email,
            'is_guest': is_guest,
            'gold': 1000,
            'gem': 300,
            'created_at': datetime.utcnow(),
            'updated_at': datetime.utcnow()
        }
        
        guest_account = accounts.find_one({'userid': token})
        if guest_account is None:
            user_inserted_id = accounts.insert_one(AccountObject).inserted_id

        

        session = {
               "email": email,
               "sub": userid,
               "is_guest": is_guest,
               "session_id": str(uuid.uuid4()),
               "expire_in": 3600,
               "created_at": datetime.utcnow(),
               "updated_at": datetime.utcnow()
        }

        session_id = sessions.update_one({"sub": userid}, {"$set":session}, upsert = True)

        resp["success"] = True
        resp["session_id"] = session["session_id"]
        resp["user_id"] = session["sub"]      
        resp['email_verified']: False

    except BaseException as err:
        print(err)
        resp["success"] = False

    return jsonify(resp)

@app.route('/api/auth/login', methods=['POST'])
def login():
    content = request.get_json()
    print(content)
    resp = {
                "success": False,
                "session_id": "",
                "guest_id": "",
               
    }

    
    if "id_token" not in content:
        abort(403)
    else:
        token = content["id_token"]

    CLIENT_ID = "374468244948-1vrf3t9ol7so72uo1as7nbo1icrmjbvb.apps.googleusercontent.com"
    ## If this request does not have `X-Requested-With` header, this could be a CSRF
    #if not request.headers.get('X-Requested-With'):
    #    abort(403)

 
    # (Receive token by HTTPS POST)
    # ...

    try:
        userid = ""
        email = ""
        # Specify the CLIENT_ID of the app that accesses the backend:
        idinfo = id_token.verify_oauth2_token(token, requests.Request(), CLIENT_ID)


        if idinfo['iss'] not in ['accounts.google.com', 'https://accounts.google.com']:
            raise ValueError('Wrong issuer.')
             
        userid = idinfo['sub']
        email = idinfo['email']
       
        print("userid:" + userid + "email:" + email)
    
        AccountObject = {
            'userid': userid,
            'email': email,
            'is_guest': is_guest,
            'gold': 1000,
            'gem': 300,
            'created_at': datetime.utcnow(),
            'updated_at': datetime.utcnow()
        }
        

        account = accounts.find_one({'email': email})
        if account is None:
            user_inserted_id = accounts.insert_one(AccountObject).inserted_id
 

        

        session = {
               "email": email,
               "sub": userid,
               "is_guest": is_guest,
               "session_id": str(uuid.uuid4()),
               "expire_in": 3600,
               "created_at": datetime.utcnow(),
               "updated_at": datetime.utcnow()
        }

        session_id = sessions.update_one({"email": email}, {"$set":session}, upsert = True)


        resp["success"] = True
        resp["session_id"] = session["session_id"]
        resp["user_id"] = session["sub"]
        resp["is_guest"] = False
        resp['email_verified']: idinfo["email_verified"]
    except BaseException as err:
        print(err)
        resp["success"] = False

 
    return jsonify(resp)

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', '192.168.1.2')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555

    app.run("192.168.1.2", PORT)
