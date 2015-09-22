using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using WindowsServiceMonitor.Configuration;

namespace WindowsServiceMonitor.ServerController
{
    public class MonitorServer : ServiceControl
    {
        public static bool bStop = false;

        public static void Start()
        {
            bStop = false;

            Thread t = new Thread(MonitorThead);
            t.Start(); 
        }

        public static void Stop()
        {
            bStop = true;
        }

        public static void MonitorThead()
        {
            MonitorConfig cfg = System.Configuration.ConfigurationManager.GetSection("winSrvMonitor") as MonitorConfig;

            while (!bStop)
            {
                foreach (var srv in cfg.MonitorServerList)
                {
                    if (srv.EnableMonitor)
                    {
                        // 如果需要监控，则发现停止的时候将其启动
                        if (srv.StartServer())
                        {
                            continue;
                        }
                    }

                    if (srv.EnableRestart)
                    {
                        if (srv.IsRestartTime())
                        {
                            srv.RestartServer();
                        }
                    }
                }

                Thread.Sleep(1000 * 60);
            }
        }


        public bool Start(HostControl hostControl)
        {
            Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Stop();

            return true;
        }
    }
}
