using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Data.Models
{
    public class Project : TableBase
    {
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsSetting { get; set; }

        public Guid? ProjectTypeId { get; set; }

        [JsonIgnore]
        [ForeignKey("ProjectTypeId")]
        public virtual ProjectType ProjectType { get; set; }

        [JsonIgnore]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
