using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static EmployeeTracking.Core.Entities.Enums;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class EditUserModel
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public Department Department { get; set; } = Department.Software;
        public List<AssignRoleModel> Roles { get; set; } = new();
    }
}
