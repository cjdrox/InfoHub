using System;
using InfoHub.Business.Interfaces;

namespace InfoHub.Business.Models
{
    public class ServiceRequest<T> : IServiceRequest<T>
    {
        public DateTime RequestedAt { get; set; }
        //public BusinessEvent EventData { get; set; }
        public T Data { get; set; }
    }
}
