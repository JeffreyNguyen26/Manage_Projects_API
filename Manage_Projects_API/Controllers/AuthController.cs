using Manage_Projects_API.Data.Static;
using Manage_Projects_API.Models;
using Manage_Projects_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Manage_Projects_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly IJwtAuthService _jwtAuth;
        private readonly IErrorHandlerService _errorHandler;

        public AuthController(IUserService user, IJwtAuthService jwtAuth, IErrorHandlerService errorHandler)
        {
            _user = user;
            _jwtAuth = jwtAuth;
            _errorHandler = errorHandler;
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
                return GetError(e);
            }
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout(string jwt)
        {
            try
            {
                JwtClaimM jwt_claim;
                if (string.IsNullOrEmpty(jwt))
                {
                    jwt_claim = _jwtAuth.GetClaims(Request);
                } else
                {
                    jwt_claim = _jwtAuth.GetClaims(jwt);
                }
                
                _jwtAuth.RemoveAudience(jwt_claim.AdminUserId, jwt_claim.UserId);
                return Ok("Logout success");
            }
            catch (Exception e)
            {
                return GetError(e);
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
                return GetError(e);
            }
        }

        private IActionResult GetError(Exception e, [CallerMemberName] string callerName = "")
        {
            if (e is RequestException re)
            {
                return re.Error.StatusCode switch
                {
                    400 => BadRequest(re.Error),
                    401 => Unauthorized(re.Error),
                    403 => StatusCode(403, re.Error),
                    404 => NotFound(re.Error),
                    _ => null,
                };
            }
            else
            {
                if (!(e is ServerException se))
                {
                    se = _errorHandler.WriteLog("An error has occured!", e, DateTime.Now, "Server", "Controller_Method_" + callerName);
                }
                return StatusCode(500, new ServerExceptionVM
                {
                    Message = se.Message,
                    Side = se.Side,
                    TraceId = se.TraceId
                });
            }
        }
    }
}
