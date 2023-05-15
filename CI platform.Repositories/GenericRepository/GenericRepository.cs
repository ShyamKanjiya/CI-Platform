using CI_platform.Entities;
using CI_platform.Entities.DataModels;
using CI_platform.Repositories.Repository.Interface;
using CI_platform.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CI_platform.Repositories.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CIDbContext _db;
        internal DbSet<T> dbSet;

        public GenericRepository(CIDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public List<T> GetAccToFilter(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
