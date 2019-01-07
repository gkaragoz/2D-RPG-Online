"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""
from warrant import Cognito
from flask import Flask
from flask import request
from flask import jsonify
import boto3

from manaconfig import SECRET_HASH, ACCESS_ID

app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app


@app.route('/api/auth/login', methods=['POST'])
def login():
    content = request.get_json()

    resp = {
        "Success": False,
        "HttpCode": 0,
        "Session": None,
        "AccessToken": "",
        "RefreshToken": "",
        "IdToken": "",
        "ErrorMessage": "",
        "ErrorType": 0,
        "ExpireIn": 0,     
    }
    u = Cognito('eu-central-1_Csbkzqg5d','1nasds8i6jci7vlntp03r0si8', "eu-central-1",
            username=content["Username"], access_key=ACCESS_ID, secret_key=SECRET_HASH)

    try:
        cognitoData = u.admin_authenticate(password=content["Password"])
    except BaseException as err:
        resp["Error"] = err.response.Error

    return jsonify(resp)

@app.route('/api/auth/signup', methods=['POST'])
def sign_up():
    content = request.get_json()
    
    resp = {
        "Success": False,
        "Session": None,
        "AccessToken": "",
        "RefreshToken": "",
        "IdToken": "",
        "ExpireIn": 0,
        "Error": {}
    }

    u = Cognito('eu-central-1_Csbkzqg5d','1nasds8i6jci7vlntp03r0si8', "eu-central-1",)

    try:
        if "Email" not in content:
            content["Email"] = ""

        u.add_base_attributes(email=content["Email"])
        cognitoData = u.register(content["Username"], content["Password"])
        resp["Success"] = True
    except BaseException as err:
        resp["Error"] = err.response["Error"]

    return jsonify(resp)

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', '192.168.1.2')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run("192.168.1.2", PORT)
