using Microsoft.EntityFrameworkCore;
using PracticeCoreMVC.Models;

namespace PracticeCoreMVC.Contexts
{
    public class AllRepoContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AllRepoContext(DbContextOptions<AllRepoContext> options) : base(options)
        {
        }
        public virtual DbSet<RegisterModel> Register { get; set; }
        public virtual DbSet<RoleModel> Roles { get; set; }
        public virtual DbSet<UserRoleMappingModel> UserRoleMapping { get; set; }
    }
}
