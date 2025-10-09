using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        private List<BankModel> bankDatas = new List<BankModel>();
        private BankParser bankParser = new BankParser();
        private DefaultParser defaultParser = new DefaultParser();

        public CalculationData()
        {
        }

        public CalculationData(int sleep, DateTime timeStart, DateTime timeEnd)
        {
            sleepTime = sleep;
            timeWorkingDateStart = timeStart;
            timeWorkingDateEnd = timeEnd;
            settingsClient = new SettingsClient();
        }

        public void DataCreation()
        {
            try
            {
                TimeSpan workHours = timeWorkingDateEnd - timeWorkingDateStart;
                int totalHours = (int)workHours.TotalHours;
                if (DateTime.Now >= timeWorkingDateStart && DateTime.Now <= timeWorkingDateEnd)
                {
                    Thread parserThread = new Thread(() =>
                    {
                        loggerCalculationData.Info($"Запуск потока парсера данных");
                        for (int i = 1; i <= totalHours; i++)
                        {
                            loggerCalculationData.Info($"Получение данных {i} из {totalHours}");
                            var parserData = defaultParser.GetDefaultValue();
                            if (parserData.Count > 0)
                            {
                                loggerCalculationData.Info($"Данные {i} запроса успешно получены");
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException($"Данных нет.");
                            }

                            if (i < totalHours)
                            {
                                var stopwatch = Stopwatch.StartNew();
                                Thread.Sleep(sleepTime);
                                stopwatch.Stop();
                                loggerCalculationData.Info($"Задержка в {stopwatch.Elapsed} между получениями данных");
                            }
                        }
                    });
                    parserThread.Start();
                }
            }
            catch (ArgumentException ex)
            {
                loggerCalculationData.Error($"{ex.Message}");
                /*var exParser = bankDatas.Add(new BankModel()
                {
                    DigitalCode = "0",
                    LetterCode = "Default",
                    Currency = "Default",
                    Units = "0",
                    Rate = "0"
                });*/
            }
            catch (Exception ex)
            {
                loggerCalculationData.Error($"{ex.Message}");
            }
        }
    }
}