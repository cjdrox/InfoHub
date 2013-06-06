using System;
using InfoHub.Business.Interfaces;
using InfoHub.Business.Types;

namespace InfoHub.Business.Models
{
    public class ServiceResponse<T> : IServiceResponse<T> where T:class
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public T ServiceData { get; set; }
        public Exception Exception { get; set; }
        public DateTime ServedAt { get; set; }
        //public BusinessEvent EventData { get; set; }
    }
}
