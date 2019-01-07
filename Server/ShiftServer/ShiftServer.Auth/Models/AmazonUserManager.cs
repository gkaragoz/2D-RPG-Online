using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using ShiftServer.Proto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShiftServer.Auth.Models
{
    public class AmazonUserManager
    {
        private CognitoAWSCredentials credentials = null;

        private readonly AmazonCognitoIdentityProviderClient _client = null;

        private readonly string _clientId = null;
        private readonly string _poolId = null;

        public AmazonUserManager(AmazonConfig cfg)
        {
            _clientId = cfg.CLIENT_ID;
            _poolId = cfg.USERPOOL_ID;

            CognitoAWSCredentials creds = new CognitoAWSCredentials(
                   cfg.IDENITYPOOL_ID, // Identity pool ID
                   RegionEndpoint.EUCentral1 // Region

               );

            _client = new AmazonCognitoIdentityProviderClient(RegionEndpoint.EUCentral1);
        }

        public async Task<AuthResponse> SignInAsync(string userName, string password)
        {
            try
            {


                var authReq = new AdminInitiateAuthRequest
                {
                    UserPoolId = _poolId,
                    ClientId = _clientId,
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                };
                authReq.AuthParameters.Add("USERNAME", userName);
                authReq.AuthParameters.Add("PASSWORD", password);

                AdminInitiateAuthResponse authResp = await _client.AdminInitiateAuthAsync(authReq);
                if (authResp.Session == null)
                {
                    return new AuthResponse() { Success = true, AccessToken = authResp.AuthenticationResult.AccessToken,
                        IdToken = authResp.AuthenticationResult.IdToken, RefreshToken = authResp.AuthenticationResult.RefreshToken,
                        HttpCode = (int)HttpStatusCode.OK};

                }
                else
                {
                    // to do more challenges
                    return new AuthResponse() { Success = true, Session = authResp.Session };
                }
            }
            catch (UserNotFoundException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.USER_NOT_FOUND, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };
            }
            catch (InvalidParameterException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.EMAIL_INVALID, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };
            }
            catch (UserNotConfirmedException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.USER_NOT_CONFIRMED, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };

            }
        }

        public async Task<AuthResponse> SignUpAsync(string Email, string Username, string Password)
        {
            try
            {

                // Register the user using Cognito
                var signUpRequest = new SignUpRequest
                {
                    ClientId = _clientId,
                    Password = Password,
                    Username = Username,

                };

                var emailAttribute = new AttributeType
                {
                    Name = "email",
                    Value = Email
                };
                signUpRequest.UserAttributes.Add(emailAttribute);

                var resp = await _client.SignUpAsync(signUpRequest);

                return new AuthResponse() { Success = true };
            }
            catch (UsernameExistsException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.USERNAME_ALREADY_EXIST, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };
            }
            catch (UserNotFoundException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.USER_NOT_FOUND, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };
            }
            catch (InvalidParameterException err)
            {
                return new AuthResponse() { Success = false, ErrorType = (int)AuthError.EMAIL_INVALID, ErrorMessage = err.Message, HttpCode = (int)HttpStatusCode.BadRequest };
            }




        }
        public async Task<AuthResponse> ChangePasswordAsync(string oldPassword, string password)
        {
            try
            {


                //var changePwRequest = new ChangePasswordRequest
                //{
                //    AccessToken = 
                //};
                //changePwRequest.PreviousPassword = oldPassword;
                //changePwRequest.ProposedPassword = password;


                //ChangePasswordResponse changeResp = await _client.ChangePasswordAsync(changePwRequest);

                return new AuthResponse() { Success = true };
            }
            catch (Exception err)
            {
                return new AuthResponse() { Success = false, ErrorMessage = err.Message };
            }
        }
    }
}
