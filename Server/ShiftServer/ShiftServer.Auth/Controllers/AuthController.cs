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
    [Route("api/[controller]/[action]")]
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
        public AuthResponse Login(LoginForm form)
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
        public AuthResponse SignUp(SignUpForm form)
        {
            AuthResponse resp = new AuthResponse();

            try
            {
                resp = _userCtx.SignUpAsync(form.Email, form.Username, form.Password).Result;
                return resp;

            }
            catch (Exception err)
            {
                return resp;
            }
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public AuthResponse ChangePassword(LoginForm form)
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
