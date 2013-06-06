using System;
using System.Collections.Generic;

namespace Infohub.Repository.Interfaces
{
    public interface IRepository<T> where T:class
    {
        T Add(T item);
        IEnumerable<T> Add(IEnumerable<T> items);
        T Update(T item);
        T Remove(T item);
        T Get(Guid id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Query(Func<T, bool> func);
    }
}
