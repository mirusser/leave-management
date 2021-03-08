using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace leave_management.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> db;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            db = this.context.Set<T>();
        }

        public async Task<bool> CheckIfExists(Expression<Func<T, bool>> searchExpression = null)
        {
            IQueryable<T> query = db;

            return await query.AnyAsync(searchExpression);
        }

        public async Task<T> Find(Expression<Func<T, bool>> searchExpression = null, List<string> includes = null)
        {
            IQueryable<T> query = db;

            if (includes != null && includes.Any())
            {
                foreach (var table in includes)
                {
                    query = query.Include(table);
                }
            }

            return await query.FirstOrDefaultAsync(searchExpression);
        }

        public async Task<IQueryable<T>> FindAll(Expression<Func<T, bool>> searchExpression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderByExpression = null, List<string> includes = null)
        {
            IQueryable<T> query = db;

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }

            if (includes != null && includes.Any())
            {
                foreach (var table in includes)
                {
                    query = query.Include(table);
                }
            }

            if (orderByExpression != null)
            {
                query = orderByExpression(query);
            }

            return query;
        }

        //public async Task<bool> Save()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<bool> Create(T entity)
        {
            var entityEntry = await db.AddAsync(entity);

            return entityEntry.State == EntityState.Added;
        }

        public bool Delete(T entity)
        {
            var entityEntry = db.Remove(entity);

            return entityEntry.State == EntityState.Deleted;
        }

        public bool Update(T entity)
        {
            var entityEntry = db.Update(entity);

            return entityEntry.State == EntityState.Modified;
        }
    }
}
