using NUnit.Framework;
using Manage_Projects_API.Services;
using Manage_Projects_API.Models.Https;
using Manage_Projects_API.Models;
using System;

namespace Manage_Projects_API_Test
{
    [TestFixture]
    public class AuthControllerTest
    {
        private UserService _user;
        private JwtAuthService _jwt;

        [SetUp]
        public void Setup()
        {
            var errorHandler = new ErrorHandlerService();
            _user = new UserService(errorHandler);
            _jwt = new JwtAuthService(errorHandler);
        }

        [Test]
        public void TestRegisterWithEmptyData()
        {
            string message = "The username or password not must emplty!";
            TestRegisterException(new UserCreateM
            {
                Password = null,
                Username = null
            }, message);
            TestRegisterException(new UserCreateM
            {
                Password = "",
                Username = null
            }, message);
            TestRegisterException(new UserCreateM
            {
                Password = null,
                Username = ""
            }, message);
            TestRegisterException(new UserCreateM
            {
                Password = "",
                Username = ""
            }, message);
        }

        [Test]
        public void TestRegisterWithExistedUser()
        {
            Assert.AreEqual("ThangNLD", _user.Register(new UserCreateM
            {
                Username = "ThangNLD",
                Password = "zaq@123"
            }, null).User.Username);
            TestRegisterException(new UserCreateM
            {
                Password = "ThangNLD",
                Username = "1332565"
            }, "The username has been used!");
        }

        private void TestRegisterException(UserCreateM model, string expected)
        {
            try
            {
                _user.Register(model, null);
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(expected, ((RequestException)e).Error.Detail.InnerMessage);
            }
        }

        [Test]
        public void TestLoginWithEmptyData()
        {
            string message = "Username and Password must not empty!";
            TestLoginException(new UserLoginM
            {
                Password = null,
                Username = null
            }, message);
            TestLoginException(new UserLoginM
            {
                Password = "",
                Username = null
            }, message);
            TestLoginException(new UserLoginM
            {
                Password = null,
                Username = ""
            }, message);
            TestLoginException(new UserLoginM
            {
                Password = "",
                Username = ""
            }, message);
        }

        [Test]
        public void TestLoginWithShortData()
        {
            string message = "Username and Password must have more than 3 characters!";
            TestLoginException(new UserLoginM
            {
                Password = "123456",
                Username = "IT"
            }, message);
            TestLoginException(new UserLoginM
            {
                Password = "123",
                Username = "DEV"
            }, message);
            TestLoginException(new UserLoginM
            {
                Password = "zaq",
                Username = "TESTER"
            }, message);
        }

        [Test]
        public void TestLoginWithWrongData()
        {
            string message = "Username or password is incorrect!";
            _user.Register(new UserCreateM
            {
                Username = "LoginTest",
                Password = "zaq123"
            }, null);
            TestLoginException(new UserLoginM
            {
                Username = "LoginTest",
                Password = "LoginTest"
            }, message);
            TestLoginException(new UserLoginM
            {
                Username = "LoginTes",
                Password = "zaq123"
            }, message);
            Assert.AreEqual("LoginTest", _user.Login(new UserLoginM
            {
                Username = "LoginTest",
                Password = "zaq123"
            }).User.Username);
        }

        private void TestLoginException(UserLoginM model, string expected)
        {
            try
            {
                _user.Login(model);
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(expected, ((RequestException)e).Error.Detail.InnerMessage);
            }
        }

        [Test]
        public void TestLogout()
        {
            var login = _user.Register(new UserCreateM
            {
                Username = "TestLogout",
                Password = "123456"
            }, null);
            ;
            string jwt = _jwt.GenerateJwt(login.AdminUser == null ? Guid.Empty : login.AdminUser.Id, login.User.Id, "Web_User");
            Assert.IsFalse(string.IsNullOrEmpty(jwt));
            var claim = _jwt.GetClaims(jwt);
            Assert.IsTrue(_jwt.RemoveAudience(claim.AdminUserId, claim.UserId));
        }
    }
}