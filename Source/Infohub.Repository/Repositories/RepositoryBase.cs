using System;
using System.Collections.Generic;
using InfoHub.ORM.Extensions;
using System.Linq;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using Infohub.Repository.Interfaces;

namespace Infohub.Repository.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class, /*Table,*/ new()
    {
        private readonly IDatabaseAdapter _adapter;

        public RepositoryBase(IDatabaseAdapter adapter)
        {
            _adapter = adapter;
        }

        public T Add(T item)
        {
            using (var connection = _adapter.OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    //transaction.Insert(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public IEnumerable<T> Add(IEnumerable<T> items)
        {
            IList<T> list = new List<T>();

            var itemList = items as List<T> ?? items.ToList();
            if (items!=null && itemList.Any())
            {
                using (var connection = _adapter.OpenConnection())
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var item in itemList)
                        {
                            //transaction.Insert(item);
                            list.Add(item);
                        }
                        transaction.Commit();
                    }
                }

                return list;
            }

            return null;
        }

        public T Update(T item)
        {
            using (var connection = _adapter.OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    //session.Update(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public T Remove(T item)
        {
            //using (var session = SessionFactoryHelper.OpenSession())
            //{
            //    using (var transaction = session.BeginTransaction())
            //    {
            //        session.Delete(item);
            //        transaction.Commit();
            //    }
            //}

            return item;
        }

        public T Get(Guid id)
        {
            T item;
            using (var connection = _adapter.OpenConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    item = transaction.Query<T>(null).FirstOrDefault();
                    transaction.Commit();
                }
            }

            return item;
        }

        public IEnumerable<T> GetAll()
        {
            //using (var session = SessionFactoryHelper.OpenSession())
            //    return session.Query<T>();
            throw new NotImplementedException();
        }

        public IEnumerable<T> Query(Func<T, bool> func)
        {
            //using (var session = SessionFactoryHelper.OpenSession())
            //    return session.Query<T>().Where(func);
            throw new NotImplementedException();
        }
    }
}

