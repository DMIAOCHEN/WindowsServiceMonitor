using log4net;
using System;
using System.Threading;

namespace WindowsServiceMonitor.Models
{
    public class Server
    {
        public Server()
        {
            LastStartTime = DateTime.Now;
        }

        public string Name { get; set; }

        public string ProcessName { get; set; }

        public bool EnableMonitor { get; set; }

        public bool EnableRestart { get; set; }

        public int Days { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int Seconds { get; set; }

        protected long RestartTicks { get; set; }

        protected DateTime LastStartTime { get; set; }

        private ILog _log = LogManager.GetLogger(typeof(Server));

        public bool IsRestartTime()
        {
            var now = DateTime.Now;

            TimeSpan ts = now - LastStartTime;
            if (ts.Days < Days)
            {
                return false;
            }

            if (Days == 0)
            {
                // 如果是零天
                DateTime dt = new DateTime(now.Year, now.Month, now.Day, Hours, Minutes, Seconds);
                if (dt < LastStartTime)
                {
                    return false;
                }
                else if (dt <= now)
                {
                    return true;
                }
            }

            if (now.Hour < Hours)
            {
                return false;
            }

            if (now.Minute < Minutes)
            {
                return false;
            }


            if (now.Second < Seconds)
            {
                return false;
            }
            
            return true;
        }

        public bool StartServer()
        {
            
            try
            {
                if (!ServiceInstaller.ServiceIsRun(Name))
                {
                    _log.InfoFormat("正在启动{0}服务", Name);
                    ServiceInstaller.ServiceControl(Name);
                    _log.InfoFormat("启动{0}服务成功", Name);

                    LastStartTime = DateTime.Now;

                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }


        public void RestartServer()
        {
            _log.InfoFormat("准备重新启动{0}服务", Name);

            try
            {
                if (ServiceInstaller.ServiceIsRun(Name))
                {
                    _log.InfoFormat("检测到服务{0}正在运行,先将其停止", Name);
                    // 如果服务正在运行, 则先停止服务
                    ServiceInstaller.ServiceControl(Name);
                }

                int nCount = 0;
                while (true)
                {
                    if (!ServiceInstaller.ServiceIsRun(Name))
                    {
                        _log.InfoFormat("停止服务{0}成功", Name);
                        break;
                    }

                    Thread.Sleep(5 * 1000);

                    if (++nCount > 100)
                    {
                        // 强制结束进程
                        _log.InfoFormat("停止服务失败,需要强制结束进程{0}", ProcessName);
                        ServiceInstaller.FindAndKillProcessByName(ProcessName);

                        break;
                    }
                }

                Thread.Sleep(1000 * 10);

                // 启动服务
                _log.InfoFormat("正在启动{0}服务", Name);
                ServiceInstaller.ServiceControl(Name);
                _log.InfoFormat("启动{0}服务成功", Name);

                LastStartTime = DateTime.Now;
            }
            catch (Exception) { }
        }
    }
}
