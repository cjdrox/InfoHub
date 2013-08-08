using System;

namespace InfoHub.Business.Interfaces
{
    public interface IServiceRequest<T>
    {
        DateTime RequestedAt { get; set; }
        //BusinessEvent EventData { get; set; }
        T Data { get; set; }
    }
}