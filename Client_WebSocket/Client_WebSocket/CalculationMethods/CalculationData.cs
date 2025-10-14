using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Client_WebSocket.CentralBank;
using Client_WebSocket.Models;
using NLog;

namespace Client_WebSocket.CalculationMethods
{
    public sealed class CalculationData
    {
        private Logger loggerCalculationData = LogManager.GetCurrentClassLogger();
        private DateTime timeWorkingDateStart;
        private DateTime timeWorkingDateEnd;
        private int sleepTime;
        private SettingsClient settingsClient;
        private BankParser bankParser = new BankParser();
        private DefaultParser defaultParser = new DefaultParser();
        private List<BankModel> parserData = new List<BankModel>();
        private bool connected;
        public event Action<List<ResponseBankModel>> DataResponse;


        public CalculationData()
        {
        }

        public CalculationData(int sleep, DateTime timeStart, DateTime timeEnd, bool isConnected)
        {
            sleepTime = sleep;
            timeWorkingDateStart = timeStart;
            timeWorkingDateEnd = timeEnd;
            settingsClient = new SettingsClient();
            settingsClient.DataResponse += OnDataResponse;
            connected = isConnected;
        }

        public async Task DataCreationAsync()
        {
            try
            {
                var workHours = timeWorkingDateEnd - timeWorkingDateStart;
                var totalHours = (int)workHours.TotalHours;
                parserData.Clear();
                if (DateTime.Now >= timeWorkingDateStart && DateTime.Now <= timeWorkingDateEnd)
                {
                    await Task.Run(async () =>
                    {
                        //Stopwatch runTime = Stopwatch.StartNew();
                        loggerCalculationData.Info($"Запуск потока парсера данных");
                        for (var i = 1; i <= totalHours; i++)
                        {
                            loggerCalculationData.Info($"Получение данных {i} из {totalHours}");
                            parserData = connected ? bankParser.CentralBankParser() : defaultParser.GetDefaultValue();

                            if (parserData.Count > 0)
                            {
                                loggerCalculationData.Info($"Данные {i} запроса успешно получены");
                                //runTime.Stop();
                                await settingsClient.StartAsync(parserData,
                                    sleepTime /*, TimeSpan.FromMilliseconds(runTime.ElapsedMilliseconds)*/);

                                if (i < totalHours)
                                {
                                    var stopwatch = Stopwatch.StartNew();
                                    await Task.Delay(sleepTime);
                                    stopwatch.Stop();
                                    loggerCalculationData.Info(
                                        $"Задержка в {stopwatch.Elapsed} между получениями данных");
                                }
                            }
                            else
                            {
                                loggerCalculationData.Warn($"Данных нет для итерации {i}");
                                if (i < totalHours)
                                {
                                    await Task.Delay(sleepTime);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                loggerCalculationData.Error($"{ex.Message}");
            }
        }

        private void OnDataResponse(List<ResponseBankModel> responseModels)
        {
            DataResponse?.Invoke(responseModels);
        }
    }
}