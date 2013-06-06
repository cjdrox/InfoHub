using System;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;

namespace InfoHub.JobAgent
{
    public partial class LogService : ServiceBase
    {
        protected Thread Worker;
        protected ManualResetEvent ShutdownEvent;
        protected TimeSpan Delay;

        public LogService()
        {
            InitializeComponent();
            ServiceName = "InfoHub Log LogService";
        }

        protected override void OnStart(string[] args)
        {
            var worker = new ThreadStart(Work);
            ShutdownEvent = new ManualResetEvent(false);
            Worker = new Thread(worker);
            Worker.Start();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ShutdownEvent.Set();
            Worker.Join();
            base.OnStop();
        }

        protected void Work()
        {
            while (true)
            {
                if (ShutdownEvent.WaitOne(Delay, true)) break;
                Execute();
            }
        }

        protected virtual int Execute()
        {
            EventLog.WriteEntry(DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return 0;
        }
    }
}
