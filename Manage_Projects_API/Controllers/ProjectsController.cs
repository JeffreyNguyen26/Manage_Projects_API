using Manage_Projects_API.Models;
using Manage_Projects_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IJwtAuthService _jwtAuth;
        private readonly IProjectService _project;

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_project.GetAll(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult CreateProject([FromBody] ProjectCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Created("", _project.Add(jwt_claim.AdminUserId, jwt_claim.UserId, model));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
