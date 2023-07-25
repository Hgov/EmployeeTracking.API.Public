using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.EmailService
{
    public class EmailModel
    {
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string To { get; set; } = default!;
    }
}
