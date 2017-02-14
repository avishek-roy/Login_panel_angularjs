using BOEService.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Factories
{
    public abstract class ReadOnlyFactory<C, T> :
      IReadOnlyFactory<T>
        where T : class
        where C : DbContext, new()
    {
        private C _entities = new C();
        protected C Context
        {

            get { return _entities; }
            set { _entities = value; }
        }

        public virtual IQueryable<T> GetAll()
        {

            IQueryable<T> query = _entities.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> query = _entities.Set<T>().Where(predicate);
            return query;
        }

        public bool HasData(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return FindBy(predicate).Count() > 0;
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {

            if (!this.disposed)
                if (disposing)
                    _entities.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual String getHTML()
        {
            return "";
        }
    }
}
