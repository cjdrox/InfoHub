using System;
using System.Collections.Generic;
using System.Linq;
using Infohub.Repository.Helpers;
using Infohub.Repository.Interfaces;
using NHibernate.Linq;

namespace Infohub.Repository.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        public T Add(T item)
        {
            using (var session = SessionFactoryHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public IEnumerable<T> Add(IEnumerable<T> items)
        {
            IList<T> list = new List<T>();

            using (var session = SessionFactoryHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var item in items)
                    {
                        session.Save(item);
                        list.Add(item);
                    }
                    transaction.Commit();
                }
            }

            return list;
        }

        public T Update(T item)
        {
            using (var session = SessionFactoryHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public T Remove(T item)
        {
            using (var session = SessionFactoryHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(item);
                    transaction.Commit();
                }
            }

            return item;
        }

        public T Get(Guid userGuid)
        {
            using (var session = SessionFactoryHelper.OpenSession())
                return session.Get<T>(userGuid);
        }

        public IEnumerable<T> GetAll()
        {
            using (var session = SessionFactoryHelper.OpenSession())
                return session.Query<T>();
        }

        public IEnumerable<T> Query(Func<T, bool> func)
        {
            using (var session = SessionFactoryHelper.OpenSession())
                return session.Query<T>().Where(func);
        }
    }
}

