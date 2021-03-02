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
            return db.LeaveAllocations;
        }

        public LeaveAllocation FindById(int id)
        {
            return db.LeaveAllocations.FirstOrDefault(x => x.Id == id);
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
    }
}
