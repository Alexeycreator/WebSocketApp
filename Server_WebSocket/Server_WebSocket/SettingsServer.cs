using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using Server_WebSocket.CalculationMethods;
using Server_WebSocket.Models;

namespace Server_WebSocket;

public class SettingsServer
{
    private Logger loggerSettingsServer = LogManager.GetCurrentClassLogger();
    private string timeNow = DateTime.Now.ToShortDateString();
    private DateTime timeWorkingDateStart = DateTime.Today.AddHours(8);
    private DateTime timeWorkingDateEnd = DateTime.Today.AddHours(22);
    private ServerConfig config;
    private TcpListener tcpListener;

    public SettingsServer(IConfiguration _configuration)
    {
        config = _configuration.GetSection("ServerSettings").Get<ServerConfig>() ?? new ServerConfig();
    }


    public void Start()
    {
        Console.WriteLine($"Запуск сервера.");
        try
        {
            if (DateTime.Now <= timeWorkingDateStart)
            {
                throw new ArgumentOutOfRangeException(
                    $"Приложение запускается не раньше {timeWorkingDateStart} по МСК");
            }
            else if (DateTime.Now >= timeWorkingDateEnd)
            {
                throw new ArgumentOutOfRangeException(
                    $"Приложение работает до {timeWorkingDateEnd} по МСК");
            }
            else
            {
                string ip = config.Ip;
                int port = config.Port;
                IPAddress ipAddress = IPAddress.Parse(ip);
                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                loggerSettingsServer.Info($"Сервер запущен по ip:{ip} port:{port}");
                Console.WriteLine($"Ожидание подключений...");
                loggerSettingsServer.Info($"Создание отдельного потока для клиента");
                Thread incommingThread = new Thread(() => { IncommingConnection(tcpListener); });
                incommingThread.Start();
                CheckClosedServer();
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Сервер не удалось запустить.");
            loggerSettingsServer.Error($"{ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сервер не удалось запустить.");
            loggerSettingsServer.Error($"{ex.Message}");
        }
    }

    private void IncommingConnection(TcpListener tcpListener)
    {
        try
        {
            while (DateTime.Now >= timeWorkingDateStart && DateTime.Now <= timeWorkingDateEnd)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Console.WriteLine($"Клиент {client.Client.RemoteEndPoint} подключен");
                loggerSettingsServer.Info($"Клиент {client.Client.RemoteEndPoint} подключен");
                Thread clientThread = new Thread(() => HandleClientAsync(client));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            loggerSettingsServer.Error($"{ex.Message}");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            {
            }

            using (NetworkStream stream = client.GetStream())
            {
                loggerSettingsServer.Info($"Считывание данных от {client.Client.RemoteEndPoint}");
                string receivedData;
                byte[] lengthBuffer = new byte[4];
                int lengthBytesRead = 0;

                while (lengthBytesRead < 4)
                {
                    int read = stream.Read(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
                    if (read == 0)
                    {
                        throw new IOException($"Соединение закрыто клиентом");
                    }

                    lengthBytesRead += read;
                }

                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                loggerSettingsServer.Info($"Ожидается сообщение длиной {messageLength} байт");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] buffer = new byte[1024];
                    int totalBytesRead = 0;
                    while (totalBytesRead < messageLength)
                    {
                        int bytesToRead = Math.Min(buffer.Length, messageLength - totalBytesRead);
                        int bytesRead = stream.Read(buffer, 0, bytesToRead);
                        if (bytesRead == 0)
                        {
                            throw new IOException($"Соединение закрыто клиентом");
                        }

                        memoryStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                        loggerSettingsServer.Info($"Прочитано {totalBytesRead}/{messageLength} байт");
                    }

                    receivedData = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                loggerSettingsServer.Info($"Получены данные: {receivedData}");
                Console.WriteLine($"Получены данные от клиента\nОбработка...");
                List<BankModel> processedData = await Task.Run(() =>
                {
                    CalculationData calculationData =
                        new CalculationData(/*responseDatas, copyingDataModels, csvWriter*/);
                    return calculationData.ProcessData(/*receivedData*/);
                });
                loggerSettingsServer.Info($"Данные обработаны.");
                string responseJson = JsonConvert.SerializeObject(processedData);
                byte[] responseDataBytes = Encoding.UTF8.GetBytes(responseJson);
                byte[] lengthBytes = BitConverter.GetBytes(responseDataBytes.Length);
                stream.Write(lengthBytes, 0, lengthBytes.Length);
                stream.Write(responseDataBytes, 0, responseDataBytes.Length);
                Console.WriteLine($"Данные отправлены обратно клиенту");
                loggerSettingsServer.Info($"Отправлены обработанные данные: {processedData}");
            }
        }
        catch (IOException ex)
        {
            loggerSettingsServer.Error($"{ex.Message}");
        }
        catch (Exception ex)
        {
            loggerSettingsServer.Error($"{ex.Message}");
        }
    }

    private void CheckClosedServer()
    {
        Thread checkCloseServerThread = new Thread(() =>
        {
            while (true)
            {
                if (DateTime.Now >= timeWorkingDateEnd)
                {
                    tcpListener.Stop();
                    loggerSettingsServer.Info($"Сервер остановлен.");
                    break;
                }

                Thread.Sleep(TimeSpan.FromMinutes(config.SleepTimeCheckedClosed));
            }
        });
        checkCloseServerThread.Start();
    }
}