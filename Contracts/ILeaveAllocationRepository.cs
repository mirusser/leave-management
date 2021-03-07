using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        Task<bool> CheackIfAllocationExistsForEmployee(int leaveTypeId, string employeeId);
        Task<IQueryable<LeaveAllocation>> GetLeaveAllocationsByEmployeeId(string employeeId);
        Task<LeaveAllocation> GetLeaveAllocationByEmployeeIdAndLeaveTypeId(string employeeId, int leaveTypeId);
    }
}
