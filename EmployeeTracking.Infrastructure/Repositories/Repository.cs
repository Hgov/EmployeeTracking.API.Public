using EmployeeTracking.Core.Helpers;
using EmployeeTracking.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected EmployeeDbContext _Context;
        private DbSet<TEntity> _dbSet;

        public Repository(EmployeeDbContext context)
        {
            this._Context = context;
            this._dbSet = _Context.Set<TEntity>();
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var _entity = await _dbSet.AddAsync(entity);
            return _entity.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIDAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public TEntity Update(TEntity entity)
        {
            return _dbSet.Update(entity).Entity;
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }




        //public async Task<IQueryable<TEntity>> IncludeManyAsync(params Expression<Func<TEntity, object>>[] includes)
        //{
        //    return await _dbSet.IncludeMultiple(includes);
        //}

        //public Task<IEnumerable<TEntity>> GetSqlAsync(string sql)
        //{
        //    return _dbSet.FromSql<TEntity>((FormattableString)sql);
        //}

    }
}
