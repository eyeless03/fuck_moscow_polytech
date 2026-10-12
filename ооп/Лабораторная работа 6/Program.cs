namespace Lab6;

public sealed class DeviceReading
{
    public string DeviceName { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public double Voltage { get; set; }
    public string? Comment { get; set; }
    public int? ErrorCode { get; set; }
    public double? Temperature { get; set; }
}

internal static class Program
{
    private const string UnknownDevice = "Неизвестное устройство";
    private const double UnknownTemperature = -273.15;

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        DeviceReading first = new()
        {
            DeviceName = "Контроллер A",
            SequenceNumber = 1,
            Voltage = 12.4,
            Comment = null,
            ErrorCode = null,
            Temperature = 36.7
        };
        DeviceReading second = new()
        {
            DeviceName = "Контроллер B",
            SequenceNumber = 2,
            Voltage = 11.9,
            Comment = "Требуется проверка",
            ErrorCode = 17,
            Temperature = null
        };

        PrintReading(first);
        PrintReading(second);
        FillMissingComment(first);
        Console.WriteLine($"После ??=: {first.Comment}");

        int nameLength = first.DeviceName!.Length;
        Console.WriteLine($"Длина имени первого устройства: {nameLength}");
        Console.WriteLine($"Имя для null-объекта: {GetDeviceName(null)}");
        Console.WriteLine($"Температура второго устройства: {GetTemperature(second):F2}");

        // string nonNullable = null;   // Предупреждение CS8600: null присваивается non-nullable string.
        string suppressedWarning = null!; // Оператор ! подавляет предупреждение, но значение остаётся null.
        Console.WriteLine($"null! не заменяет значение: {suppressedWarning is null}");
        // int number = null;           // Ошибка компиляции: int не допускает null.
        // int number = null!;          // Оператор ! не делает значимый тип nullable; ошибка остаётся.
    }

    private static void PrintReading(DeviceReading reading)
    {
        ArgumentNullException.ThrowIfNull(reading);
        Console.WriteLine($"\nУстройство: {reading.DeviceName}");
        Console.WriteLine($"Номер: {reading.SequenceNumber}");
        Console.WriteLine($"Напряжение: {reading.Voltage:F1} В");
        Console.WriteLine($"Комментарий: {reading.Comment ?? "данные отсутствуют"}");
        Console.WriteLine($"Код ошибки: {reading.ErrorCode?.ToString() ?? "данные отсутствуют"}");
        Console.WriteLine($"Температура: {reading.Temperature?.ToString("F1") ?? "данные отсутствуют"}");
    }

    private static void FillMissingComment(DeviceReading reading) =>
        reading.Comment ??= "Комментарий добавлен автоматически";

    private static string GetDeviceName(DeviceReading? reading) =>
        reading?.DeviceName ?? UnknownDevice;

    private static double GetTemperature(DeviceReading reading) =>
        reading.Temperature ?? UnknownTemperature;
}
