using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IEFCRUDRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
        T Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        bool Remove(T entity);
        bool RemoveRange(IEnumerable<T> entities);
    }

    public class EFCRUDRepository<T> : IEFCRUDRepository<T> where T : class
    {
        public NorthwindContext _context;
        public EFCRUDRepository(NorthwindContext context)
        {
            _context = context;
        }

        public T Add(T entity)
        {
            var insertedEntity = _context.Set<T>().Add(entity).Entity;
            _context.SaveChanges();
            return insertedEntity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Set<T>().AddRange(entities);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public bool Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChanges() > default(int);
        }

        public bool RemoveRange(IEnumerable<T> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Set<T>().RemoveRange(entities);
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public T Update(T entity)
        {
            var updatedEntity = _context.Set<T>().Update(entity).Entity;
            _context.SaveChanges();
            return updatedEntity;
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Set<T>().UpdateRange(entities);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
