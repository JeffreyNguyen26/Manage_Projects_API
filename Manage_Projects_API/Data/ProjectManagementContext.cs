using Manage_Projects_API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Data
{
    public class ProjectManagementContext : DbContext
    {
        public ProjectManagementContext(DbContextOptions<ProjectManagementContext> option) : base(option) { }

        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectType> ProjectType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
