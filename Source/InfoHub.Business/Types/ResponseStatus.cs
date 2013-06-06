namespace InfoHub.Business.Types
{
    public enum ResponseStatus
    {
        Success,
        Failed
    }

    public static class ResponseHelper
    {
        public static string ResposeMessage(this ResponseStatus responseStatus)
        {
            switch (responseStatus)
            {
                case ResponseStatus.Success:
                    return "Success";
                default:
                    return "Failed";
            }
        }
    }
}