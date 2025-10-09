using Newtonsoft.Json;
using NLog;
using Server_WebSocket.Models;
using Server_WebSocket.WorkFiles;

namespace Server_WebSocket.CalculationMethods;

public class CalculationData
{
    private Logger loggerCalculationData = LogManager.GetCurrentClassLogger();
    private static string dateGetRate = DateTime.Now.ToShortDateString();

    private readonly string csvFilePath =
        Path.Combine(Directory.GetCurrentDirectory(), "CentralBank", $"{dateGetRate}");

    private CsvWriter csvWriter = new CsvWriter();
    private List<ResponseDataModel> responseDatas;
    private List<BankModel> bankDatas;
    private List<CopyingDataModel> copyingDatas;

    public CalculationData(List<ResponseDataModel> respData, List<CopyingDataModel> copData)
    {
        responseDatas = respData;
        copyingDatas = copData;
        if (!Directory.Exists(csvFilePath))
        {
            Directory.CreateDirectory(csvFilePath);
        }

        string dateTimeNowRate = DateTime.Now.ToString("HH:mm");
        string fileNameRate = $"Rate_{dateTimeNowRate}.csv";
        csvFilePath = Path.Combine(csvFilePath, fileNameRate);
        if (!File.Exists(csvFilePath))
        {
            File.Create(csvFilePath).Close();
        }
    }

    public List<BankModel> ProcessData(string data)
    {
        try
        {
            var coming = JsonConvert.DeserializeObject<List<BankModel>>(data);
            loggerCalculationData.Info("Запись данных приходящего файла.");
            csvWriter.Write(csvFilePath, coming);
            loggerCalculationData.Info("Запись данных успешно выполнена.");
            loggerCalculationData.Info($"Преобразование полученных данных для отправки.");
            responseDatas = coming.Select(bankItem => new ResponseDataModel()
            {
                DigitalCode = bankItem.DigitalCode,
                LetterCode = bankItem.LetterCode,
                Units = bankItem.Units,
                Currency = bankItem.Currency,
                Rate = Convert.ToDouble(bankItem.Rate)
            }).ToList();
            loggerCalculationData.Info("Копирование полученных данных для вычислений");
            int countResp = responseDatas.Count;
            string tempJsonRespData = JsonConvert.SerializeObject(responseDatas);
            var copyingData = JsonConvert.DeserializeObject<List<CopyingDataModel>>(tempJsonRespData);
            copyingDatas.AddRange(copyingData);
            loggerCalculationData.Info("Данные скопированы");
            responseDatas.Clear();
            loggerCalculationData.Info("Анализ данных");
            var digitalCodeGroups = copyingDatas.GroupBy(item => item.DigitalCode);
            foreach (var group in digitalCodeGroups)
            {
                loggerCalculationData.Info($"Группа {group.Key}: {group.Count()} элементов");
                foreach (var item in group)
                {
                    loggerCalculationData.Info($"{item.DigitalCode}: {item.Rate}");
                }

                if (group.Count() > 1)
                {
                    double averageRate = group.Average(item => item.Rate);
                    var firstItem = group.First();
                    responseDatas.Add(new ResponseDataModel()
                    {
                        DigitalCode = firstItem.DigitalCode,
                        LetterCode = firstItem.LetterCode,
                        Units = firstItem.Units,
                        Currency = firstItem.Currency,
                        Rate = Math.Round(averageRate, 4)
                    });
                    loggerCalculationData.Info($"Вычислено среднее для {firstItem.DigitalCode}: {averageRate}");
                }
            }

            if (responseDatas.Count == 0)
            {
                string tempJsonRespData2 = JsonConvert.SerializeObject(copyingDatas);
                List<ResponseDataModel> copyingData2 =
                    JsonConvert.DeserializeObject<List<ResponseDataModel>>(tempJsonRespData2);
                responseDatas.AddRange(copyingData2);
            }

            bankDatas = responseDatas.Select(item => new BankModel
            {
                DigitalCode = item.DigitalCode,
                LetterCode = item.LetterCode,
                Units = item.Units,
                Currency = item.Currency,
                Rate = item.Rate.ToString()
            }).ToList();
            loggerCalculationData.Info($"Данные успешно преобразованы. Обработано {bankDatas.Count} записей");
        }
        catch (Exception ex)
        {
            loggerCalculationData.Error(ex.Message);
        }

        return bankDatas;
    }
}