# Windows ϵͳ������
1. ���Windowsϵͳ���������������صķ���ֹͣ�����Զ�����������
2. �ض���ʱ���Զ������ƶ��ķ���

## ����˵��
��configSections�ڵ��м�������section:
```xml
<section name="winSrvMonitor" type="WindowsServiceMonitor.Configuration.MonitorConfig, WindowsServiceMonitor" />
```

���ýڵ����£�
```xml
  <winSrvMonitor>
    <!-- enableָ���Ƿ��ط�������ֹͣ��ʱ���Զ����� -->
    <Server enable="true">
      <!--��������-->
      <Name>ICCServer</Name>
      <!--�����������޷��������������ʱ����Ҫǿ��ɱ������-->
      <ProcessName>ICCServer</ProcessName>
      <!--enableָ���Ƿ��Զ���������-->
      <AutoRestart enable="true">
        <!--ÿ����������-->
        <Days>0</Days>
        <!--ÿ�켸������-->
        <Time>17:35:00</Time>
      </AutoRestart>
    </Server>
  </winSrvMonitor>
```

## ����İ�װ��������ж��
������ʹ����TopShelf��ܣ���������ʮ�ּ򵥣�����:
```
WindowsServiceMonitor.exe install //��װ
WindowsServiceMonitor.exe start   //����
WindowsServiceMonitor.exe stop    //ֹͣ
WindowsServiceMonitor.exe uninstall //ж��

```
