﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Data;
using leave_management.Repository;

namespace leave_management.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<LeaveType> LeaveTypes { get; }
        IGenericRepository<LeaveRequest> LeaveRequests { get; }
        IGenericRepository<LeaveAllocation> LeaveAllocations { get; }

        Task Save();
    }
}
