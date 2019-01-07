using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShiftServer.Auth.Models;
using ShiftServer.Proto.Models;

namespace ShiftServer.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AmazonConfig> config;
        private AmazonUserManager _userCtx;


        public AuthController(IOptions<AmazonConfig> config)
        {
            this.config = config;

            _userCtx = new AmazonUserManager(this.config.Value);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<AuthResponse> LoginAsync(LoginForm form)
        {
            AuthResponse resp = new AuthResponse();

            try
            {
                resp = _userCtx.SignInAsync(form.Username, form.Password).Result;
                return resp;

            }
            catch (Exception err)
            {
                return resp;
            }
        }
        [HttpPost]
        [ActionName("SignUp")]
        public async Task<AuthResponse> SignUpAsync(LoginForm form)
        {
            AuthResponse resp = new AuthResponse();

            try
            {
                resp = _userCtx.SignInAsync(form.Username, form.Password).Result;
                return resp;

            }
            catch (Exception err)
            {
                return resp;
            }
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public async Task<AuthResponse> ChangePasswordAsync(LoginForm form)
        {
            AuthResponse resp = new AuthResponse();

            try
            {
                //resp = _userCtx.SignInAsync(form.Username, form.Password).Result;
                return resp;

            }
            catch (Exception err)
            {
                return resp;
            }
        }

    }
}
