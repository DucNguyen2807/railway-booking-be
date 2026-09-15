using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Interfaces
{
    public interface IGenericRepository<T>
        where T : class
    {
        IQueryable<T> Get(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>,
            IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            bool noTracking = false);

        IQueryable<T> GetIncludeMultiLayer(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>,
            IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>,
            IIncludableQueryable<T, object>>?
                include = null,
            bool noTracking = false);

        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> filter);
        T GetByID(object id);
        Task InsertAsync(T entity);

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task UpdateOrInsertAsync(T entity);
    }
}
