using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Client_WebSocket.Models;
using HtmlAgilityPack;

namespace Client_WebSocket.CentralBank
{
    public sealed class BankParser
    {
        private Logger loggerBankParser = LogManager.GetCurrentClassLogger();
        private static string dateGetRate = DateTime.Now.ToShortDateString();

        private string urlCentralbank =
            $"https://www.cbr.ru/currency_base/daily/?UniDbQuery.Posted=True&UniDbQuery.To={dateGetRate}";

        private readonly HttpClient httpClient = new HttpClient();
        private const int countColumns = 5;
        private List<BankModel> bankModels = new List<BankModel>();

        public List<BankModel> CentralBankParser()
        {
            return GetRate();
        }

        private List<BankModel> GetRate()
        {
            loggerBankParser.Info("Процесс получения курса валют запущен...");
            try
            {
                bankModels.Clear();
                loggerBankParser.Info($"Подключение к данным по адресу: {urlCentralbank}");
                var httpResponseMessage = httpClient.GetAsync(urlCentralbank).Result;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    loggerBankParser.Info($"Подключение прошло успешно: {httpResponseMessage.StatusCode}");
                    var htmlResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(htmlResponse))
                    {
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(htmlResponse);
                        var container = document.GetElementbyId("content");
                        if (container != null)
                        {
                            int cells = document.GetElementbyId("content").ChildNodes.FindFirst("tbody").ChildNodes
                                .Where(c => c.Name == "tr").First().ChildNodes.Where(c => c.Name == "th").Count();
                            if (cells == countColumns)
                            {
                                var tableBody = document.GetElementbyId("content").ChildNodes.FindFirst("tbody")
                                    .ChildNodes.Where(t => t.Name == "tr").Skip(1).ToArray();
                                loggerBankParser.Info("Извлечение данных");
                                foreach (var tableRow in tableBody)
                                {
                                    var cellDigitalCode = tableRow.SelectSingleNode(".//td[1]").InnerText;
                                    var cellLetterCode = tableRow.SelectSingleNode(".//td[2]").InnerText;
                                    var cellUnits = tableRow.SelectSingleNode(".//td[3]").InnerText;
                                    var cellCurrency = tableRow.SelectSingleNode(".//td[4]").InnerText;
                                    var cellRate = tableRow.SelectSingleNode(".//td[5]").InnerText;

                                    bankModels.Add(new BankModel
                                    {
                                        DigitalCode = cellDigitalCode,
                                        LetterCode = cellLetterCode,
                                        Units = cellUnits,
                                        Currency = cellCurrency,
                                        Rate = cellRate
                                    });
                                    if (bankModels != null)
                                    {
                                        loggerBankParser.Info(
                                            $"Данные успешно получены. Количество {bankModels.Count}");
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException(
                                    $"Количество столбцов данных таблицы изменилось с {countColumns} на {cells}");
                            }
                        }
                        else
                        {
                            throw new NullReferenceException($"Данных нет");
                        }
                    }
                    else
                    {
                        throw new NullReferenceException($"Ответ от страницы с данными пришел пустой");
                    }
                }
                else
                {
                    throw new HttpRequestException($"Подключиться не удалось: {httpResponseMessage.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                loggerBankParser.Error(ex.Message);
            }
            catch (Exception ex)
            {
                loggerBankParser.Error(ex.Message);
            }

            return bankModels;
        }
    }
}