namespace Server_WebSocket.Models;

public sealed class CopyingDataModel
{
    public string DigitalCode { get; set; }

    public string LetterCode { get; set; }

    public string Units { get; set; }

    public string Currency { get; set; }

    public double Rate { get; set; }
}