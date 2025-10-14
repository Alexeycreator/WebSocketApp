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
    private DateTime timeWorkingDateEnd = DateTime.Today.AddHours(24);
    private ServerConfig config;
    private TcpListener tcpListener;
    private List<ResponseDataModel> responseDatas = new List<ResponseDataModel>();
    private List<CopyingDataModel> copyingDatas = new List<CopyingDataModel>();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public SettingsServer(IConfiguration _configuration)
    {
        config = _configuration.GetSection("ServerSettings").Get<ServerConfig>() ?? new ServerConfig();
    }

    public void Start()
    {
        Console.WriteLine($"Запуск сервера.");
        try
        {
            if (DateTime.Now < timeWorkingDateStart)
            {
                throw new ArgumentOutOfRangeException(
                    $"Приложение запускается не раньше {timeWorkingDateStart} по МСК");
            }
            else if (DateTime.Now > timeWorkingDateEnd)
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
                _ = AcceptClientsAsync(cancellationTokenSource.Token);
                Console.WriteLine("Нажмите любую клавишу для остановки сервера...");
                Console.ReadKey();
                cancellationTokenSource.Cancel();
                tcpListener.Stop();
                loggerSettingsServer.Info("Сервер остановлен.");
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

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested &&
                   DateTime.Now >= timeWorkingDateStart &&
                   DateTime.Now <= timeWorkingDateEnd)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync(cancellationToken);
                loggerSettingsServer.Info($"Новое подключение от {client.Client.RemoteEndPoint}");
                _ = Task.Run(() => HandleClientAsync(client), cancellationToken);
                loggerSettingsServer.Info($"Запущена задача для клиента");
            }
        }
        catch (OperationCanceledException)
        {
            loggerSettingsServer.Info("Сервер остановлен по запросу пользователя");
        }
        catch (Exception ex)
        {
            loggerSettingsServer.Error($"Ошибка в AcceptClientsAsync: {ex.Message}");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            await using (NetworkStream stream = client.GetStream())
            {
                loggerSettingsServer.Info($"Считывание данных от {client.Client.RemoteEndPoint}");
                string receivedData;
                byte[] lengthBuffer = new byte[4];
                int lengthBytesRead = 0;
                while (lengthBytesRead < 4)
                {
                    int read = await stream.ReadAsync(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
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
                        int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
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
                var processedData = await Task.Run(() =>
                {
                    responseDatas.Clear(); 
                    //copyingDatas.Clear();
                    CalculationData calculationData =
                        new CalculationData(responseDatas, copyingDatas /*, csvWriter*/);
                    return calculationData.ProcessData(receivedData);
                });
                loggerSettingsServer.Info($"Данные обработаны.");
                string responseJson = JsonConvert.SerializeObject(processedData);
                byte[] responseDataBytes = Encoding.UTF8.GetBytes(responseJson);
                byte[] lengthBytes = BitConverter.GetBytes(responseDataBytes.Length);
                await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                await stream.WriteAsync(responseDataBytes, 0, responseDataBytes.Length);
                Console.WriteLine($"Данные отправлены обратно клиенту");
                Console.WriteLine("\nНажмите любую клавишу для остановки сервера...");
                loggerSettingsServer.Info($"Отправлены обработанные данные клиенту");
            }
        }
        catch (IOException ex)
        {
            loggerSettingsServer.Error($"Ошибка ввода-вывода: {ex.Message}");
        }
        catch (Exception ex)
        {
            loggerSettingsServer.Error($"Ошибка обработки клиента: {ex.Message}");
        }
    }

    public void Stop()
    {
        cancellationTokenSource?.Cancel();
        tcpListener?.Stop();
    }
}