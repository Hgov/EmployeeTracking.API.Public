using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.EmailService
{
    public class EmailSettings
    {
        public const string ConfigurationProps = "ApplicationConfiguration";
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
