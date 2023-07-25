using EmployeeTracking.Core.Helpers;
using EmployeeTracking.Core.LoggerManager;
using EmployeeTracking.Core.UnitOfWork;

namespace EmployeeTracking.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private EmployeeDbContext employeeDbContext;

        public UnitOfWork(EmployeeDbContext DataContext)
        {
            loggerManager = new LoggerManager.LoggerManager();
            employeeDbContext = DataContext;

        }
        public ILoggerManager loggerManager { get; private set; }
        public int Complete()
        {

            return employeeDbContext.SaveChanges();
        }

        public void Dispose()
        {
            employeeDbContext.Dispose();
        }
    }

}
