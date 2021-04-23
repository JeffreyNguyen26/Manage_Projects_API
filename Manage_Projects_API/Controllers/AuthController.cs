using Manage_Projects_API.Data.Static;
using Manage_Projects_API.Models;
using Manage_Projects_API.Services;
using Manage_Projects_API.Services.Nococid;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Manage_Projects_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly IJwtAuthService _jwtAuth;

        public AuthController(IUserService user, IJwtAuthService jwtAuth)
        {
            _user = user;
            _jwtAuth = jwtAuth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromQuery] string redirect_uri, [FromBody] UserLoginM model)
        {
            try
            {
                string role = ApplicationRole.Web_User;
                UserAuthorizationM result = _user.Login(model);
                if (model.Username.Equals(ApplicationAuth.Nococid_Application_Admin))
                {
                    role = ApplicationRole.Application_Admin;
                }
                result.Jwt = _jwtAuth.GenerateJwt(result.AdminUser == null ? Guid.Empty : result.AdminUser.Id, result.User.Id, role);
                if (string.IsNullOrEmpty(redirect_uri))
                {
                    return Ok(result);
                }
                return Redirect(redirect_uri + "?user=nococid&jwt=" + result.Jwt);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _jwtAuth.RemoveAudience(jwt_claim.AdminUserId, jwt_claim.UserId);
                return Ok("Logout success");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCreateM model)
        {
            try
            {
                var result = _user.Register(model, null);
                result.Jwt = _jwtAuth.GenerateJwt(Guid.Empty, result.User.Id, ApplicationRole.Web_User);
                return Created("", result);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
