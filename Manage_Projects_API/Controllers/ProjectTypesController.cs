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
    public class ProjectTypesController : ControllerBase
    {
        private readonly IProjectTypeService _projectType;
        private readonly IErrorHandlerService _errorHandler;

        public ProjectTypesController(IProjectTypeService projectType, IErrorHandlerService errorHandler)
        {
            _projectType = projectType;
            _errorHandler = errorHandler;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_projectType.GetAll());
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("{project_type_id}")]
        public IActionResult GetDetail([FromRoute] Guid project_type_id)
        {
            try
            {
                return Ok(_projectType.GetDetail(project_type_id));
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
