using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Models
{
    public class UserLoginM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserAuthorizationM
    {
        public UserM User { get; set; }
        public UserM AdminUser { get; set; }
        public string Jwt { get; set; }
    }

    public class UserM
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class UserCreateM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
