using System;
using InfoHub.Business.Types;
using InfoHub.Entity.Entities;

namespace InfoHub.Business.Interfaces
{
    public interface IServiceResponse<T> where T : class
    {
        ResponseStatus Status { get; set; }
        string Message { get; set; }
        T ServiceData { get; set; }
        Exception Exception { get; set; }
        DateTime ServedAt { get; set; }
        //BusinessEvent EventData { get; set; }
    }
}