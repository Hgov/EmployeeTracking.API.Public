using Microsoft.AspNetCore.Identity;


namespace EmployeeTracking.Core.Entities
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
