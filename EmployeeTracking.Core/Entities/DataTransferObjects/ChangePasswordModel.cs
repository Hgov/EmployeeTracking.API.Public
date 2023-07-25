using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class ChangePasswordModel
    {
        public string Password { get; set; } = default!;
        public string NewPassword { get; set; } = default!;

        [Compare(nameof(NewPassword), ErrorMessage = "Entered passwords doesn't match.")]
        public string NewPasswordConfirm { get; set; } = default!;
    }
}
