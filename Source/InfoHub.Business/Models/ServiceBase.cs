using System;
using InfoHub.Business.Interfaces;
using InfoHub.Business.Types;

namespace InfoHub.Business.Models
{
    public class ServiceBase<T> : IService<T> where T : class
    {
        protected readonly ServiceResponse<T> NullResponse;

        public ServiceBase()
        {
            NullResponse = new ServiceResponse<T>
            {
                Exception = new NotImplementedException(),
                Message = "This method must be overriden",
                ServedAt = DateTime.Now,
                ServiceData = null,
                Status = ResponseStatus.Failed
            };
        }

        public virtual IServiceResponse<T> GetNullResponse()
        {
            return NullResponse;
        }

        public virtual void Dispose()
        {
        }
    }
}
