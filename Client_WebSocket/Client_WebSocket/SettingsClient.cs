using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using System.Threading.Tasks;
using Client_WebSocket.Models;
using NLog;

namespace Client_WebSocket
{
    public sealed class SettingsClient
    {
        private Logger loggerSettingsClient = LogManager.GetCurrentClassLogger();
        private TcpClient client;
        private NetworkStream stream;

        public void Start(List<BankModel> dataToSend, int sleepTime)
        {
            string serverIp = ConfigurationManager.AppSettings["ip"];
            string serverPort = ConfigurationManager.AppSettings["port"];
            try
            {
                loggerSettingsClient.Info($"Запуск клиента");
            }
            catch (Exception ex)
            {
                loggerSettingsClient.Error($"{ex.Message}");
            }
        }
    }
}