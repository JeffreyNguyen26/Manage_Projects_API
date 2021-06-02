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
        public ProjectService(IErrorHandlerService errorHandler) : base(errorHandler)
        {
        }

        public ProjectM Add(Guid admin_user_id, Guid user_id, ProjectCreateM model)
        {
            try
            {
                if (!admin_user_id.Equals(Guid.Empty)) throw Forbidden();
                if (model.Name.Contains("/")) throw BadRequest("Project name can not contain slash(/)!");
                ProjectType project_type = Utils.GuidUtils.projectTypes.Where(p => p.Id.Equals(model.ProjectTypeId)).FirstOrDefault();
                if (project_type == null) throw NotFound(model.ProjectTypeId, "project type id");

                var project = Utils.GuidUtils.projects.Where(p => p.Name.Equals(model.Name)).FirstOrDefault();
                if (project != null)
                {
                    if (Utils.GuidUtils.permissions.Any(p => p.ProjectId.Equals(project.Id) && p.UserId.Equals(user_id) && p.RoleId.Equals(RoleID.Admin)))
                    {
                        throw BadRequest("The project name is already existed!");
                    }
                }

                project = new Project
                {
                    ProjectTypeId = model.ProjectTypeId,
                    IsDelete = false,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    CreatedDate = DateTime.Now,
                    EndDate = model.EndDate,
                    IsSetting = false
                };
                Utils.GuidUtils.projects.Add(project);

                Utils.GuidUtils.permissions.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Admin
                });
                Utils.GuidUtils.permissions.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Project_Manager
                });
                Utils.GuidUtils.permissions.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Technical_Manager
                });

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
                    Owner = Utils.GuidUtils.users.Where(u => u.Id.Equals(user_id)).Select(u => new UserM
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
                var result = new List<ProjectLastSprintM>();

                var project_ids = Utils.GuidUtils.permissions.Where(p => p.UserId.Equals(user_id)).Select(p => p.ProjectId).ToArray();
                var projects = Utils.GuidUtils.projects.Where(p => project_ids.Any(id => id.Equals(p.Id))).Select(p => p);
                foreach (var project in projects)
                {
                    var project_type = Utils.GuidUtils.projectTypes.Where(pt => pt.Id.Equals(project.ProjectTypeId)).FirstOrDefault();
                    user_id = Utils.GuidUtils.permissions.Where(p => p.RoleId.Equals(RoleID.Admin) && p.ProjectId.Equals(project.Id)).Select(p => p.UserId.Value).FirstOrDefault();
                    var user = Utils.GuidUtils.users.Where(u => u.Id.Equals(user_id)).FirstOrDefault();
                    result.Add(new ProjectLastSprintM
                    {
                        CreatedDate = project.CreatedDate,
                        Id = project.Id,
                        EndDate = project.EndDate,
                        Name = project.Name,
                        StartDate = project.StartDate,
                        ProjectType = new ProjectTypeM
                        {
                            Id = project.ProjectTypeId.Value,
                            Name = project_type.Name
                        },
                        Owner = new UserM
                        {
                            Id = user.Id,
                            Username = user.Username
                        }
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project!",
                    e, DateTime.Now, "Server", "Service_Project_GetAll");
            }
        }
    }
}
