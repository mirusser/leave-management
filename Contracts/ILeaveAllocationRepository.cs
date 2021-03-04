﻿using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        bool CheackIfAllocationExistsForEmployee(int leaveTypeId, string employeeId);
        IQueryable<LeaveAllocation> GetLeaveAllocationsByEmployeeId(string employeeId);
    }
}
