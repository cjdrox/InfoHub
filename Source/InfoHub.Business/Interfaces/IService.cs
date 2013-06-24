using System;

namespace InfoHub.Business.Interfaces
{
    public interface IService<T> : IDisposable where T:class
    {
        IServiceResponse<T> GetNullResponse();
    }
}