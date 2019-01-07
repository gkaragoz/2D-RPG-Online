using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using ShiftServer.Proto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

                return new AuthResponse() { Success = true };
            }
            catch (Exception err)
            {
                return new AuthResponse() { Success = false, ErrorMessage = err.Message };
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
