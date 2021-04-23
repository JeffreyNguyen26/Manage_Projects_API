using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models
{
    public class ProjectLastSprintM : ProjectM
    {
    }

    public class ProjectCreateM
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid ProjectTypeId { get; set; }
    }

    public class ProjectM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectTypeM ProjectType { get; set; }
        public UserM Owner { get; set; }
    }

    public class ProjectTypeM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
