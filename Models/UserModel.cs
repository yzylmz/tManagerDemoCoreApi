using System.Collections.Generic;

namespace taskManagerDemoApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<TaskModel> TaskModels { get; set; }

    }
}