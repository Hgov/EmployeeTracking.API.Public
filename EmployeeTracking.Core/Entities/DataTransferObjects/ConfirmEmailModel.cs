using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class ConfirmEmailModel
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
    }
}
