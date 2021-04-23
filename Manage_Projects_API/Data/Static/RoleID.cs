using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Data.Static
{
    public class RoleID
    {
        public static Guid Project_Manager = new Guid(2, 1, 2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 });
        public static Guid Technical_Manager = new Guid(3, 1, 2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 });
        public static Guid Project_Tester = new Guid(4, 1, 2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 });
        public static Guid Developer = new Guid(5, 1, 2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 });
        public static Guid Admin = new Guid(1, 1, 2, new byte[] { 3, 4, 5, 6, 7, 8, 9, 10 });
        public static Guid[] All = new Guid[] { Technical_Manager, Project_Tester, Developer
    };
}
}
