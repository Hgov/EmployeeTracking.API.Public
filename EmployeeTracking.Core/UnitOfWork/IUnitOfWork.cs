using EmployeeTracking.Core.Entities;
using EmployeeTracking.Core.LoggerManager;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILoggerManager loggerManager { get; }
        int Complete();
        void Dispose();
    }

}
