using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> CheckIfExistsById(int id)
        {
            return await db.LeaveRequests.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Create(LeaveRequest entity)
        {
            await db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            db.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task<IQueryable<LeaveRequest>> FindAll()
        {
            return db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType);
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            return await db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IQueryable<LeaveRequest>> GetLeaveRequestsByEmployeeId(string employeeId)
            => db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .Where(x => x.RequestingEmployeeId == employeeId);

        public async Task<bool> Save()
        {
            return await db.SaveChangesAsync() != default;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}