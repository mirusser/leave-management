using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool CheckIfExistsById(int id)
        {
            return db.LeaveRequests.Any(x => x.Id == id);
        }

        public bool Create(LeaveRequest entity)
        {
            db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            db.LeaveRequests.Remove(entity);
            return Save();
        }

        public IQueryable<LeaveRequest> FindAll()
        {
            return db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType);
        }

        public LeaveRequest FindById(int id)
        {
            return db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<LeaveRequest> GetLeaveRequestsByEmployeeId(string employeeId)
            => db.LeaveRequests
                .Include(x => x.RequestingEmployee)
                .Include(x => x.ApprovedBy)
                .Include(x => x.LeaveType)
                .Where(x => x.RequestingEmployeeId == employeeId);

        public bool Save()
        {
            return db.SaveChanges() != default;
        }

        public bool Update(LeaveRequest entity)
        {
            db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}