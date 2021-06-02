using Manage_Projects_API.Models;
using Manage_Projects_API.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manage_Projects_API_Test
{
    [TestFixture]
    public class ProjectControllerTest
    {
        private UserService _user;
        private JwtAuthService _jwt;
        private ProjectService _project;
        private ProjectTypeService _projectType;

        [SetUp]
        public void Setup()
        {
            var errorHandler = new ErrorHandlerService();
            _user = new UserService(errorHandler);
            _jwt = new JwtAuthService(errorHandler);
            _project = new ProjectService(errorHandler);
            _projectType = new ProjectTypeService(errorHandler);
        }

        [Test]
        public void TestCreateProjectWithNoTUserAdmin()
        {
            var admin_user_id = GetUserID("TestCreateProjectWithUserAdmin", null);
            var user_id = GetUserID("TestCreateProjectWithNoTUserAdmin", admin_user_id);

            string message = "You do not have permission to do this acction!";
            TestCreateProjectException(admin_user_id, user_id, new ProjectCreateM
            {
               EndDate = DateTime.Now.AddMonths(10),
               Name = "TestCreateProjectName",
               ProjectTypeId = _projectType.GetAll()[0].Id,
               StartDate = DateTime.Now
            }, message);
        }

        [Test]
        public void TestCreateProjectWithNameContainSlash()
        {
            var user_id = GetUserID("TestCreateProjectWithNameContainSlash", null);
            string message = "Project name can not contain slash(/)!";
            TestCreateProjectException(Guid.Empty, user_id, new ProjectCreateM
            {
                EndDate = DateTime.Now.AddMonths(10),
                Name = "TestCreateProjectWithNameContainSlash",
                ProjectTypeId = _projectType.GetAll()[0].Id,
                StartDate = DateTime.Now
            }, message);
        }

        [Test]
        public void TestCreateProjectWithNotFoundProjectType()
        {
            var user_id = GetUserID("TestCreateProjectWithNotFoundProjectType", null);
            var id = new Guid(0, 1, 1, new byte[] { 1, 1, 1, 1, 1, 3, 2, 34 });
            string message = "The 'project type id' with value '" + id.ToString() + "' is not exist!";
            TestCreateProjectException(Guid.Empty, user_id, new ProjectCreateM
            {
                EndDate = DateTime.Now.AddMonths(10),
                Name = "TestCreateProjectWithNotFoundProjectType",
                ProjectTypeId = id,
                StartDate = DateTime.Now
            }, message);
        }

        [Test]
        public void TestCreateProjectWithExistProjectName()
        {
            var user_id = GetUserID("TestCreateProjectWithExistProjectName", null);
            _project.Add(Guid.Empty, user_id, new ProjectCreateM
            {
                EndDate = DateTime.Now.AddMonths(10),
                Name = "TestCreateProjectWithExistProjectName",
                ProjectTypeId = _projectType.GetAll()[0].Id,
                StartDate = DateTime.Now
            });
            string message = "The project name is already existed!";
            TestCreateProjectException(Guid.Empty, user_id, new ProjectCreateM
            {
                EndDate = DateTime.Now.AddMonths(10),
                Name = "TestCreateProjectWithExistProjectName",
                ProjectTypeId = _projectType.GetAll()[0].Id,
                StartDate = DateTime.Now
            }, message);
        }

        [Test]
        public void TestCreateProject()
        {
            Assert.AreEqual("TestCreateProject", new ProjectCreateM
            {
                EndDate = DateTime.Now.AddMonths(10),
                Name = "TestCreateProject",
                ProjectTypeId = _projectType.GetAll()[0].Id,
                StartDate = DateTime.Now
            }.Name);
        }

        private void TestCreateProjectException(Guid admin_user_id, Guid user_id, ProjectCreateM model, string expected)
        {
            try
            {
                _project.Add(admin_user_id, user_id, model);
            }
            catch (Exception e)
            {
                Assert.AreEqual(expected, ((RequestException)e).Error.Detail.InnerMessage);
            }
        }

        private Guid GetUserID(string name, Guid? admin_user_id)
        {
            UserAuthorizationM result;
            try
            {
                result = _user.Login(new UserLoginM
                {
                    Username = name,
                    Password = "zaq@123",
                });
            }
            catch (Exception)
            {
                result = _user.Register(new UserCreateM
                {
                    Username = name,
                    Password = "zaq@123"
                }, admin_user_id);
            }

            return result.User.Id;
        }
    }
}
