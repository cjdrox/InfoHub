using System;
using InfoHub.Infrastructure.Logging;

namespace InfoHub.Infrastructure.SafeCall
{
    public class Safely
    {
        public static void Call(Action method, params object[] parameters)
        {
            try
            {
                method();
            }
            catch (Exception ex)
            {
                Logger.Log(LogEventType.Exception, ex.Message);
                throw;
            }
        }


        public static T Call<T>(Func<T> method, params object[] parameters)
        {
            try
            {
                var retValue = method();
                return retValue;
            }
            catch (Exception ex)
            {
                Logger.Log(LogEventType.Exception, ex.Message);
                throw;
            }
        }
    }
}
