using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WpfUserOrRoleManager.Repositories
{
    interface IGenericRepository<TEntity> where TEntity : class  //:IDisIQposable
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> ordeyBy = null,
            string includeProperties = "");
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Update(TEntity entity);
        void Save();
    }
}


