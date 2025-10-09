using System.Text;
using Server_WebSocket.Models;

namespace Server_WebSocket.WorkFiles;

public sealed class CsvWriter
{
    public void Write(string csvFilePath, List<BankModel> rates)
    {
        StringBuilder csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("DigitalCode;LetterCode;Units;Currency;Rate");
        foreach (var rate in rates)
        {
            csvBuilder.AppendLine($"{rate.DigitalCode};{rate.LetterCode};{rate.Units};{rate.Currency};{rate.Rate}");
        }

        File.WriteAllText(csvFilePath, csvBuilder.ToString());
    }
}