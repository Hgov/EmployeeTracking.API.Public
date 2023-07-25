using EmployeeTracking.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTracking.Core.Helpers
{
    public class EmployeeDbContext : IdentityDbContext<User, Role, string>
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<Department> Departments { get; set; }
    }

}
