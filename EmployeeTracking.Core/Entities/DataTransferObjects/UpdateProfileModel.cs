

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class UpdateProfileModel
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
