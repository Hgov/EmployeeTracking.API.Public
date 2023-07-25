using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class UpsertRoleModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
