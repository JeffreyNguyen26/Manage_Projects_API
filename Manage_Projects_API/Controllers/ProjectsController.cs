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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectsController : ControllerBase
    {
        private readonly IJwtAuthService _jwtAuth;
        private readonly IProjectService _project;
        private readonly IErrorHandlerService _errorHandler;

        public ProjectsController(IJwtAuthService jwtAuth, IProjectService project, IErrorHandlerService errorHandler)
        {
            _jwtAuth = jwtAuth;
            _project = project;
            _errorHandler = errorHandler;
        }

        [HttpGet]
        public IActionResult GetAll(string jwt)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_project.GetAll(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("create")]
        public IActionResult CreateProject([FromBody] ProjectCreateM model, string jwt)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Created("", _project.Add(jwt_claim.AdminUserId, jwt_claim.UserId, model));
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
