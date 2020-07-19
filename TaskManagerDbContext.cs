using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using taskManagerDemoApi.Models;

namespace taskManagerDemoApi
{
    public class TaskManagerDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }

        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options) { 

            
        }

        

    }
}