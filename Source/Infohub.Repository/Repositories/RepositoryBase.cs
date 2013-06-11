using System;
using System.Collections.Generic;
using System.Linq;
using InfoHub.ORM.Interfaces;
using InfoHub.ORM.Models;
using Infohub.Repository.Interfaces;

namespace Infohub.Repository.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : DynamicModel
    {
        private readonly IConfiguration _configuration;

        public RepositoryBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Add(T item)
        {
            using (var session = item.OpenConnection(_configuration))
            {
                using (var transaction = session.BeginTransaction())
                {
                    item.Insert(item);
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
                var hold = itemList.First();

                using (var session = hold.OpenConnection(_configuration))
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        foreach (var item in itemList)
                        {
                            item.Save();
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
            using (var session = item.OpenConnection())
            {
                using (var transaction = session.BeginTransaction())
                {
                    item.Update(item, item);
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

        public T Get(Guid userGuid)
        {
            //using (var session = SessionFactoryHelper.OpenSession())
            //    return session.Get<T>(userGuid);
            throw new NotImplementedException();
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

