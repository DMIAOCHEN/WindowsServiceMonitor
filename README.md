# Windows 系统服务监控
1. 监控Windows系统服务，如果发现所监控的服务停止，则自动将其启动。
2. 特定的时间自动重启制定的服务。

## 配置说明
在configSections节点中加入如下section:
```xml
<section name="winSrvMonitor" type="WindowsServiceMonitor.Configuration.MonitorConfig, WindowsServiceMonitor" />
```

配置节点如下：
```xml
  <winSrvMonitor>
    <!-- enable指定是否监控服务在其停止的时候自动启动 -->
    <Server enable="true">
      <!--服务名称-->
      <Name>ICCServer</Name>
      <!--进程名，在无法正常结束服务的时候需要强制杀死进程-->
      <ProcessName>ICCServer</ProcessName>
      <!--enable指定是否自动重启服务-->
      <AutoRestart enable="true">
        <!--每隔几天重启-->
        <Days>0</Days>
        <!--每天几点重启-->
        <Time>17:35:00</Time>
      </AutoRestart>
    </Server>
  </winSrvMonitor>
```

## 服务的安装、启动和卸载
程序中使用了TopShelf框架，所以命令十分简单，如下:
```
WindowsServiceMonitor.exe install //安装
WindowsServiceMonitor.exe start   //启动
WindowsServiceMonitor.exe stop    //停止
WindowsServiceMonitor.exe uninstall //卸载

```
