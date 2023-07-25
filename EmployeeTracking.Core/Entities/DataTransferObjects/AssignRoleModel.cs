using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class AssignRoleModel
    {
        public string RoleId { get; set; } = default!;
        public string RoleName { get; set; } = default!;
        public bool IsAssigned { get; set; }
    }
}
