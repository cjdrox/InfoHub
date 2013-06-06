namespace InfoHub.Business.Interfaces
{
    public interface IService<T> where T:class
    {
        IServiceResponse<T> GetNullResponse();
    }
}