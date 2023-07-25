namespace EmployeeTracking.Core.EmailService
{
    public class EmailSettings
    {
        public const string ConfigurationProps = "ApplicationConfiguration";
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
