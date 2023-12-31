﻿namespace EmployeeTracking.Core.Entities.DataTransferObjects
{
    public class SignInModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; } = true;
    }
}
