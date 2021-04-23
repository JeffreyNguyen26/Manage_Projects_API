using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Data.Models
{
    public class User : TableBase
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Guid? AdminUserId { get; set; }

        [JsonIgnore]
        [ForeignKey("AdminUserId")]
        public virtual User AdminUser { get; set; }

        [JsonIgnore]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
