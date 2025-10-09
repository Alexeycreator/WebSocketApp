using System;
using System.Collections.Generic;
using Client_WebSocket.Models;
using NLog;

namespace Client_WebSocket.CentralBank
{
    public sealed class DefaultParser
    {
        private Logger loggerDefaultParser = LogManager.GetCurrentClassLogger();
        private List<BankModel> defaultRates = new List<BankModel>();
        private Random random = new Random();

        private static string[] letterCodes =
        {
            "AUD", "AZN", "DZD", "AMD", "THB", "BHD", "BYN", "BGN", "BOB", "BRL", "KRW", "HKD", "UAH", "DKK", "AED",
            "USD", "VND", "EUR", "EGP", "PLN", "JPY", "INR", "IRR", "CAD", "QAR", "CUP", "MMK", "GEL", "MDL", "NGN",
            "NZD", "TMT", "NOK", "OMR", "RON", "IDR", "ZAR", "SAR", "XDR", "RSD", "SGD", "KGS", "TJS", "BDT", "KZT",
            "MNT", "TRY", "UZS", "HUF", "GBP", "CZK", "SEK", "CHF", "ETB", "CNY"
        };

        private static string[] currency =
        {
            "Австралийский доллар", "Азербайджанский манат", "Алжирских динаров", "Армянских драмов", "Батов",
            "Бахрейнский динар", "Белорусский рубль", "Болгарский лев", "Боливиано", "Бразильский реал", "Вон",
            "Гонконгский доллар", "Гривен", "Датская крона", "Дирхам ОАЭ", "Доллар США", "Донгов", "Евро",
            "Египетских фунтов", "Злотый", "Иен", "Индийских рупий", "Иранских риалов", "Канадский доллар",
            "Катарский риал", "Кубинских песо", "Кьятов", "Лари", "Молдавских леев", "Найр", "Новозеландский доллар",
            "Новый туркменский манат", "Норвежских крон", "Оманский риал", "Румынский лей", "Рупий", "Рэндов",
            "Саудовский риял", "СДР (специальные права заимствования)", "Сербских динаров", "Сингапурский доллар",
            "Сомов", "Сомони", "Так", "Тенге", "Тугриков", "Турецких лир", "Узбекских сумов", "Форинтов",
            "Фунт стерлингов", "Чешских крон", "Шведских крон", "Швейцарский франк", "Эфиопских быров", "Юань"
        };

        private static int[] digitalCode;
        private static int[] units;
        private double minScaleRnd = 10.0000;
        private double maxScaleRnd = 250.0000;

        public List<BankModel> GetDefaultValue()
        {
            return ImitationRate();
        }

        private List<BankModel> ImitationRate()
        {
            loggerDefaultParser.Info("Процесс получения имитированных курсов валют запущен...");
            try
            {
                if (digitalCode == null)
                {
                    digitalCode = new int[letterCodes.Length];
                    for (int i = 0; i < letterCodes.Length; i++)
                    {
                        digitalCode[i] = random.Next(000, 999);
                        for (int j = 0; j < i; j++)
                        {
                            if (digitalCode[j] == digitalCode[i])
                            {
                                digitalCode[i] = random.Next(000, 999);
                            }
                        }
                    }
                }

                if (units == null)
                {
                    units = new int[letterCodes.Length];
                    for (int i = 0; i < letterCodes.Length; i++)
                    {
                        units[i] = random.Next(1, 2);
                    }
                }

                double[] rate = new double[letterCodes.Length];
                for (int i = 0; i < letterCodes.Length; i++)
                {
                    rate[i] = Math.Round(random.NextDouble() * (maxScaleRnd - minScaleRnd) + minScaleRnd, 4);
                }

                for (int i = 0; i < letterCodes.Length; i++)
                {
                    defaultRates.Add(new BankModel
                    {
                        LetterCode = letterCodes[i],
                        DigitalCode = digitalCode[i].ToString(),
                        Units = units[i].ToString(),
                        Currency = currency[i],
                        Rate = rate[i].ToString()
                    });
                }

                loggerDefaultParser.Info("Данные успешно добавлены");
            }
            catch (Exception ex)
            {
                loggerDefaultParser.Error(ex.Message);
            }

            return defaultRates;
        }
    }
}