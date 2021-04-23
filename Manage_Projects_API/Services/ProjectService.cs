using Manage_Projects_API.Data;
using Manage_Projects_API.Data.Models;
using Manage_Projects_API.Data.Static;
using Manage_Projects_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Services
{
    public interface IProjectService
    {
        ProjectM Add(Guid admin_user_id, Guid user_id, ProjectCreateM model);
        IList<ProjectLastSprintM> GetAll(Guid user_id);
    }

    public class ProjectService : ServiceBase, IProjectService
    {
        private readonly IContext<Project> _project;
        private readonly IContext<Permission> _permission;
        private readonly IContext<ProjectType> _projectType;
        private readonly IContext<User> _user;

        public ProjectService(IContext<User> user, IContext<ProjectType> projectType, IContext<Permission> permission, IContext<Project> project, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _user = user;
            _projectType = projectType;
            _permission = permission;
            _project = project;
        }

        public ProjectM Add(Guid admin_user_id, Guid user_id, ProjectCreateM model)
        {
            try
            {
                if (!admin_user_id.Equals(Guid.Empty)) throw Forbidden();
                if (model.Name.Contains("/")) throw BadRequest("Project name can not contain slash(/)!");
                ProjectType project_type = _projectType.GetOne(p => p.Id.Equals(model.ProjectTypeId));
                if (project_type == null) throw NotFound(model.ProjectTypeId, "project type id");
                if (_project.Any(p => p.Name.Equals(model.Name) && p.Permissions.Any(p => p.UserId.Equals(user_id) && p.RoleId.Equals(RoleID.Admin)))) throw BadRequest("The project name is already existed!");

                Project project = _project.Add(new Project
                {
                    ProjectTypeId = model.ProjectTypeId,
                    IsDelete = false,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    CreatedDate = DateTime.Now,
                    EndDate = model.EndDate
                });
                _permission.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Admin
                });
                _permission.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Project_Manager
                });
                SaveChanges();

                return new ProjectM
                {
                    Id = project.Id,
                    CreatedDate = project.CreatedDate,
                    EndDate = project.EndDate,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    ProjectType = new ProjectTypeM
                    {
                        Id = project_type.Id,
                        Name = project_type.Name
                    },
                    Owner = _user.Where(u => u.Id.Equals(user_id)).Select(u => new UserM
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).FirstOrDefault()
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add project!",
                    e, DateTime.Now, "Server", "Service_Project_Add");
            }
        }

        public IList<ProjectLastSprintM> GetAll(Guid user_id)
        {
            try
            {
                var result = _permission.Where(p => p.Project.Permissions.Any(p => p.UserId.Equals(user_id)) && p.RoleId.Equals(RoleID.Admin))
                    .Select(p => new ProjectLastSprintM
                    {
                        CreatedDate = p.Project.CreatedDate,
                        EndDate = p.Project.EndDate,
                        Id = p.Project.Id,
                        Name = p.Project.Name,
                        ProjectType = new ProjectTypeM
                        {
                            Id = p.Project.ProjectType.Id,
                            Name = p.Project.ProjectType.Name
                        },
                        StartDate = p.Project.StartDate,
                        Owner = new UserM
                        {
                            Id = p.User.Id,
                            Username = p.User.Username
                        }
                    }).ToList();
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project!",
                    e, DateTime.Now, "Server", "Service_Project_GetAll");
            }
        }

        private int SaveChanges()
        {
            return _project.SaveChanges();
        }
    }
}
