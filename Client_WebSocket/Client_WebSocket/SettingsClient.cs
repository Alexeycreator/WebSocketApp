using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Client_WebSocket.Models;
using Newtonsoft.Json;
using NLog;

namespace Client_WebSocket
{
    public sealed class SettingsClient
    {
        private Logger loggerSettingsClient = LogManager.GetCurrentClassLogger();
        private TcpClient client;
        private NetworkStream stream;
        public event Action<List<ResponseBankModel>> DataResponse;


        public async Task StartAsync(List<BankModel> dataToSend, int sleepTime/*, TimeSpan runTime*/)
        {
            string serverIp = ConfigurationManager.AppSettings["ip"];
            string serverPort = ConfigurationManager.AppSettings["port"];
            try
            {
                loggerSettingsClient.Info($"Запуск клиента");
                Task.Run(() => ProcessRequestsLoopAsync(dataToSend, sleepTime, serverIp, Convert.ToInt32(serverPort)));
                /*if (sleepTime > runTime.TotalMilliseconds)
                {
                    int remainingDelay = sleepTime - (int)runTime.TotalMilliseconds;
                    //await Task.Delay(remainingDelay);
                }*/
            }
            catch (Exception ex)
            {
                loggerSettingsClient.Error($"{ex.Message}");
            }
        }

        private async Task ProcessRequestsLoopAsync(List<BankModel> dataToSend, int sleepTime, string serverIp,
            int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    loggerSettingsClient.Info("Подключение к серверу...");
                    await client.ConnectAsync(serverIp, port);
                    var stream = client.GetStream();
                    loggerSettingsClient.Info("Подключение к серверу установлено");
                    string jsonData = JsonConvert.SerializeObject(dataToSend);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);
                    byte[] lengthBytes = BitConverter.GetBytes(dataBytes.Length);
                    await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                    await stream.WriteAsync(dataBytes, 0, dataBytes.Length);
                    loggerSettingsClient.Info($"Данные отправлены: {dataBytes.Length} байт");
                    var response = await ReceiveSingleResponseAsync(stream);
                    if (response != null)
                    {
                        loggerSettingsClient.Info($"Получено {response.Count} записей с сервера");
                        DataResponse?.Invoke(response);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                loggerSettingsClient.Info("Операция отменена");
            }
            catch (IOException ioEx)
            {
                loggerSettingsClient.Error($"Ошибка ввода-вывода: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                loggerSettingsClient.Error($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        private async Task<List<ResponseBankModel>> ReceiveSingleResponseAsync(NetworkStream stream)
        {
            var lengthBuffer = new byte[4];
            var lengthBytesRead = 0;
            while (lengthBytesRead < 4)
            {
                var read = await stream.ReadAsync(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
                if (read == 0)
                {
                    throw new IOException("Сервер закрыл соединение");
                }

                lengthBytesRead += read;
            }

            var messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            loggerSettingsClient.Info($"Ожидается ответ длиной {messageLength} байт");
            if (messageLength == -1)
            {
                loggerSettingsClient.Info("Получен сигнал завершения от сервера");
                return null;
            }

            if (messageLength <= 0 || messageLength > 10 * 1024 * 1024)
            {
                loggerSettingsClient.Error($"Некорректная длина сообщения: {messageLength}");
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[4096];
                var totalBytesRead = 0;
                while (totalBytesRead < messageLength)
                {
                    var bytesToRead = Math.Min(buffer.Length, messageLength - totalBytesRead);
                    var bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
                    if (bytesRead == 0)
                    {
                        throw new IOException("Соединение разорвано до получения полного ответа");
                    }

                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    loggerSettingsClient.Debug($"Получено {totalBytesRead}/{messageLength} байт ответа");
                }

                var receivedData = memoryStream.ToArray();
                var responseData = Encoding.UTF8.GetString(receivedData);
                loggerSettingsClient.Info($"Получены обработанные данные длиной {responseData.Length} символов");
                try
                {
                    var responseModels = JsonConvert.DeserializeObject<List<ResponseBankModel>>(responseData);
                    return responseModels;
                }
                catch (JsonException jsonEx)
                {
                    loggerSettingsClient.Error($"Ошибка десериализации: {jsonEx.Message}");
                    return null;
                }
            }
        }
    }
}