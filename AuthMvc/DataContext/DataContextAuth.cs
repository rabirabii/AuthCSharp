using AuthMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthMvc.DataContext
{
    public class DataContextAuth : DbContext
    {
        public DataContextAuth(DbContextOptions<DataContextAuth> options) : base(options)
        { }
        public DbSet<UserModel> Users { get; set; }
        
    }
}
