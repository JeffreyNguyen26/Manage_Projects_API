using Manage_Projects_API.Data.Models;
using Manage_Projects_API.Data.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Utils
{
    public class GuidUtils
    {
        private static int a = 0;
        private static short b = 0;
        private static short c = 0;
        private static byte[] bytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

        public static List<User> users = new List<User>();
        public static List<ProjectType> projectTypes = new List<ProjectType>
        {
            new ProjectType
            {
                Id = new Guid(++a, b, c, bytes),
                Name = "Desktop"
            },
            new ProjectType
            {
                Id = new Guid(++a, b, c, bytes),
                Name = "Mobile"
            }, new ProjectType
            {
                Id = new Guid(++a, b, c, bytes),
                Name = "Web Appplication"
            }
        };
        public static List<Project> projects = new List<Project>();
        public static List<Permission> permissions = new List<Permission>();
        public static List<Role> roles = new List<Role>
        {
            new Role
            {
                Id = RoleID.Admin,
                Name = "Admin"
            },
            new Role
            {
                Id = RoleID.Project_Manager,
                Name = "Project Manager"
            },new Role
            {
                Id = RoleID.Technical_Manager,
                Name = "Technician"
            },new Role
            {
                Id = RoleID.Project_Tester,
                Name = "Tester"
            },new Role
            {
                Id = RoleID.Developer,
                Name = "Developer"
            }
        };

        public static Guid GetNewGuid()
        {
            return new Guid(++a, b, c, bytes);
        }
    }
}
