using Microsoft.Extensions.Configuration;
using NLog;
using Server_WebSocket;

Logger loggerProgram = LogManager.GetCurrentClassLogger();

try
{
    var configuration = new ConfigurationManager().SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
    SettingsServer server = new SettingsServer(configuration);
    server.Start();
}
catch (FileNotFoundException ex)
{
    loggerProgram.Error($"{ex.Message}");
}
catch (Exception ex)
{
    loggerProgram.Error($"{ex.Message}");
}
finally
{
    LogManager.Shutdown();
}