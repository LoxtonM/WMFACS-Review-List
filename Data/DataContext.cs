using Microsoft.EntityFrameworkCore;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Data //replace with your own data where appropriate
{
    public class DataContext : DbContext
    { 
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<StaffMembers> StaffMembers { get; set; }
        public DbSet<PatientReferrals> PatientReferrals { get; set; }
        public DbSet<ActivityItems> ActivityItems { get; set; }
        public DbSet<AreaNames> AreaNames { get; set; }
        public DbSet<AdminStatuses> AdminStatuses { get; set; }
        public DbSet<Pathways> Pathways { get; set; }
    }
}
