using Manage_Projects_API.Data;
using Manage_Projects_API.Data.Models;
using Manage_Projects_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Services
{
    public interface IProjectTypeService
    {
        IList<ProjectTypeM> GetAll();
        ProjectTypeM GetDetail(Guid id);
    }

    public class ProjectTypeService : ServiceBase, IProjectTypeService
    {
        public ProjectTypeService(IErrorHandlerService errorHandler) : base(errorHandler)
        {
        }

        public IList<ProjectTypeM> GetAll()
        {
            try
            {
                return Utils.GuidUtils.projectTypes.Select(pt => new ProjectTypeM
                {
                    Id = pt.Id,
                    Name = pt.Name
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project type!",
                    e, DateTime.Now, "Server", "Service_ProjectType_GetAll");
            }
        }

        public ProjectTypeM GetDetail(Guid id)
        {
            try
            {
                ProjectType project_type = Utils.GuidUtils.projectTypes.Where(pt => pt.Id.Equals(id)).FirstOrDefault();
                return new ProjectTypeM
                {
                    Id = project_type.Id,
                    Name = project_type.Name
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project type detail!",
                    e, DateTime.Now, "Server", "Service_ProjectType_GetDetail");
            }
        }
    }
}
