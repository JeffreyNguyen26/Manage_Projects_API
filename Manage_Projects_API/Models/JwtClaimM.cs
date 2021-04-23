using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models
{
    public class JwtClaimM
    {
        public Guid AdminUserId { get; set; }
        public Guid UserId { get; set; }
        public string ApplicationRole { get; set; }
    }
}
