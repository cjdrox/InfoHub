using System.ServiceProcess;

namespace InfoHub.JobAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
                                              { 
                                                  new LogService() 
                                              };
            ServiceBase.Run(servicesToRun);
        }
    }
}
