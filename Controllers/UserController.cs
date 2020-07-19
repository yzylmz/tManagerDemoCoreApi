using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using taskManagerDemoApi;
using taskManagerDemoApi.Models;

namespace Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        private readonly TaskManagerDbContext taskManagerDbContext;
        public UserController(IConfiguration configuration, TaskManagerDbContext taskManagerDbContext)
        {
            this.configuration = configuration;
            this.taskManagerDbContext = taskManagerDbContext;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login([FromBody] UserViewModel userViewModel)
        {
            UserModel _user = taskManagerDbContext.Users.FirstOrDefault(x => x.Username == userViewModel.Username && x.Password == userViewModel.Password);

            if (_user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration["jSecret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, "username")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                userViewModel.Token = tokenHandler.WriteToken(token);
                userViewModel.Password = string.Empty;
                userViewModel.UserId = _user.Id;

                return Ok(userViewModel);
            }
            else
            {
                return new JsonResult("wrongUsernameorPassword");
            }

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult Register([FromBody] UserViewModel userViewModel)
        {
            UserModel _user = taskManagerDbContext.Users.FirstOrDefault(x => x.Username == userViewModel.Username);

            if (_user == null)
            {
                UserModel _newUser = new UserModel();

                _newUser.Username = userViewModel.Username;
                _newUser.Password = userViewModel.Password;

                taskManagerDbContext.Users.Add(_newUser);

                taskManagerDbContext.SaveChanges();

                taskManagerDbContext.SaveChanges();

                return new JsonResult("successful");
            }
            else
            {
                return new JsonResult("usernamexist");
            }
        }


        [HttpPost("gettasks")]
        public ActionResult GetTasks([FromBody] int userId)
        {
            List<TaskModel> _userTaskList = taskManagerDbContext.Tasks.Where(x => x.UserModelId == userId).OrderBy(x => x.Date).ToList();

            return Ok(_userTaskList);
        }

        [HttpPost("createTask")]
        public ActionResult CreateTask([FromBody] TaskModel taskModel)
        {
            taskManagerDbContext.Tasks.Add(taskModel);

            int res = taskManagerDbContext.SaveChanges();

            if (res > 0)
            {
                return new JsonResult("successful");
            }else{
                 return new JsonResult("err");
            }

        }

    }
}