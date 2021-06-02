using Manage_Projects_API.Authentication;
using Manage_Projects_API.Data;
using Manage_Projects_API.Data.Models;
using Manage_Projects_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Projects_API.Services
{
    public interface IUserService
    {
        UserAuthorizationM Login(UserLoginM model);
        UserAuthorizationM Register(UserCreateM model, Guid? admin_user_id);
    }

    public class UserService : ServiceBase, IUserService
    {
        public UserService(IErrorHandlerService errorHandler) : base(errorHandler){}

        public UserAuthorizationM Login(UserLoginM model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) throw BadRequest("Username and Password must not empty!");
                if (model.Username.Length <= 3 || model.Password.Length <= 3) throw BadRequest("Username and Password must have more than 3 characters!");

                var user = Utils.GuidUtils.users.Where(u => u.Username.Equals(model.Username)).FirstOrDefault();
                if (user == null) throw BadRequest("Username or password is incorrect!");
                bool result = ProjectManagementAuthentication.VerifyHashedPassword(user.Username, user.Password, model.Password, out string rehashed_password);
                if (!result) throw BadRequest("Username or password is incorrect!");

                if (rehashed_password != null)
                {
                    user.Password = rehashed_password;
                }
                return new UserAuthorizationM
                {
                    User = new UserM
                    {
                        Id = user.Id,
                        Username = user.Username
                    }
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while log in!",
                    e, DateTime.Now, "Server", "Service_User_Login");
            }
        }

        public UserAuthorizationM Register(UserCreateM model, Guid? admin_user_id)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) throw BadRequest("The username or password not must emplty!");
                if (Utils.GuidUtils.users.Any(u => u.Username.Equals(model.Username))) throw BadRequest("The username has been used!");
                var user = new User
                {
                    Id = Utils.GuidUtils.GetNewGuid(),
                    Username = model.Username,
                    Password = ProjectManagementAuthentication.GetHashedPassword(model.Username, model.Password),
                    AdminUserId = admin_user_id
                };
                Utils.GuidUtils.users.Add(user);
                UserAuthorizationM result = new UserAuthorizationM
                {
                    User = new UserM
                    {
                        Id = user.Id,
                        Username = user.Username
                    }
                };
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while register!",
                    e, DateTime.Now, "Server", "Service_User_Register");
            }
        }
    }
}
