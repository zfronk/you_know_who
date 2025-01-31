using Microsoft.EntityFrameworkCore;

namespace Hello_Docker_Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<UsersModel> users { get; set; }
        public DbSet<CreatedPostModel> posts {get;set;}


    }

}

