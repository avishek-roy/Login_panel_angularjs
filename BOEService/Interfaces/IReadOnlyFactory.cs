using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Interfaces
{
    public interface IReadOnlyFactory<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        bool HasData(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        String getHTML();
    }
}
