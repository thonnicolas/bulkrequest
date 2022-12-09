<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=4.4.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false"/>
  </configSections>
  <appSettings>
    <add key="ServerObject.SyncInterval" value="60"/>
    <add key="ResultOutPath" value="f:\test\test_multitasking.txt"/>
    <add key="SampleTestCaseCommandRequest" value="f:\TestGSMElementCommands.txt"/>

    <!--<add key="ServiceServerUrl" value="http://192.168.165.166:83/Execute.svc" />-->
    <!--<add key="ServiceServerUrl" value="http://192.168.165.116:83/Execute.svc" />-->
    <!--<add key="ServiceServerUrl" value="http://192.168.165.69:8998/Asiacell/mdw_EMAClient.svc" />-->
    <!--<add key="ServiceServerUrl" value="http://192.168.165.5:83/Execute.svc" />-->
    <!--<add key="ServiceServerUrl" value="http://192.168.164.142:83/Execute.svc"/>-->
    <add key="ServiceServerUrl" value="http://localhost:5885/Execute.svc"/>
    <!--<add key="ServiceServerUrl" value="http://192.168.164.91:94/Execute.svc" />-->
    <add key="MaxPoolSize" value="5"/>
    <add key="ClientSettingsProvider.ServiceUri" value=""/>
    <!-- air -->
    <add key="Host" value="10.77.101.15:10011"/>
    <add key="HandShakCMD" value="G:/Asiacell/Application/ProductionDotNet/Projects/ITMDW_1/ITMDW/EMAClients/HandShak.xml"/>
    <add key="EMALogin" value="G:/Asiacell/Application/ProductionDotNet/Projects/ITMDW_1/ITMDW/EMAClients/Login.xml"/>
    <add key="EMALogout" value="G:/Asiacell/Application/ProductionDotNet/Projects/ITMDW_1/ITMDW/EMAClients/Logout.xml"/>
    <add key="HeaderTemplate" value="G:/Asiacell/Application/ProductionDotNet/Projects/ITMDW_1/ITMDW/EMAClients/HeaderTemplate.xml"/>
    <add key="ProvisioningTemplate" value="G:/Asiacell/Application/ProductionDotNet/Projects/ITMDW_1/ITMDW/EMAClients/ProvisioningTemplate.xml"/>
    <add key="SuccessRespCode" value="[0];[240021040];"/>
    <add key="ElementID" value="10"/>
    <add key="AutoSyn_DB_IntervalSecond" value="60"/>

    <add key="IsSchedule" value="0"/>
    <add key="IsSameSchedule" value="1"/>
    <add key="ScheduleInterval" value="1"/>
    <add key="ElementTypeID" value="20"/>
    <!-- 10: EMA, 11: AIR, 12: Server, 13: dClear Session, 14: Element, 15: HxC, 16: BlackBox, 17: Test Transaction Log old, 18: Test Transaction Log With Array Binding -->
    <add key="NumberOfClient" value="1"/>
    <add key="NumberOfRequestPerClient" value="50"/>
    <add key="DBTransactionBulkInsert" value="100"/>
    <add key="HttpTimeout" value="5000"/>
    <add key="HttpKeepAlive" value="true"/>
    <add key="EmaSessionPath" value="e:\ema\sessions.txt"/>
    <add key="TestCount" value="30"/>
    <add key="GWUser" value="bulkinsert" />
    <add key="GWPassword" value="itbulkinsert" />
    <!--<add key="GWUser" value="bulkinsert" />
    <add key="GWPassword" value="itbulkinsert" />-->
    <add key="migrate_table" value="tbl_bulk_execute"/>
    <add key="ApplicationID" value="BULK_EXECUTE"/>
    <add key="RechargeLogCommands" value="[8790],[9687]"/>
    <!--<add key="SignalRConnnectionURL" value="http://webitad01-n1.asiacell.com:8080,http://webitad01-n2.asiacell.com:8080"/>-->
    <add key="SignalRConnnectionURL" value=""/>
  </appSettings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/>
  </startup>
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="mdw_SMSSoap" closeTimeout="00:01:00" openTimeout="00:01:00"
          receiveTimeout="00:10:00" sendTimeout="00:01:00" allowCookies="false"
          bypassProxyOnLocal="false" hostNameComparisonMode="StrongWildcard"
          maxBufferPoolSize="524288" maxBufferSize="65536" maxReceivedMessageSize="65536"
          textEncoding="utf-8" transferMode="Buffered" useDefaultWebProxy="true"
          messageEncoding="Text">
          <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384"
            maxBytesPerRead="4096" maxNameTableCharCount="16384" />
          <security mode="None">
            <transport clientCredentialType="None" proxyCredentialType="None"
              realm="" />
            <message clientCredentialType="UserName" algorithmSuite="Default" />
          </security>
        </binding>
        <binding name="AsiacellWebSMSServiceServiceSoapBinding" />
      </basicHttpBinding>
      <wsHttpBinding>
        <binding name="WSHttpBinding_IService1" closeTimeout="00:01:00"
          openTimeout="00:01:00" receiveTimeout="00:10:00" sendTimeout="00:01:00"
          bypassProxyOnLocal="false" transactionFlow="false" hostNameComparisonMode="StrongWildcard"
          maxBufferPoolSize="524288" maxReceivedMessageSize="65536" messageEncoding="Text"
          textEncoding="utf-8" useDefaultWebProxy="true" allowCookies="false">
          <readerQuotas maxDepth="32" maxStringContentLength="8192" maxArrayLength="16384"
            maxBytesPerRead="4096" maxNameTableCharCount="16384" />
          <reliableSession ordered="true" inactivityTimeout="00:10:00"
            enabled="false" />
          <security mode="Message">
            <transport clientCredentialType="Windows" proxyCredentialType="None"
              realm="" />
            <message clientCredentialType="Windows" negotiateServiceCredential="true"
              algorithmSuite="Default" />
          </security>
        </binding>
      </wsHttpBinding>
    </bindings>
    <client>
      <endpoint address="http://cbsappt2.asiacell.com:888/Service1.svc"
        binding="wsHttpBinding" bindingConfiguration="WSHttpBinding_IService1"
        contract="ServiceReference1.IService1" name="WSHttpBinding_IService1">
        <identity>
          <servicePrincipalName value="host/CBSAPPT2.asiacell.com" />
        </identity>
      </endpoint>
      <endpoint address="http://192.168.165.115/mdw_SMS.asmx" binding="basicHttpBinding"
        bindingConfiguration="mdw_SMSSoap" contract="SMSService.mdw_SMSSoap"
        name="mdw_SMSSoap" />
      <endpoint address="http://192.168.164.142:10000/services/asiacellWebSMS"
        binding="basicHttpBinding" bindingConfiguration="AsiacellWebSMSServiceServiceSoapBinding"
        contract="com.asiacell.smsclientv2.AsiacellWebSMSService" name="AsiacellWebSMSServicePort" />
    </client>
  </system.serviceModel>
  <connectionStrings>
    <add name="itmdw_ConnString" connectionString="User Id=itmdw;Password=Asi@2014GW ;Data Source=AGWPROD ;Min Pool Size=2;Max Pool Size=50;Connection Lifetime=0;Connection Timeout=15;Pooling=true;" providerName="Oracle.DataAccess.Client"/>
    <!--<add name="itmdw_ConnString" connectionString="User Id=itmdw;Password=Asi@2014GW ;Data Source=AGWPROD ;Min Pool Size=2;Max Pool Size=50;Connection Lifetime=0;Connection Timeout=15;Pooling=true;" providerName="Oracle.DataAccess.Client"/>-->

    <!--<add name="Entities" connectionString="metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;provider=Oracle.DataAccess.Client;provider connection string=&quot;DATA SOURCE=RACITPROD;PASSWORD=itmdw_tst;PERSIST SECURITY INFO=True;USER ID=ITMDW_TST&quot;" providerName="System.Data.EntityClient" />-->
  </connectionStrings>
  <log4net>
    <appender name="Console" type="log4net.Appender.ConsoleAppender">
      <layout type="log4net.Layout.PatternLayout">
        <!-- Pattern to output the caller's file name and line number -->
        <conversionPattern value="%date [Thread : %thread] [Line : %line] %-5level : %logger - %message%newline"/>
      </layout>
    </appender>
    <appender name="RollingFile" type="log4net.Appender.RollingFileAppender">
      <file value="logs\logfile.log"/>
      <appendToFile value="true"/>
      <maximumFileSize value="100MB"/>
      <maxSizeRollBackups value="100"/>
      <layout type="log4net.Layout.PatternLayout">
        <!--conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" /-->
        <conversionPattern value="%date [Thread : %thread] [Line : %line] %-5level : %logger - %message%newline"/>
      </layout>
    </appender>
    <root>
      <level value="DEBUG"/>
      <appender-ref ref="Console"/>
      <appender-ref ref="RollingFile"/>
    </root>
  </log4net>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral"/>
        <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0"/>
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
