using Microsoft.EntityFrameworkCore;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Data //replace with your own data where appropriate
{
    public class DataContext : DbContext
    { 
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
        public DbSet<ReviewItems> ReviewItems { get; set; }
        public DbSet<AreaNames> AreaNames { get; set; }
        public DbSet<AdminStatuses> AdminStatuses { get; set; }        
    }
}
