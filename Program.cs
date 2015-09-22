using System.IO;
using Topshelf;
using WindowsServiceMonitor.ServerController;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace WindowsServiceMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            // change from service account's dir to more logical one
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                x.SetDescription("系统服务监控");
                x.SetDisplayName("eWorldServerMonitor");
                x.SetServiceName("eWorldServerMonitor");

                x.Service(factory =>
                {
                    MonitorServer server = new MonitorServer();
                    //server.Initialize();
                    return server;
                });
            });
        }
    }
}
