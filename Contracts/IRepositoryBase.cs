﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IQueryable<T>> FindAll();

        Task<T> FindById(int id);
        Task<bool> CheckIfExistsById(int id);

        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }

    public interface IGenericRepository<T> where T : class
    {
        Task<IQueryable<T>> FindAll(
            Expression<Func<T, bool>> searchExpression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByExpression = null,
            List<string> includes = null);

        Task<T> Find(Expression<Func<T, bool>> searchExpression = null, List<string> includes = null);
        Task<bool> CheckIfExists(Expression<Func<T, bool>> searchExpression = null);

        Task<bool> Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}
