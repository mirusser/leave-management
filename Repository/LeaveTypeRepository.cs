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

        public async Task<bool> CheckIfExistsById(int id)
            => await db.LeaveTypes.AnyAsync(x => x.Id == id);

        public async Task<bool> Create(LeaveType entity)
        {
            await db.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<IQueryable<LeaveType>> FindAll()
        {
            return db.LeaveTypes;
        }

        public async Task<LeaveType> FindById(int id)
            => await db.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IQueryable<LeaveType>> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
            => await db.SaveChangesAsync() != default;

        public async Task<bool> Update(LeaveType entity)
        {
            db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
