namespace Server_WebSocket;

public sealed class ServerConfig
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public int SleepTimeCheckedClosed { get; set; }
}