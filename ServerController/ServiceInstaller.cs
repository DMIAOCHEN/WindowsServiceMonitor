using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace WindowsServiceMonitor
{
    class ServiceInstaller
    {
        #region DLLImport
        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
         int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
         string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);
        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        #endregion DLLImport

        #region Constants declaration.
        static int SC_MANAGER_CREATE_SERVICE = 0x0002;
        static int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        //int SERVICE_DEMAND_START = 0x00000003;
        static int SERVICE_ERROR_NORMAL = 0x00000001;
        static int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        static int SERVICE_QUERY_CONFIG = 0x0001;
        static int SERVICE_CHANGE_CONFIG = 0x0002;
        static int SERVICE_QUERY_STATUS = 0x0004;
        static int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        static int SERVICE_START = 0x0010;
        static int SERVICE_STOP = 0x0020;
        static int SERVICE_DELETE = 0x10000;
        static int SERVICE_PAUSE_CONTINUE = 0x0040;
        static int SERVICE_INTERROGATE = 0x0080;
        static int SERVICE_USER_DEFINED_CONTROL = 0x0100;
        static int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
         SERVICE_QUERY_CONFIG |
         SERVICE_CHANGE_CONFIG |
         SERVICE_QUERY_STATUS |
         SERVICE_ENUMERATE_DEPENDENTS |
         SERVICE_START |
         SERVICE_STOP |
         SERVICE_PAUSE_CONTINUE |
         SERVICE_INTERROGATE |
         SERVICE_USER_DEFINED_CONTROL);
        static int SERVICE_AUTO_START = 0x00000002;
        #endregion Constants declaration.


        /// 
        /// 安装和运行
        /// 
        /// 程序路径.
        /// 服务名
        /// 服务显示名称.
        /// 服务安装是否成功.
        public static bool InstallService(string svcPath, string svcName, string svcDispName)
        {
            
            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0)
                    {
                        CloseServiceHandle(sv_handle);
                        CloseServiceHandle(sc_handle);
                        return false;
                    }
                    else
                    {
                        CloseServiceHandle(sv_handle);
                        CloseServiceHandle(sc_handle);
                        return true;
                    }
                }
                else

                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        static public bool ServiceIsInstall(string svcName)
        {
            ServiceController[] service = ServiceController.GetServices();
            foreach (ServiceController s in service)
            {
                if (s.ServiceName == svcName)
                {
                    return true;
                }
            }

            return false;
        }

        static public bool ServiceCanStart(string svcName)
        {
            ServiceController sc = new ServiceController(svcName);

            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                 (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                // Start the service if the current status is stopped.
                return true;
            }

            return false;
        }

        static public bool ServiceIsRun(string svcName)
        {
            ServiceController sc = new ServiceController(svcName);
            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                 (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                return false;
            }

            return true;
        }

        static public bool ServiceControl(string svcName)
        {
            ServiceController sc = new ServiceController(svcName);

            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                 (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                // Start the service if the current status is stopped.
                sc.Start();

                while (sc.Status == ServiceControllerStatus.Stopped)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }

            }
            else
            {
                // Stop the service if its status is not set to "Stopped".
                sc.Stop();
                while (sc.Status != ServiceControllerStatus.Stopped)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }

            }

            // Refresh and display the current service status.

            sc.Refresh();


            return true;
        }


        /// 
        /// 反安装服务.
        /// 
        /// 服务名.
        static public bool UnInstallService(string svcName)
        {
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                int DELETE = SERVICE_DELETE | SERVICE_STOP;
                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt32() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        CloseServiceHandle(svc_hndl);
                        CloseServiceHandle(sc_hndl);
                        sc_hndl = IntPtr.Zero;
                        GC.Collect();
                        return true;
                    }
                    else
                    {
                        CloseServiceHandle(svc_hndl);
                        CloseServiceHandle(sc_hndl);
                        sc_hndl = IntPtr.Zero;
                        GC.Collect();
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 强制结束进程
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool FindAndKillProcessByName(string name)
        {
            //Parameter check
            if (0 == name.Length)
            {
                return false;
            }

            //Find the named process and terminate it
            foreach (Process winProc in Process.GetProcesses())
            {
                //use equals for the task in case we kill
                //a wrong process
                if (winProc.ProcessName.Equals(name))
                {
                    winProc.Kill();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取服务安装路径
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceInstallPath(string ServiceName)
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + ServiceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            //替换掉双引号   
            path = path.Replace("\"", string.Empty);
            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();
        }
    }
}

