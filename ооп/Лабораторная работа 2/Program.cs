namespace Lab2;

public readonly record struct Measurement(DateTime Time, double Temperature, double Humidity)
{
    public Measurement CopyWithTemperature(double temperature) => this with
    {
        Temperature = temperature,
        Time = DateTime.Now
    };

    public override string ToString() =>
        $"{Time:HH:mm:ss}: {Temperature:F1} °C, влажность {Humidity:F0}%";
}

public sealed class WeatherSensor
{
    public string Name { get; set; }
    public List<Measurement> Measurements { get; }

    public WeatherSensor(string name, IEnumerable<Measurement>? measurements = null)
    {
        Name = name;
        Measurements = measurements?.ToList() ?? [];
    }

    public WeatherSensor CopyWithName(string name) => new(name, Measurements);

    public void Add(Measurement measurement) => Measurements.Add(measurement);

    public void Print()
    {
        Console.WriteLine($"Датчик «{Name}»");
        foreach (Measurement measurement in Measurements)
            Console.WriteLine($"  {measurement}");
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Measurement original = new(DateTime.Now, 21.5, 45);
        Measurement changed = original.CopyWithTemperature(23.0);
        Console.WriteLine("Структуры являются типами значений:");
        Console.WriteLine($"Исходное измерение: {original}");
        Console.WriteLine($"Копия с изменением: {changed}");

        WeatherSensor sensor = new("Кабинет 101", [original]);
        WeatherSensor referenceCopy = sensor;
        referenceCopy.Name = "Кабинет 101, основной датчик";
        Console.WriteLine("\nКласс является ссылочным типом: изменение через вторую ссылку видно в исходном объекте.");
        sensor.Print();

        WeatherSensor independentCopy = sensor.CopyWithName("Резервный датчик");
        independentCopy.Add(changed);
        Console.WriteLine("\nНезависимая копия класса:");
        independentCopy.Print();

        Console.WriteLine($"\nПамять до сборки мусора: {GC.GetTotalMemory(false):N0} байт");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"Память после сборки мусора: {GC.GetTotalMemory(true):N0} байт");
    }
}
