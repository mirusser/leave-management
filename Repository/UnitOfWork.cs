using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Contracts;
using leave_management.Data;

namespace leave_management.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IGenericRepository<LeaveType> leaveTypes;
        private IGenericRepository<LeaveRequest> leaveRequests;
        private IGenericRepository<LeaveAllocation> leaveAllocations;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IGenericRepository<LeaveType> LeaveTypes 
            => leaveTypes ??= new GenericRepository<LeaveType>(context);

        public IGenericRepository<LeaveRequest> LeaveRequests
            => leaveRequests ??= new GenericRepository<LeaveRequest>(context);

        public IGenericRepository<LeaveAllocation> LeaveAllocations
            => leaveAllocations ??= new GenericRepository<LeaveAllocation>(context);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                context.Dispose();
            }
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
