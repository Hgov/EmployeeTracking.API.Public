using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
