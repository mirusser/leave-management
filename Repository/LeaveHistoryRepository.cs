using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext db;

        public LeaveHistoryRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool Create(LeaveHistory entity)
        {
            db.LeaveHistories.Add(entity);
            return Save();
        }

        public bool Delete(LeaveHistory entity)
        {
            db.LeaveHistories.Remove(entity);
            return Save();
        }

        public IQueryable<LeaveHistory> FindAll()
        {
            return db.LeaveHistories;
        }

        public LeaveHistory FindById(int id)
        {
            return db.LeaveHistories.FirstOrDefault(x => x.Id == id);
        }

        public bool Save()
        {
            return db.SaveChanges() != default;
        }

        public bool Update(LeaveHistory entity)
        {
            db.LeaveHistories.Update(entity);
            return Save();
        }
    }
}
