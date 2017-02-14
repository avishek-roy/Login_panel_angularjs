using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Interfaces
{
    public interface IGenericFactory<T> : IDisposable where T : class
    {

        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        bool HasData(Expression<Func<T, bool>> predicate);
        int Count(Expression<Func<T, bool>> predicate);
        bool IsDuplicate(T entity);
        void Add(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        void Edit(T entity);
        void Save();
        String getHTML();
        bool hasDuplicates<T>(List<T> myList);
    }
}
