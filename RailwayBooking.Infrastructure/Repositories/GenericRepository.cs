using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Infrastructure.Repositories
{
    public class GenericRepository<TEntity>
        : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected readonly BookingDbContext _context;

        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(
            BookingDbContext context)
        {
            _context = context;

            _dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            bool noTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in
                     includeProperties.Split(
                         new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public IQueryable<TEntity>
            GetIncludeMultiLayer(
                Expression<Func<TEntity, bool>>?
                    filter = null,
                Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>>?
                    orderBy = null,
                Func<IQueryable<TEntity>,
                IIncludableQueryable<TEntity,
                object>>? include = null,
                bool noTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public async Task<TEntity?>
            FirstOrDefaultAsync(
                Expression<Func<TEntity, bool>>
                    filter)
        {
            return await _dbSet
                .FirstOrDefaultAsync(filter);
        }

        public async Task InsertAsync(
            TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Entry(entity).State =
                EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public async Task UpdateOrInsertAsync(
            TEntity entity)
        {
            var keyName = _context.Model
                .FindEntityType(typeof(TEntity))
                ?.FindPrimaryKey()
                ?.Properties
                ?.Select(x => x.Name)
                .FirstOrDefault();

            if (keyName == null)
            {
                throw new Exception(
                    "Primary key not found");
            }

            var keyValue = _context
                .Entry(entity)
                .Property(keyName)
                .CurrentValue;

            var isInsert =
                keyValue == null ||
                keyValue.Equals(0L);

            if (isInsert)
            {
                await _dbSet.AddAsync(entity);
            }
            else
            {
                _context.Entry(entity).State =
                    EntityState.Modified;
            }
        }
    }
}
