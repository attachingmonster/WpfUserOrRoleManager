using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WpfUserOrRoleManager.DAL;

namespace WpfUserOrRoleManager.Repositories
{
    public class GenericRepository<TEntity> : UnitOfWork where TEntity : class
    {
        private AccountContext context;
        private DbSet<TEntity> dbSet;

        public GenericRepository(AccountContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)           // !=  表示不等于
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
            //return dbSet.ToList();
        }
        public TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }
        public void Insert(TEntity entity)
        {

            dbSet.Add(entity);
            //dbSet.Add(entity);
        }
        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            dbSet.Remove(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
