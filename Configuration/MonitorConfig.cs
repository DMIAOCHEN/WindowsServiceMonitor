using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WindowsServiceMonitor.Models;

namespace WindowsServiceMonitor.Configuration
{
    public class MonitorConfig : IConfigurationSectionHandler
    {
        public List<Server> MonitorServerList = new List<Server>();
        private ILog _log = LogManager.GetLogger(typeof(MonitorConfig));

        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlNodeList srvList = section.SelectNodes("Server");
            foreach (XmlNode ndSrv in srvList)
            {
                Server srv = new Server();

                bool enable = false;
                bool.TryParse(ndSrv.Attributes["enable"].Value, out enable);
                srv.EnableMonitor = enable;

                XmlNode nd = ndSrv.SelectSingleNode("Name");
                srv.Name = nd.InnerText;

                nd = ndSrv.SelectSingleNode("ProcessName");
                srv.ProcessName = nd.InnerText;

                nd = ndSrv.SelectSingleNode("AutoRestart");
                bool.TryParse(nd.Attributes["enable"].Value, out enable);
                srv.EnableRestart = enable;

                XmlNode ndDays = nd.SelectSingleNode("Days");
                int val = 0;
                Int32.TryParse(ndDays.InnerText, out val);
                srv.Days = val;

                XmlNode ndTime = nd.SelectSingleNode("Time");
                string strTime = ndTime.InnerText;

                DateTime dt = DateTime.ParseExact(strTime, "HH:mm:ss", null);
                srv.Hours = dt.Hour;
                srv.Minutes = dt.Minute;
                srv.Seconds = dt.Second;

                MonitorServerList.Add(srv);

                _log.InfoFormat("监视服务:{0}, ProcessName:{1}, EnableMonitor:{2}, EnableRestart:{3}, Days:{4}, Time:{5}:{6}:{7}",
                    srv.Name, srv.ProcessName, srv.EnableMonitor, srv.EnableRestart, srv.Days, srv.Hours, srv.Minutes, srv.Seconds);
            }

            return this;
        }
    }
}
