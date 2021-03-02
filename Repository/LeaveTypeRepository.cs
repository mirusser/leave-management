using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool CheckIfExistsById(int id)
        {
            return db.LeaveTypes.Any(x => x.Id == id);
        }

        public bool Create(LeaveType entity)
        {
            db.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveType entity)
        {
            db.LeaveTypes.Remove(entity);
            return Save();
        }

        public IQueryable<LeaveType> FindAll()
        {
            return db.LeaveTypes;
        }

        public LeaveType FindById(int id)
        {
            return db.LeaveTypes.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return db.SaveChanges() != default;
        }

        public bool Update(LeaveType entity)
        {
            db.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
