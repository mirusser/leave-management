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

        public bool CheckIfExistsById(int id)
        {
            return db.LeaveAllocations.Any(x => x.Id == id);
        }

        public bool Create(LeaveAllocation entity)
        {
            db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public IQueryable<LeaveAllocation> FindAll()
        {
            return db.LeaveAllocations.Include(x => x.Employee).Include(x => x.LeaveType);
        }

        public IQueryable<LeaveAllocation> GetLeaveAllocationsByEmployeeId(string employeeId)
            => db.LeaveAllocations.Where(x => x.EmployeeId == employeeId && x.Period == DateTime.Now.Year).Include(x => x.LeaveType).Include(x => x.Employee);

        public LeaveAllocation FindById(int id)
        {
            return db.LeaveAllocations.Include(x => x.Employee).Include(x => x.LeaveType).FirstOrDefault(x => x.Id == id);
        }

        public bool Save()
        {
            return db.SaveChanges() != default;
        }

        public bool Update(LeaveAllocation entity)
        {
            db.LeaveAllocations.Update(entity);
            return Save();
        }

        public bool CheackIfAllocationExistsForEmployee(int leaveTypeId, string employeeId)
            => db.LeaveAllocations.Any(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId && x.Period == DateTime.Now.Year);
    }
}
