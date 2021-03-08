using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {

        private readonly ApplicationDbContext db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> CheckIfExistsById(int id)
        {
            return await db.LeaveAllocations.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<IQueryable<LeaveAllocation>> FindAll()
        {
            return db.LeaveAllocations.Include(x => x.Employee).Include(x => x.LeaveType);
        }

        public async Task<IQueryable<LeaveAllocation>> GetLeaveAllocationsByEmployeeId(string employeeId)
            => db.LeaveAllocations
            .Where(x => x.EmployeeId == employeeId && x.Period == DateTime.Now.Year)
            .Include(x => x.LeaveType)
            .Include(x => x.Employee);

        public async Task<LeaveAllocation> GetLeaveAllocationByEmployeeIdAndLeaveTypeId(string employeeId, int leaveTypeId)
            => await db.LeaveAllocations
            .Include(x => x.LeaveType)
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Period == DateTime.Now.Year && x.LeaveTypeId == leaveTypeId);

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await db.LeaveAllocations.Include(x => x.Employee).Include(x => x.LeaveType).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Save()
        {
            return await db.SaveChangesAsync() != default;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            db.LeaveAllocations.Update(entity);
            return await Save();
        }

        public async Task<bool> CheackIfAllocationExistsForEmployee(int leaveTypeId, string employeeId)
            => await db.LeaveAllocations
            .AnyAsync(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId && x.Period == DateTime.Now.Year);
    }
}
